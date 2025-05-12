using InternetDatabase.EntityDB;
using InternetDatabase.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusLogic.Services
{
    public class TransactionService(InternetDbContext context)
    {

        public async Task<bool> TryUpdateSumAsync(int id, decimal sum, string comment)
        {
            var count = await context.TransactionWallets.Where(o => o.Id == id)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.Sum, sum).SetProperty(p => p.Message, comment));

            return count > 0;
        }

        public async Task CreateReferralDeposit(int id, decimal sum, decimal percent)
        {
            if (await context.TransactionWallets
                                        .AsNoTracking()
                                        .Where(o => o.Id == id)
                                        .Select(o => o.Wallet)
                                        .Where(o => o != null && o.TelegramUserId != null)
                                        .Select(o => o!.TelegramUser)
                                        .FirstOrDefaultAsync() is not { } user)
                return;

            if (await context.Users.Where(o => o.Id == user.CameFromTelegramUserId).Include(o => o.Wallets)
                                    .ThenInclude(o => o.TransactionWallets).FirstOrDefaultAsync() is { } userReferral)
            {
                if(userReferral.Wallets.FirstOrDefault(o => o.Status == WalletStatus.Connected) is { } wallet)
                    wallet.TransactionWallets.Add(new(1, wallet.Id, sum * percent, "Реферал", TransactionStatus.Completed, TransactionType.Referral));
                else
                    userReferral.BalanceTemp += sum * percent;

                await context.SaveChangesAsync();
            }
        }

        public Task UpdateDateTimeUTC(int id, DateTime dateTime)
            => context.TransactionWallets.Where(o => o.Id == id)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.DateTimeUTC, dateTime));

        public async Task<decimal> GetSumAsync(int id, string titleCurrency)
        {
            if (await context.TransactionWallets.FirstOrDefaultAsync(f => f.Id == id) is not { } transaction)
                return 0m;

            if (await context.Currencies.FirstOrDefaultAsync(f => f.Title == titleCurrency) is not { } currency)
                return 0m;

            return transaction.Sum / currency.RubCurrency;
        }

        public async Task<decimal> GetSumAsync(int id)
        {
            if (await context.TransactionWallets.FirstOrDefaultAsync(f => f.Id == id) is not { } transaction)
                return 0m;

            return transaction.Sum;
        }

        public Task AddPercentOnSumAsync(int id, decimal percent)
            => context.TransactionWallets.Where(w => w.Id == id)
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.Sum, p => p.Sum  + p.Sum * percent));
    }
}
