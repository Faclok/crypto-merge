using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands.Abstractions;

public interface ITextCommand : ICommand
{
    public string[] MessageKeys { get; }

    public Task Handler(Message message);
}
