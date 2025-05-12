using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Handlers;

public class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> _logger;
    private readonly MessageHandler _messageHandler;
    private readonly CallbackQueryHandler _callbackQueryHandler;

    public UpdateHandler(ILogger<UpdateHandler> logger, MessageHandler messageHandler, CallbackQueryHandler callbackQueryHandler)
    {
        _logger = logger;
        _messageHandler = messageHandler;
        _callbackQueryHandler = callbackQueryHandler;
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource handleErrorSource, CancellationToken cancellationToken)
    {
        _logger.LogError("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return (update switch
        {
            { Message: { } message } => OnMessage(message),
            { EditedMessage: { } message } => OnMessage(message),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
            //{ InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
            //{ ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
            //{ Poll: { } poll } => OnPoll(poll),
            //{ PollAnswer: { } pollAnswer } => OnPollAnswer(pollAnswer),
            // ChannelPost:
            // EditedChannelPost:
            // ShippingQuery:
            // PreCheckoutQuery:
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    private Task OnMessage(Message msg)
    {
        _logger.LogDebug("Receive message type: {MessageType} from chat id: {MessageChatId} Username: {MessageFromUsername}",
            msg.Type, msg.Chat.Id, msg.From?.Username);
        FireAndForget(_messageHandler.HandlerAsync(msg));

        return Task.CompletedTask;
    }

    // Process Inline Keyboard callback data
    private Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        _logger.LogDebug("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        FireAndForget( _callbackQueryHandler.HandlerAsync(callbackQuery));

        return Task.CompletedTask;
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    internal async void FireAndForget(Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex.Message,ex);
            throw;
        }
    }
}
