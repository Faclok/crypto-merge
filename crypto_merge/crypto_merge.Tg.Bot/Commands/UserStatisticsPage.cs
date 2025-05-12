using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands.Abstractions;
using InternetDatabase.EntityDTO;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands
{
    public class UserStatisticsPage(ITelegramBotClient client, WalletService walletService) : ITextCommand, ICallbackCommand
    {

        public string[] MessageKeys => ["Статистика"];

        public const string CALLBACK_KEY = nameof(UserStatisticsPage);
        public string CallbackKey => CALLBACK_KEY;

        public const string ONE_DAY = "one_day";
        public const string THREE_DAY = "three_day";

        public async Task Handler(Message message)
        {
            var sum = await walletService.GetSumMoneyInReferralAndTempAsync(message.Chat.Id);

            var transactions = await walletService.GetTransactionWalletReferralYesterdayAsync(message.Chat.Id);

            await client.SendTextMessageAsync(message.Chat.Id,
                text: $"За все время статистика: {sum:0.##}\n\r\nВаша статистика за 1 день: {transactions.Sum(o => o.Count):0.##}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: Keyboard.CreateInline([[new("За 3 дня", CallbackKey + " " + THREE_DAY)]]));
        }

        public async Task Handler(CallbackQuery callbackQuery, string[] args)
        {

            if (callbackQuery.Message is not { } message)
                return;

            var sum = await walletService.GetSumMoneyInReferralAndTempAsync(message.Chat.Id);
            var transactions = Array.Empty<TransactionDTO>();

            switch (args[0])
            {
                case ONE_DAY:

                    transactions = await walletService.GetTransactionWalletReferralYesterdayAsync(message.Chat.Id);

                    await client.EditMessageTextAsync(message.Chat,
                        message.MessageId,
                        text: $"За все время статистика: {sum}\n\r\nВаша статистика за 1 день:\n\n{GetMessageTextInCollection(transactions)}",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyMarkup: Keyboard.CreateInline([[new("За 3 дня", CallbackKey + " " + THREE_DAY)]]));

                    break;

                case THREE_DAY:

                    transactions = await walletService.GetTransactionWalletReferralThreeDayBackAsync(message.Chat.Id);

                    await client.EditMessageTextAsync(message.Chat,
                        message.MessageId,
                        text: $"За все время статистика: {sum}\n\r\nВаша статистика за 3 дня:\n\n{GetMessageTextInCollection(transactions)}",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyMarkup: Keyboard.CreateInline([[new("За 1 день", CallbackKey + " " + ONE_DAY)]]));

                    break;
                default:

                    await client.SendTextMessageAsync(message.Chat, "Что-то пошло не по плану");

                    break;
            }
        }

        private static string GetMessageTextInCollection(TransactionDTO[] transactions)
            => transactions.Length <= 0 ? "К сожалению пока здесь ничего нет" : $"{transactions.Sum(o => o.Count):0.##}";
    }
}
