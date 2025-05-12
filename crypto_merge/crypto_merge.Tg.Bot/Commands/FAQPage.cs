using crypto_merge.Tg.Bot.Commands.Abstractions;
using crypto_merge.Tg.Bot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands
{
    public class FAQPage : ITextCommand
    {
        private readonly MessageSender _sender;
        private readonly ILogger _logger;

        public FAQPage(MessageSender sender, ILogger<FAQPage> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public string[] MessageKeys => ["FAQ"];

        public Task Handler(Message message)
        {
            var sendMessage = new SendMessage()
            {
                Text = """
                <b>❓ Часто задаваемые вопросы (FAQ) ❓</b>

                <b>Где я могу найти последние обновления и новости?</b> 
                    📢 Все последние обновления и новости доступны на нашем <a href="https://t.me/+LrrfO5kKiqJlMGYy">канале</a>.

                <b>Как я могу задать вопрос?</b>
                    ✍️Вы можете задать вопрос нашему саппорту.

                <b>Какие темы обсуждаются на канале?</b>  
                    📚 На нашем канале Вы найдете указания для новичков, ОБЯЗАТЕЛЬНЫЕ ДЛЯ ОЗНАКОМЛЕНИЯ <a href="https://t.me/c/2173957641/2">ПРАВИЛА</a>
                """,
                ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html,
                LinkPreviewOptions = true,
            };

            return _sender.SendMessageAsync(message.Chat.Id, sendMessage);
        }
    }
}
