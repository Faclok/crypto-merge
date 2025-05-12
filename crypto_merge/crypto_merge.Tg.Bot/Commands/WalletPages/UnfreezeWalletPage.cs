using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands.Abstractions;
using crypto_merge.Tg.Bot.Services;
using System.Reflection;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands.WalletPages;

public class UnfreezeWalletPage(MessageSender sender, WalletService walletService) : ICallbackCommand
{
    public string CallbackKey => nameof(UnfreezeWalletPage);

    public Task Handler(CallbackQuery callbackQuery, string[] args)
    {
        if (callbackQuery.Message is not { } message)
            return Task.CompletedTask;

        if (args.Length <= 0)
            return ChoosingWallet(message.Chat.Id, message.MessageId);

        return args[0] switch
        {
            "unfreeze" => UnfreezeWallet(message.Chat.Id, message.MessageId, int.Parse(args[1])),
            _ => ChoosingWallet(message.Chat.Id, message.MessageId),
        };
    }

    private async Task UnfreezeWallet(long chatId, int messageId, int bankId)
    {
        if (await walletService.GetById(bankId) is not { } wallet)
        {
            await sender.EditMessageAsync(chatId, messageId, "Банк отсутствует");
            return;
        }

        await walletService.UpdateStatusAsync(bankId, InternetDatabase.EntityDB.WalletStatus.Connection);

        var sendMessage = new SendMessage()
        {
            Text = $"{wallet.Bank} подключается",
        };

        await sender.EditMessageAsync(chatId, messageId, sendMessage);
    }

    private async Task ChoosingWallet(long chatId, int messageId)
    {
        var wallets = walletService.GetByChatId(chatId,InternetDatabase.EntityDB.WalletStatus.Stop);
        var walletsBalance = wallets.ToDictionary(o => o.Id, o => 0m);

        foreach (var balance in walletsBalance)
        {
            if (balance.Key == 0)
                continue;

            walletsBalance[balance.Key] = await walletService.GetTotalBalanceAndTempAsync(balance.Key);
        }

        var sendMessage = new SendMessage()
        {
            Text = "Выберите какой банк хотите восстановить",
            KeyboardMarkup = Keyboard.CreateInline(wallets.Select(w => new InlineButton($"{w.Bank}", nameof(UnfreezeWalletPage), "unfreeze", w.Id.ToString()))),
        };

        await sender.EditMessageAsync(chatId, messageId, sendMessage);
    }

}