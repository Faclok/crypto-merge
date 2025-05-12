using crypto_merge.Tg.Bot.Commands;
using crypto_merge.Tg.Bot.Commands.WalletPages;
using crypto_merge.Tg.Bot.Configuration;
using crypto_merge.Tg.Bot.Extensions;
using crypto_merge.Tg.Bot.Handlers;
using crypto_merge.Tg.Bot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace crypto_merge.Tg.Bot;

public static class ConfigurationExtensions
{
    public static void ConfigurationTelegramBot(this IServiceCollection services)
    {
        services.AddSingleton<UpdateHandler>();
        services.AddSingleton<MessageHandler>();
        services.AddTransient<CallbackQueryHandler>();
        services.AddTransient<WaitForUserResponse>();
        services.AddTransient<MessageSender>();

        services.AddCommand<MainPage>();
        services.AddCommand<MessageToAdminPage>();
        services.AddCommand<FAQPage>();
        services.AddCommand<UserStatisticsPage>();
        services.AddCommand<SelfReferralPage>();
        services.AddCommand<BalancePage>();
        services.AddCommand<DepositPage>();
        services.AddCommand<SelfCryptoWalletPage>();
        services.AddCommand<CheckDepositPage>();

        //WalletPage
        services.AddCommand<AddWalletPage>();
        services.AddCommand<CollectionBankToMePage>();
        services.AddCommand<UnfreezeWalletPage>();
        services.AddCommand<DeleteWalletPage>();
        services.AddCommand<FreezeWalletPage>();
        services.AddCommand<MainWalletPage>();

        services.AddSingleton<SecretConfig>();
        services.AddTransient<MessageSenderFromAdmin>();
    }
}
