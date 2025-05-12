using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands;

public class SelfReferralPage(UserService userService, ITelegramBotClient telegramBot) : ITextCommand
{
    public string[] MessageKeys => ["Реферальная программа"];

    public async Task Handler(Message message)
    {
        var botUsername = await telegramBot.GetMeAsync();
        var selfReferralLink = await userService.GetReferralLinkSelf(botUsername.Username!, message.Chat.Id);

        await telegramBot.SendTextMessageAsync(message.Chat, "💰Получайте 0,5% от каждого внесенного депозита своих рефералов\r\n\r\nРеферальный процент зачисляется, когда вы пополняете баланс \nРеферальная ссылка: "+ selfReferralLink);
    }
}
