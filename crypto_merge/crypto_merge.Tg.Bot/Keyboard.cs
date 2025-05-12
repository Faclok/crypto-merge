using Telegram.Bot.Types.ReplyMarkups;

namespace crypto_merge.Tg.Bot;

public static class Keyboard
{
    public static ReplyKeyboardMarkup CreateReply(KeyboardButton[][] keyboardButtons) =>
        new(keyboardButtons) { ResizeKeyboard = true, };

    public static InlineKeyboardMarkup CreateInline(InlineButton[][] values)
    {
        InlineKeyboardButton[][] keyboardButtons = new InlineKeyboardButton[values.Length][];
        for (int i = 0; i < values.Length; i++)
        {
            keyboardButtons[i] = new InlineKeyboardButton[values[i].Length];

            for (int j = 0; j < values[i].Length; j++)
                keyboardButtons[i][j] = InlineKeyboardButton.WithCallbackData(values[i][j].Text, values[i][j].Data);
        }
        return new(keyboardButtons);
    }
    public static InlineKeyboardMarkup? CreateInline(IEnumerable<InlineButton> values) 
        => CreateInline(values.ToArray());
    public static InlineKeyboardMarkup? CreateInline(params InlineButton[] values)
    {
        if (values.Length < 1)
            return null;

        InlineKeyboardButton[][] keyboardButtons = new InlineKeyboardButton[values.Length][];

        for (int i = 0; i < values.Length; i++)
        {
            keyboardButtons[i] = [InlineKeyboardButton.WithCallbackData(values[i].Text, values[i].Data)];
        }

        return new(keyboardButtons);
    }
}
public enum InlineKeyboard
{
    Subscription,
}
