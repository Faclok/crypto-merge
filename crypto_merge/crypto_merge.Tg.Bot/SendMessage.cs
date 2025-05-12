using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace crypto_merge.Tg.Bot;

public class SendMessage()
{
    public required string Text { get; set; }
    public ParseMode ParseMode { get; set; } = ParseMode.None;
    public LinkPreviewOptions? LinkPreviewOptions { get; set; }
    public IEnumerable<MessageEntity>? MessageEntities { get; set; }
    public IReplyMarkup? KeyboardMarkup { get; set; }
}
