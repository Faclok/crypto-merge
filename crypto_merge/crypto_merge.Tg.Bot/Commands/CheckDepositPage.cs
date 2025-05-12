using crypto_merge.Tg.Bot.Commands.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands
{
    public class CheckDepositPage(ITelegramBotClient telegramBot) : ICallbackCommand
    {
        public const string CALLBACK_KEY = nameof(CheckDepositPage);

        public string CallbackKey => CALLBACK_KEY;

        public Task Handler(CallbackQuery callbackQuery, string[] args)
        {
            if (callbackQuery.Message == null)
                return Task.CompletedTask;

            return telegramBot.SendTextMessageAsync(callbackQuery.Message.Chat, "Автоматическая проверка пополнения на криптокошелек и зачисление средств по формуле ХХХ(Выведу ее отдельно)\r\nИЛИ\r\nЕсли автоматическая проверка затягивает время реализации проекта, сделать систему, по которой пользователь после нажатия кнопки “Проверить пополнение” должен будет прислать ссылку на транзакцию в https://tronscan.org/#/\r\nМы в ответ должны получить уведомление о депозите от пользователя и либо подтвердить его, либо отклонить.\r\n”В дополнение сделать возможность прямой связи с клиентом(На случай каких либо вопросов с нашей сторону)”, ");
        }
    }
}
