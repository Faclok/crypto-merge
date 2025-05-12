using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands.Abstractions;

public interface ICallbackCommand : ICommand
{
    public Task Handler(CallbackQuery callbackQuery, string[] args);

    public string CallbackKey { get; }
}
