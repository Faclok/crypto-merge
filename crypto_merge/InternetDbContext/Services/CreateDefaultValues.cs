using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace InternetDatabase.Services
{
    public class CreateDefaultValues(InternetDbContext context) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!await context.Banks.AnyAsync())
                await context.Banks.AddAsync(new()
                {
                    Name = "Crypto merge",
                });

            await context.SaveChangesAsync();
        }
    }
}
