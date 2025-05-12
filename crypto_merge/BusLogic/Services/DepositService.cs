using InternetDatabase.EntityDTO.Deposit;
using InternetDatabase.Extensions;
using InternetDatabase.Services;
using Microsoft.EntityFrameworkCore;

namespace BusLogic.Services
{
    public class DepositService(InternetDbContext context)
    {

        /// <summary>
        /// Получение всех депозитов
        /// ДЛЯ АДМИНИСТРАТОРОВ
        /// </summary>
        /// <returns></returns>
        public async Task<DepositDTO[]> GetActiveDepositsAsync(Func<int, Task<decimal>> getSum, string titleCurrency)
        {
            if (await context.Currencies.AsNoTracking().FirstOrDefaultAsync(o => o.Title == titleCurrency) is not { } currency)
                return [];

            var result = await context.TransactionWallets
                .AsNoTrackingWithIdentityResolution()
                .Where(o => o.Type == InternetDatabase.EntityDB.TransactionType.Deposit)
                .Where(o => o.Status < InternetDatabase.EntityDB.TransactionStatus.Completed)
                .GetDepositDTOsAsync(getSum);

            return result.Select(o =>
            {
                if (o.BalanceBoost > 0m)
                    o.BalanceBoost /= currency.RubCurrency;

                return o;
            }).ToArray();
        }

        /// <summary>
        /// Получение всех депозитов
        /// ДЛЯ АДМИНИСТРАТОРОВ
        /// </summary>
        /// <returns></returns>
        public async Task<DepositDTO[]> GetActiveDepositsOnCCAsync(Func<int, Task<decimal>> getSum, string titleCurrency, int accountCryptoCardId)
        {
            if (await context.Currencies.AsNoTracking().FirstOrDefaultAsync(o => o.Title == titleCurrency) is not { } currency)
                return [];

            var result = await context.TransactionWallets
                .AsNoTrackingWithIdentityResolution()
                .Where(o => o.Wallet != null && o.Wallet.AccountCryptoCardId == accountCryptoCardId)
                .Where(o => o.Type == InternetDatabase.EntityDB.TransactionType.Deposit)
                .Where(o => o.Status < InternetDatabase.EntityDB.TransactionStatus.Completed)
                .GetDepositDTOsAsync(getSum);

            return result.Select(o =>
            {
                if (o.BalanceBoost > 0m)
                    o.BalanceBoost /= currency.RubCurrency;

                return o;
            }).ToArray();
        }
    }
}
