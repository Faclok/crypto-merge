
namespace BusLogic.Middleware
{

    /// <summary>
    /// Интерфейс для получения кода во вход страницы
    /// </summary>
    public interface ITelegramClientLogin
    {

        public bool IsLogin { get; set; }

        public Task SendCode(int code);
        public Task<int> GetCodeAsync();
    }
}
