using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands;
using InternetDatabase.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Services
{
    public class MessageSenderFromAdmin(InternetDbContext context, MessageSender sender,WalletService walletService,TransactionService transactionService)
    {

        public async Task SendFromAdmin(int transactionId, string message)
        {
            var chatId = await GetChatIdByTransactionId(transactionId);

            if (chatId == 0)
                return;

            var sendMessage = new SendMessage()
            {
                Text = "<b>Сообщение админа</b>\n" + message,
                KeyboardMarkup = Keyboard.CreateInline(new InlineButton("Связаться с админом", nameof(MessageToAdminPage), transactionId.ToString())),
                ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html,
            };

            await sender.SendMessageAsync(chatId, sendMessage);
        }

        public Task SendFromAdmin(int transactionId, string titleRequest, string request, string titleSum, string sum, string comment)
        {
            return SendFromAdmin(transactionId, titleRequest + ": " + $"<code>{request}</code>" + "\n" + titleSum + ": " + $"<code>{sum}</code>" + "\n" + comment);
        }

        public async Task SendTransactionCompleted(int transactionId)
        {
            var chatId = await GetChatIdByTransactionId(transactionId);

            var sum = await transactionService.GetSumAsync(transactionId);
            var userBalance = await walletService.GetTotalBalanceAsTransactionAsync(transactionId);
            if (chatId == 0)
                return;

            await sender.SendMessageAsync(chatId, $"""
                Ваше пополнение успешно зачислено на баланс
                На ваш счет поступило: {sum:0.##}
                Ваш баланс: {userBalance:0.##}
                """);
        }

        private Task<long> GetChatIdByTransactionId(int transactionId)
        {
            return context.TransactionWallets.Where(o => o.Id == transactionId)
                .Select(o => o.Wallet)
                .Where(o => o != null && o.TelegramUserId != null)
                .Select(o => o!.TelegramUser!.ChatId)
                .FirstAsync();
        }
    }
}
