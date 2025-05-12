using BusLogic.Middleware;
using BusLogic.Services;
using crypto_merge.Tg.Bot;
using crypto_merge.Tg.Bot.Services;
using InternetDatabase.Services;
using System.Globalization;
using T1 = telegram_client.Services;
using T2 = telegram_client_two.Services;
using TG = Telegram.Bot;

namespace crypto_merge
{
    public class Program
    {
        private const string CORS_POLICY = "CorsPolicy";
        public static Task Main(string[] args)
        {
            CultureInfo.CurrentCulture = new("ru-RU");
            var builder = WebApplication.CreateBuilder(args);

            // Telegram
            builder.Services.AddHostedService<TelegramHosted>();
            builder.Services.ConfigurationTelegramBot();
            builder.Services.AddSingleton<TG.ITelegramBotClient, TelegramBotClient>();

            // BuLogic
            builder.Services.AddTransient<UserService>();
            builder.Services.AddTransient<WalletService>();
            builder.Services.AddTransient<DepositService>();
            builder.Services.AddTransient<MessageService>();
            builder.Services.AddTransient<TransactionService>();
            builder.Services.AddTransient<CurrencyService>();
            builder.Services.AddTransient<AccountCryptoCardService>();

            builder.Services.AddHostedService<EverydaySetCheckHosted>();
            builder.Services.AddSingleton<TelegramClientLogin>();
            builder.Services.AddSingleton<TelegramClientLoginTwo>();

            //// Telegram Client
            builder.Services.AddSingleton(TelegramClient);
            builder.Services.AddHostedService<T1.TelegramClientHosted>();

            //// Telegram Client Two
            builder.Services.AddSingleton(TelegramClientTwo);
            builder.Services.AddHostedService<T2.TelegramClientHosted>();

            // Setting CORS Policy
            builder.Services.AddCors(option => option.AddPolicy(CORS_POLICY, policy => { policy.WithOrigins(builder.Configuration["CORS_URL"]!); }));

            //ASP.NET
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Database
            builder.Services.AddEntityFrameworkMySql();
            builder.Services.AddDbContext<InternetDbContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);
            builder.Services.AddHostedService<CreateDefaultValues>();

            //Build project DI
            var app = builder.Build();


            app.UseCors(CORS_POLICY);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            
            return app.RunAsync();
        }

        // Create telegram user client
        private static T1.TelegramClient TelegramClient(IServiceProvider service)
        {
            var configuration = service.GetRequiredService<IConfiguration>();
            var loginService = service.GetRequiredService<TelegramClientLogin>();

            var apiId = configuration["telegram_user_api_id"];
            var apiHash = configuration["telegram_user_api_hash"];

            if (string.IsNullOrWhiteSpace(apiId) || string.IsNullOrWhiteSpace(apiHash))
                throw new ArgumentNullException(nameof(configuration));

            var client = new T1.TelegramClient(configuration, loginService, int.Parse(apiId), apiHash, "main_bot"); // this constructor doesn't need a Config method

            return client;
        }

        // Create telegram user client
        private static T2.TelegramClient TelegramClientTwo(IServiceProvider service)
        {
            var configuration = service.GetRequiredService<IConfiguration>();
            var loginService = service.GetRequiredService<TelegramClientLoginTwo>();

            var apiId = configuration["telegram_user_api_id2"];
            var apiHash = configuration["telegram_user_api_hash2"];

            if (string.IsNullOrWhiteSpace(apiId) || string.IsNullOrWhiteSpace(apiHash))
                throw new ArgumentNullException(nameof(configuration));

            var client = new T2.TelegramClient(configuration, loginService, int.Parse(apiId), apiHash, "two_bot"); // this constructor doesn't need a Config method

            return client;
        }
    }
}
