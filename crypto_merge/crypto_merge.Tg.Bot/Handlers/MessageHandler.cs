using crypto_merge.Tg.Bot.Commands.Abstractions;
using crypto_merge.Tg.Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Handlers;

public class MessageHandler
{
    private readonly Dictionary<string, Type> _commandHandlerPairs = new();
    private readonly Dictionary<string, Type> _textHandlerPairs = new();
    private readonly WaitForUserResponse _waitForUserResponse;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger _logger;

    public MessageHandler(ITelegramBotClient botClient, ILogger<MessageHandler> logger, WaitForUserResponse waitForUserResponse, IEnumerable<IMessageCommand> messageCommands, IEnumerable<ITextCommand> textCommands, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _botClient = botClient;
        _waitForUserResponse = waitForUserResponse;
        _serviceScopeFactory = serviceScopeFactory;

        foreach (var command in messageCommands)
            AddMessageCommand(command);

        foreach (var command in textCommands)
            AddTextCommand(command);
    }

    public void AddMessageCommand(IMessageCommand command)
        => _commandHandlerPairs.Add(command.Command.ToLower(), command.GetType());

    public void AddTextCommand(ITextCommand command)
    {
        foreach (var item in command.MessageKeys)
        {
            _textHandlerPairs.Add(item.ToLower(), command.GetType());
        }
    }

    public async Task HandlerAsync(Message message)
    {
        using var serviceScope = _serviceScopeFactory.CreateScope();
        var service = serviceScope.ServiceProvider;

        if (!(message.Text is { } messageText))
        {
            await DefaultHandler(message.Chat.Id, message);
            return;
        }

        if (messageText[0] == '/')
        {
            var args = messageText.Split(' ');

            if (_commandHandlerPairs.TryGetValue(args[0].ToLower(), out var type))
            {
                if (service.GetRequiredService(type) is IMessageCommand command)
                {
                    await _waitForUserResponse.CloseMessageAsync(message.Chat.Id);
                    await command.Handler(message, args.Skip(1).ToArray());
                    return;
                }

            }
        }
        else
        {
            if (_textHandlerPairs.TryGetValue(messageText.ToLower(), out var type))
            {
                if (service.GetRequiredService(type) is ITextCommand command)
                {
                    await _waitForUserResponse.CloseMessageAsync(message.Chat.Id);
                    await command.Handler(message);
                    return;
                }
            }
        }

        await DefaultHandler(message.Chat.Id, message);
    }


    private async Task DefaultHandler(long chatId, Message message)
    {
        if (await _waitForUserResponse.TrySetMessageAsync(chatId, message))
            return;
        await _botClient.CopyMessageAsync(message.Chat.Id, message.Chat.Id, message.MessageId);
    }

}
