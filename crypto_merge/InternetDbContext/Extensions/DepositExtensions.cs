using InternetDatabase.EntityDB;
using InternetDatabase.EntityDTO.Deposit;
using Microsoft.EntityFrameworkCore;

namespace InternetDatabase.Extensions
{
    public static class DepositExtensions
    {

        public static async Task<DepositDTO[]> GetDepositDTOsAsync(this IQueryable<TransactionWallet> transactionWallets, Func<int, Task<decimal>> getBalance)
        {
            var data = await transactionWallets
            .Where(o => o.WalletId != null)
            .AsSplitQuery()
            .Include(o => o.Chat)
            .ThenInclude(o=>o.File)
            .Include(o => o.Wallet)
            .ThenInclude(o => o.TelegramUser)
            .ToArrayAsync();

            var balances = data.DistinctBy(o => o.WalletId).ToDictionary(o => o.WalletId ?? 0, o => 0m);

            foreach (var balance in balances)
            {
                if (balance.Key == 0)
                    continue;

                balances[balance.Key] = await getBalance(balance.Key);
            }

            return data.Select(o => new DepositDTO()
            {
                TransactionId = o.Id,
                CryptoCardId = o.Wallet?.CryptoCardId,
                Username = o.Wallet?.TelegramUser?.Username,
                Balance = balances[o.WalletId ?? 0],
                ChatId = o.Wallet?.TelegramUser?.ChatId ?? 0,
                CountTransaction = o.Sum,
                BalanceBoost = o.Wallet?.BalanceBoost ?? 0,
                Chat = o.Chat.Select(o => new MessageDTO() { Message = o.Message, DateTime = o.DateTimeUTC, IsUser = o.IsUserSender, Tag = o.Tag, File = o.File }).ToArray(),
                Banks = o.Banks.Split(o.Separator),
            }).ToArray();
        }
    }
}
