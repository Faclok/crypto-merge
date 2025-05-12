using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands.Abstractions;
using crypto_merge.Tg.Bot.Services;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands;

public class MainPage(MessageSender messageSender, UserService userService) : IMessageCommand, ITextCommand
{
    private readonly SendMessage _defaultMessage = new()
    {
        Text = "Главное меню",
        ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html,
        KeyboardMarkup = Keyboard.CreateReply([
            ["Реферальная программа"],
            ["Баланс","Пополнение баланса"],
            ["Управление банками"],
            ["FAQ"],
            ]),
    };

    public string Command => "/start";
    public string[] MessageKeys => ["Главное меню", "Назад"];

    public async Task Handler(Message message, string[] args)
    {
        var user = message.From;

        _defaultMessage.Text = """
                Добро пожаловать в CryptoMerge 👋
                📙 Алгоритм работы бота простой - с каждого пополнения вы получаете + 4.5% на свой баланс
                🧭 Инструкции по банкам и боту на канале в закрепе - https://t.me/+LrrfO5kKiqJlMGYy
                ⚠️ Одновременно к боту может быть подключен только один банк!
                ⚠️ После удаления банк невозможно добавить повторно!
                ⚠️ В боте действует страховой депозит в размере 50$
                💸 Получайте дополнительный пассивный доход, приглашая своих друзей (реферальная программа)
                """;

        if(message.Text != null && message.Text.Contains(Command) && !await userService.ContainsAsync(message.Chat.Id))
        {
            if (args.Length == 0)
                await userService.CreateAsync(message.Chat.Id, user?.FirstName + " " + user?.LastName, user?.Username);
            else
                await userService.CreateAsync(message.Chat.Id, user?.FirstName + " " + user?.LastName, user?.Username, int.TryParse(args[0], out var parse) ? parse: null);
        }

        await messageSender.SendMessageAsync(message.Chat.Id, _defaultMessage);
    }

    public Task Handler(Message message)
    {
        return messageSender.SendMessageAsync(message.Chat.Id, _defaultMessage);
    }
}
