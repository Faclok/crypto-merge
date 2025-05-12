using System.Collections.Concurrent;
using System.Threading.Channels;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Services;

public class WaitForUserResponse
{
    private static ConcurrentDictionary<ChatId, ChannelWriter<Message?>> keyValuePairs = new();
    public async Task<Message?> GetMessageAsync(long ChatId)
    {
        var channel = Channel.CreateBounded<Message?>(new BoundedChannelOptions(1) { SingleReader = true, SingleWriter = true });
        if (!keyValuePairs.TryAdd(ChatId, channel))
            keyValuePairs[ChatId] = channel;

        ChannelReader<Message?> reader = channel;
        //читать данные из канала по мере их доступности
        var message = await reader.ReadAsync();
        keyValuePairs.Remove(ChatId, out _);
        return message;

    }

    private async Task<Message?> GetMessageAsync(long chatId, Func<Message, bool> func)
    {
        bool isExit = false;
        while (true)
        {
            var message = await GetMessageAsync(chatId);
            if (message is null)
                return null;

            isExit = func(message);

            if (isExit)
                return message;
        }
    }

    public async Task SetMessageAsync(long ChatId, Message message)
    {
        if (!keyValuePairs.TryGetValue(ChatId, out var keyValue))
            return;

        await keyValue.WriteAsync(message);
        keyValue.Complete();

        return;
    }

    public async Task<bool> TrySetMessageAsync(long ChatId, Message message)
    {
        if (!keyValuePairs.TryGetValue(ChatId, out var keyValue))
            return false;

        await keyValue.WriteAsync(message);
        keyValue.Complete();

        return true;
    }

    public async Task CloseMessageAsync(long ChatId)
    {
        if (keyValuePairs.TryGetValue(ChatId, out var keyValue))
        {
            keyValuePairs.Remove(ChatId, out _);
            await keyValue.WriteAsync(null);
            keyValue.Complete();
        }
    }
}
