using Microsoft.Extensions.Configuration;

namespace crypto_merge.Tg.Bot.Configuration
{
    public class SecretConfig(IConfiguration configuration)
    {
        public readonly BotConfiguration Configuration = configuration.GetSection("BotConfiguration").Get<BotConfiguration>()!;
    }
}
