using BusLogic.Services;
using crypto_merge.Tg.Bot.Commands.Abstractions;
using crypto_merge.Tg.Bot.Services;
using InternetDatabase.EntityDB;
using System.Reflection;
using Telegram.Bot.Types;

namespace crypto_merge.Tg.Bot.Commands.WalletPages;

public class DeleteWalletPage(MessageSender sender, WalletService walletService) : ICallbackCommand
{
    public string CallbackKey => nameof(DeleteWalletPage);

    public Task Handler(CallbackQuery callbackQuery, string[] args)
    {
        if (callbackQuery.Message is not { } message)
            return Task.CompletedTask;

        if (args.Length <= 0)
            return ChoosingWallet(message.Chat.Id, message.MessageId);

        return args[0] switch
        {
            "isDel" => IsDelWallet(message.Chat.Id, message.MessageId, int.Parse(args[1])),
            "del" => DelWallet(message.Chat.Id, message.MessageId, int.Parse(args[1])),
            _ => ChoosingWallet(message.Chat.Id, message.MessageId),
        };
    }

    private async Task DelWallet(long chatId, int messageId, int walletId)
    {
        if(await walletService.GetCountTransactionOnWaiting(walletId) > 0)
        {
            await sender.SendMessageAsync(chatId, "К сожалению у вас есть не завершенные операции");
            return;
        }


        if(await walletService.SoftDelete(walletId) <= 0)
        {
            await sender.SendMessageAsync(chatId, "К сожалению банка не существует");
            return;
        }

        var sendMessage = new SendMessage()
        {
            Text = $"Банк удален",
        };

        await sender.EditMessageAsync(chatId, messageId, sendMessage);
    }
    private async Task IsDelWallet(long chatId, int messageId, int bankId)
    {
        var wallet = await walletService.GetById(bankId);

        if (wallet is null)
        {
            await sender.EditMessageAsync(chatId, messageId, "Банк отсутствует");
            return;
        }

        var sendMessage = new SendMessage()
        {
            Text = $"Вы точно хотите удалить {wallet.Bank}?",
            KeyboardMarkup = Keyboard.CreateInline([
                [new("Да", nameof(DeleteWalletPage), "del", wallet.Id.ToString()),new("Нет",nameof(DeleteWalletPage))],
                ]),
        };

        await sender.EditMessageAsync(chatId, messageId, sendMessage);
    }

    private async Task ChoosingWallet(long chatId, int messageId)
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
            Text = "Выберите какой банк хотите удалить",
            KeyboardMarkup = Keyboard.CreateInline(wallets.Select(w => new InlineButton($"{w.Bank} ({walletsBalance[w.Id].ToString("0.##")}₽)", nameof(DeleteWalletPage), "isDel", w.Id.ToString()))),
        };

        await sender.EditMessageAsync(chatId, messageId, sendMessage);
    }
}