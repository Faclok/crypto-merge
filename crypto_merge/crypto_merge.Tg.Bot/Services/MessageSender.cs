using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace crypto_merge.Tg.Bot.Services;

public class MessageSender(ITelegramBotClient botClient, ILogger<MessageSender> logger)
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly ILogger _logger = logger;

    private async Task<Message> TryExecuteAsync(Func<Task<Message>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            _logger.LogError("SenderMessage Exception: {exMessage}\n{exStackTrace}\nError {@ex}", ex.Message,ex.StackTrace,ex);
            return new Message();
        }
    }

    private async Task TryExecuteAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            _logger.LogError("SenderMessage Exception: {exMessage}\n{exStackTrace}\nError {@ex}", ex.Message, ex.StackTrace, ex);
        }
    }

    public Task<Message> SendMessageAsync(ChatId chatId, SendMessage sendMessage)
    {
        _logger.LogDebug("Chat id:{chatId}\nMessage {sendMessage}", chatId, sendMessage);

        return TryExecuteAsync(()=>_botClient.SendTextMessageAsync(
            chatId: chatId,
            text: sendMessage.Text,
            replyMarkup: sendMessage.KeyboardMarkup,
            parseMode: sendMessage.ParseMode));
    }

    public Task<Message> SendMessageAsync(ChatId chatId, string text, ParseMode parseMode = ParseMode.None)
    {
        _logger.LogDebug("Chat id:{chatId}\nMessage {text}", chatId, text);
        return TryExecuteAsync(() => _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            parseMode: parseMode));
    }

    public Task<Message> EditMessageAsync(ChatId chatId, int messageId, SendMessage sendMessage)
    {
        _logger.LogDebug("Chat id:{chatId}\nMessage id {messageId}\nMessage {sendMessage}", chatId, messageId, sendMessage);
        if (sendMessage.KeyboardMarkup is InlineKeyboardMarkup keyboardMarkup)
            return TryExecuteAsync(() => _botClient.EditMessageTextAsync(
                messageId: messageId,
                chatId: chatId,
                text: sendMessage.Text,
                replyMarkup: keyboardMarkup,
                parseMode: sendMessage.ParseMode));
        else
            _logger.LogWarning("Edit message only InlineKeyboardMarkup");
        return TryExecuteAsync(() => _botClient.EditMessageTextAsync(
                messageId: messageId,
                chatId: chatId,
                text: sendMessage.Text,
                parseMode: sendMessage.ParseMode));

    }

    public Task<Message> EditMessageAsync(ChatId chatId, int messageId, string text)
    {
        _logger.LogDebug("Chat id:{chatId} Message id {messageId} New text {text}", chatId, messageId, text);
        return TryExecuteAsync(() => _botClient.EditMessageTextAsync(
                        messageId: messageId,
                        chatId: chatId,
                        text: text
                        ));
    }

    public Task<Message> EditReplyMarkupAsync(ChatId chatId, int messageId, InlineKeyboardMarkup? keyboardMarkup = null)
        => TryExecuteAsync(() => _botClient.EditMessageReplyMarkupAsync(chatId, messageId, keyboardMarkup));

    public Task DeleteMessageAsync(ChatId chatId, int messageId)
         => TryExecuteAsync(() => _botClient.DeleteMessageAsync(chatId, messageId));
}
