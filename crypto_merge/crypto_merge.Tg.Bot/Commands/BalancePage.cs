using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands.Abstractions;
using crypto_merge.Tg.Bot.Services;
using InternetDatabase.EntityDTO;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands;

public class BalancePage(ITelegramBotClient client, MessageSender messageSender, WalletService walletService, MessageSender sender) : ITextCommand, ICallbackCommand
{
    public string[] MessageKeys => ["Баланс"];

    public const string CALLBACK_KEY = nameof(BalancePage);
    public string CallbackKey => CALLBACK_KEY;

    public const string ONE_DAY = "one_day";
    public const string THREE_DAY = "three_day";

    public async Task Handler(Message message)
    {
        var sumMoney = await walletService.GetTotalBalanceAndTempAsync(message.Chat.Id);

        await messageSender.SendMessageAsync(message.Chat,
            text: $"Ваш баланс: {sumMoney:0.##}",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
    }

    public async Task Handler(CallbackQuery callbackQuery, string[] args)
    {

        if (callbackQuery.Message is not { } message)
            return;

        var sum = await walletService.GetTotalBalanceAndTempAsync(message.Chat.Id);

        var mainText = "";
        var mainTextStrings = new List<string>();

        var sendMessageMain = new SendMessage()
        {
            Text = mainText,
            ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html,
        };

        try
        {

       

        switch (args[0])
        {
            case ONE_DAY:

                mainText = GetMainText(sum, await walletService.GetTransactionWalletYesterdayAsync(message.Chat.Id));
                mainTextStrings = SplitString(mainText, 4000);
                sendMessageMain.Text = mainTextStrings[0];
                if (mainTextStrings.Count < 2)
                {
                    sendMessageMain.KeyboardMarkup = Keyboard.CreateInline([[new("За 3 дня", CallbackKey + " " + THREE_DAY)]]);
                    await sender.EditMessageAsync(message.Chat, message.MessageId, sendMessageMain);
                    return;
                }
                await sender.EditMessageAsync(message.Chat, message.MessageId, sendMessageMain);
                break;

            case THREE_DAY:

                mainText = GetMainText(sum, await walletService.GetTransactionWalletThreeDayBackAsync(message.Chat.Id));
                mainTextStrings = SplitString(mainText, 4000);
                sendMessageMain.Text = mainTextStrings[0];

                if (mainTextStrings.Count < 2)
                {
                    sendMessageMain.KeyboardMarkup = Keyboard.CreateInline([[new("За 1 день", CallbackKey + " " + ONE_DAY)]]);
                    await sender.EditMessageAsync(message.Chat, message.MessageId, sendMessageMain);
                    return;
                }
                await sender.EditMessageAsync(message.Chat, message.MessageId, sendMessageMain);
                break;

            default:

                await client.SendTextMessageAsync(message.Chat, "Что-то пошло не по плану");
                return;
        }
        }
        catch (Exception)
        {
        }

        foreach (var item in mainTextStrings.Skip(1))
        {
            sendMessageMain.Text = item;
            await sender.SendMessageAsync(message.Chat, sendMessageMain);
        }
    }

    private static string GetMainText(decimal sum, TransactionDTO[] transactions)
        => $"Ваш баланс: {sum.ToString("#.##")}" + $"\n\nОперации в обработке: \n{GetMessageTextInCollection(transactions.Where(o => o.Status == InternetDatabase.EntityDB.TransactionStatus.Wait).ToArray())}"
        + $"\n\nУспешные операции: \n{GetMessageTextInCollection(transactions.Where(o => o.Status != InternetDatabase.EntityDB.TransactionStatus.Wait).ToArray())}";

    private static string GetMessageTextInCollection(TransactionDTO[] transactions)
        => transactions.Length <= 0 ? "К сожалению пока здесь ничего нет" : string.Join("\n\n", transactions.Select(o => $"<b>Дата:</b> {o.DateTimeUTC:mm:HH dd.MM.yy}\n<b>Сумма:</b> {o.Count.ToString("0.##")}\n<b>Сообщение:</b> {o.Message}"));

    private static List<string> SplitString(string str, int chunkSize)
    {
        List<string> chunks = new List<string>();
        for (int i = 0; i < str.Length; i += chunkSize)
        {
            if (i + chunkSize > str.Length)
                chunkSize = str.Length - i;

            chunks.Add(str.Substring(i, chunkSize));
        }
        return chunks;
    }
}
