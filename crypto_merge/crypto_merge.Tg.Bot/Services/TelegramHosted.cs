using crypto_merge.Tg.Bot.Handlers;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace crypto_merge.Tg.Bot.Services;

public class TelegramHosted(ITelegramBotClient telegramBot, UpdateHandler updateHandler) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        telegramBot.StartReceiving(updateHandler, cancellationToken: stoppingToken);

        return Task.CompletedTask;
    }
}
