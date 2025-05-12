namespace crypto_merge.Tg.Bot;

public class BotConfiguration
{
    public string BotToken { get; init; } = default!;
    public string? BotWebhookUrl { get; init; } = null;
    public string? SecretToken { get; init; } = null;
}
