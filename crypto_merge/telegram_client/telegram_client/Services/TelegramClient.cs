using BusLogic.Middleware;
using Microsoft.Extensions.Configuration;
using WTelegram;

namespace telegram_client.Services;

public class TelegramClient(IConfiguration configuration, TelegramClientLogin loginService, int apiId, string apiHash, string sessionName) : Client(apiId, apiHash, sessionName)
{
    public async Task<bool> LoginAsync()
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var phoneNumber = configuration["telegram_user_phone_number"];

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentNullException(nameof(configuration));

        await DoLogin(phoneNumber);

        async Task DoLogin(string? loginInfo) // (add this method to your code)
        {
            while (this.User == null)
                loginInfo = await this.Login(loginInfo) switch // returns which config is needed to continue login
                {
                    "verification_code" => (await loginService.GetCodeAsync()).ToString(),
                    "name" => "Деньги правят миром 💵",
                    _ => null,
                };
            Console.WriteLine($"We are logged-in as {this.User} (id {this.User.id})");
        }

        loginService.IsLogin = true;
        return true;
    }
}
