using InternetDatabase.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BusLogic.Services
{
    public class EverydaySetCheckHosted(IServiceProvider service) : BackgroundService
    {
        private Timer? _timer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new(new TimerCallback(OnTick), null, (int)(DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow + TimeSpan.FromHours(3) - TimeSpan.FromMinutes(2)).TotalMilliseconds, (int)TimeSpan.FromDays(1).TotalMilliseconds);

            return Task.CompletedTask;
        }

        private async void OnTick(object? sender)
        {
            var context = service.GetRequiredService<InternetDbContext>();

            await context.Wallets
                   .Where(o => o.Status == InternetDatabase.EntityDB.WalletStatus.Connected)
                   .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsCheckYesterday, false)
                   .SetProperty(p=>p.BalanceBoost, 0));
        }
    }
}
