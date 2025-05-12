using crypto_merge.Tg.Bot.Commands.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands
{
    public class SelfCryptoWalletPage(ITelegramBotClient telegramBot) : ICallbackCommand
    {
        public const string CALLBACK_KEY = nameof(SelfCryptoWalletPage);

        public string CallbackKey => CALLBACK_KEY;

        public Task Handler(CallbackQuery callbackQuery, string[] args)
        {
            if (callbackQuery.Message == null)
                return Task.CompletedTask;

            return telegramBot.SendTextMessageAsync(callbackQuery.Message.Chat, "Вывод клиенту инструкции+вывод сообщения о необходимости поочередно ввести нужные данные");
        }
    }
}
