using crypto_merge.Tg.Bot.Commands.Abstractions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Handlers;

public class CallbackQueryHandler
{
    private readonly Dictionary<string, Type> _callbackQueryHandlerPairs = [];
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public CallbackQueryHandler(IEnumerable<ICallbackCommand> commands, ILogger<CallbackQueryHandler> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        foreach (var command in commands)
            AddCallbackQueryCommand(command);
    }

    public void AddCallbackQueryCommand(ICallbackCommand callbackQueryCommand)
    {
        var type = callbackQueryCommand.GetType();
        _callbackQueryHandlerPairs.Add(callbackQueryCommand.CallbackKey, type);
        if (!_callbackQueryHandlerPairs.ContainsKey(type.Name))
            _callbackQueryHandlerPairs.Add(type.Name, type);
    }

    public Task HandlerAsync(CallbackQuery callbackQuery)
    {
        var args = callbackQuery.Data!.Split(' ');
        if (_callbackQueryHandlerPairs.TryGetValue(args[0], out var type))
        {
            if (_serviceProvider.GetService(type) is ICallbackCommand callbackCommand)
                return callbackCommand.Handler(callbackQuery, args.Skip(1).ToArray());
        }

        _logger.LogCritical($"CallbackQuery: [{args[0]}] not found");
        return Task.CompletedTask;
    }
}