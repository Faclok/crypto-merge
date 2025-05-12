using crypto_merge.Tg.Bot.Commands.Abstractions;
using InternetDatabase.EntityDB;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands.WalletPages;

public class MainWalletPage(ITelegramBotClient telegramBot) : ITextCommand
{
    public string[] MessageKeys => ["Управление банками"];

    public Task Handler(Message message)
    {
        return telegramBot.SendTextMessageAsync(message.Chat, @"Привяжите и авторизуйте банк(на данный момент только СБЕР)

📌 Перед этим изучите <a href=""https://telegra.ph/Instrukciya-po-nastrojke-vhoda-v-Sber-08-31"">инструкцию</a> по настройке входа

✅ Для авторизации Вам понадобятся: номер телефона, логин, пароль, ФИО, номер карты 

⏸️ Используйте паузу, чтобы на банк не поступали средства",
            replyMarkup: Keyboard.CreateInline([
                [new("Добавить банк", nameof(AddWalletPage))],
                [new("Постановка на паузу банка", nameof(FreezeWalletPage))],
                [new("Снять банк с паузы", nameof(UnfreezeWalletPage))],
                [new("Удалить банк", nameof(DeleteWalletPage))],
            ]), 
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
    }
}
