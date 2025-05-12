
using crypto_merge.Tg.Bot.Commands.Abstractions;

namespace crypto_merge.Tg.Bot;

public class InlineButton
{
    public string Text { get; set; }
    public string Data { get; set; }

    public InlineButton(string text, params string[] data)
    {
        Text = text;
        Data = string.Join(" ", data);
    }

    public InlineButton(Type type, string text, params string[] args)
    {
        Data = type.Name + " " + string.Join(" ", args);
        Text = text;
    }

    public InlineButton(Type type, string text) : this(text, type.Name)
    {
    }

    public static InlineButton Create<T>(string Data) where T : class, ICallbackCommand
    {
        return new InlineButton(Data, "");
    }
}
