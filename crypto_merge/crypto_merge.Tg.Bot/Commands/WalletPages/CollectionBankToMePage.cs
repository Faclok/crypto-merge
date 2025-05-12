using crypto_merge.Tg.Bot.Commands.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands.WalletPages;

public class CollectionBankToMePage(ITelegramBotClient telegramBot) : ICallbackCommand
{
    public const string CALL_BACK = nameof(CollectionBankToMePage);

    public string CallbackKey => CALL_BACK;

    public Task Handler(CallbackQuery callbackQuery, string[] args)
    {
        if (callbackQuery.Message == null)
            return Task.CompletedTask;
        return telegramBot.SendTextMessageAsync(callbackQuery.Message.Chat, "“Список банков.Статистика по каждому банку”");
    }
}
