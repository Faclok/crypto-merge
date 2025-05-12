using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands.Abstractions;
using crypto_merge.Tg.Bot.Services;
using InternetDatabase.EntityDB;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands.WalletPages;

public class FreezeWalletPage(MessageSender sender, WalletService walletService) : ICallbackCommand
{
    public const string CALLBACK_KEY = nameof(FreezeWalletPage);

    public string CallbackKey => CALLBACK_KEY;

    public Task Handler(CallbackQuery callbackQuery, string[] args)
    {

        if (callbackQuery.Message is not { } message)
            return Task.CompletedTask;

        if (args.Length <= 0)
            return ChoosingBank(message.Chat.Id, message.MessageId);

        return args[0] switch
        {
            "stop" => StoppingBank(message.Chat.Id, message.MessageId, int.Parse(args[1])),
            "freeze" => Freeze(message.Chat.Id, message.MessageId, int.Parse(args[1])),
            _ => ChoosingBank(message.Chat.Id, message.MessageId),
        };
    }

    private async Task Freeze(long chatId, int messageId, int bankId)
    {
        await walletService.UpdateStatusAsync(bankId, WalletStatus.Stopping);
        await sender.EditMessageAsync(chatId, messageId, "Банк вскоре будет отключен");
    }

    private async Task StoppingBank(long chatId, int messageId, int bankId)
    {

        var wallet = await walletService.GetById(bankId);

        if (wallet is null)
        {
            await sender.EditMessageAsync(chatId, messageId, "Банк отсутствует");
            return;
        }

        await walletService.UpdateStatusAsync(bankId, WalletStatus.Stopping);
        await sender.EditMessageAsync(chatId, messageId, "Банк вскоре будет отключен");
    }

    private async Task ChoosingBank(long chatId, int messageId)
    {
        var wallets = walletService.GetByChatId(chatId);
        var walletsBalance = wallets.ToDictionary(o => o.Id, o => 0m);

        foreach (var balance in walletsBalance)
        {
            if (balance.Key == 0)
                continue;

            walletsBalance[balance.Key] = await walletService.GetTotalBalanceAndTempAsync(balance.Key);
        }

        var sendMessage = new SendMessage()
        {
            Text = "Выберите какой банк хотите поставить на паузу",
            KeyboardMarkup = Keyboard.CreateInline(wallets.Select(w => new InlineButton($"{w.Bank} ({walletsBalance[w.Id].ToString("0.##")}₽)", nameof(FreezeWalletPage), "stop", w.Id.ToString())).ToArray()),
        };
        await sender.EditMessageAsync(chatId, messageId, sendMessage);
    }
}
