using System.Threading.Channels;

namespace BusLogic.Middleware
{
    public class TelegramClientLogin : ITelegramClientLogin
    {

        public bool IsLogin { get; set; } = false;

        private readonly Channel<int> _channel = Channel.CreateBounded<int>(
                new BoundedChannelOptions(1)
                {
                    SingleWriter = true,
                    SingleReader = false,
                    AllowSynchronousContinuations = false,
                    FullMode = BoundedChannelFullMode.DropWrite
                });

        public async Task SendCode(int code)
        {
            await _channel.Writer.WriteAsync(item: code);
            _channel.Writer.Complete();
        }

        public async Task<int> GetCodeAsync()
        {
            return await _channel.Reader.ReadAsync();
        }
    }


    public class TelegramClientLoginTwo : ITelegramClientLogin
    {

        public bool IsLogin { get; set; } = false;

        private readonly Channel<int> _channel = Channel.CreateBounded<int>(
                new BoundedChannelOptions(1)
                {
                    SingleWriter = true,
                    SingleReader = false,
                    AllowSynchronousContinuations = false,
                    FullMode = BoundedChannelFullMode.DropWrite
                });

        public async Task SendCode(int code)
        {
            await _channel.Writer.WriteAsync(item: code);
            _channel.Writer.Complete();
        }

        public async Task<int> GetCodeAsync()
        {
            return await _channel.Reader.ReadAsync();
        }
    }
}
