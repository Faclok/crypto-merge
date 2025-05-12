using crypto_merge.Tg.Bot.Commands.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace crypto_merge.Tg.Bot.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCommand<T>(this IServiceCollection services) where T : class, ICommand
    {
        if (typeof(T).GetInterfaces().Contains(typeof(IMessageCommand)))
        {
            services.AddTransient(typeof(IMessageCommand), typeof(T));
        }
        if (typeof(T).GetInterfaces().Contains(typeof(ICallbackCommand)))
        {
            services.AddTransient(typeof(ICallbackCommand), typeof(T));
        }
        if (typeof(T).GetInterfaces().Contains(typeof(ITextCommand)))
        {
            services.AddTransient(typeof(ITextCommand), typeof(T));
        }
        services.AddTransient<T>();
        //services.AddTransient<ICommand,T>();
        return services;
    }
}
