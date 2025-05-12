using crypto_merge.Tg.Bot.Configuration;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using TG = Telegram.Bot;

namespace crypto_merge.Tg.Bot.Services;

public class TelegramBotClient : ITelegramBotClient
{
    public TG.TelegramBotClient TelegramBot { get; private set; }

    public bool LocalBotServer => TelegramBot.LocalBotServer;

    public long BotId => TelegramBot.BotId;

    public TimeSpan Timeout { get => TelegramBot.Timeout; set => TelegramBot.Timeout = value; }
    public IExceptionParser ExceptionsParser { get => TelegramBot.ExceptionsParser; set => TelegramBot.ExceptionsParser = value; }

    public TelegramBotClient(SecretConfig secret)
    {
        if (secret.Configuration.BotToken is not { } telegramAPI)
            throw new ArgumentException(nameof(telegramAPI));

        TelegramBot = new TG.TelegramBotClient(telegramAPI);
        TelegramBot.OnApiResponseReceived += OnApiResponseReceived;
        TelegramBot.OnMakingApiRequest += OnMakingApiRequest;

        //if (!string.IsNullOrWhiteSpace(botConfiguration.BotWebhookUrl))
        //{
        //    TelegramBot.SetWebhookAsync(
        //        url:botConfiguration.BotWebhookUrl,
        //        secretToken: botConfiguration.SecretToken);
        //}

    }

    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;

    public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        => TelegramBot.MakeRequestAsync(request, cancellationToken);

    public Task<bool> TestApiAsync(CancellationToken cancellationToken = default)
        => TelegramBot.TestApiAsync(cancellationToken);

    public Task DownloadFileAsync(string filePath, Stream destination, CancellationToken cancellationToken = default)
        => TelegramBot.DownloadFileAsync(filePath, destination, cancellationToken);
}
