using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands.Abstractions;

public interface IMessageCommand : ICommand
{
    public string Command { get; }
    public Task Handler(Message message, string[] args);
}
