using InternetDatabase.EntityDB;
using InternetDatabase.EntityDTO;
using InternetDatabase.Services;
using Microsoft.EntityFrameworkCore;

namespace InternetDatabase.Extensions
{
    public static class WalletExtensions
    {
        public const int APP_BANK_ID = 1;

        public static Task<Bank> GetMainBank(this IQueryable<Bank> banks)
            => banks.FirstAsync(o => o.Id == APP_BANK_ID);

        public static Task<TransactionDTO[]> GetTransactionDTOsAsync(this IQueryable<TransactionWallet> transactionWallet)
            => transactionWallet
            .Include(o => o.Bank)
            .Select(o => new TransactionDTO()
            {
                BankId = o.BankId,
                Status = o.Status,
                Count = o.Sum,
                Type = o.Type,
                DateTimeUTC = o.DateTimeUTC,
                Message = o.Message,
            })
            .ToArrayAsync();

        public static Task<TransactionCCSearchDTO[]> GetTransactionCCSearchDTOsAsync(this IQueryable<TransactionWallet> transactionWallet)
            => transactionWallet
            .Include(o => o.Bank)
            .Select(o => new TransactionCCSearchDTO()
            {
                Id = o.Id,
                WalletId = o.WalletId,
                BankId = o.BankId,
                Status = o.Status,
                Count = o.Sum,
                DateTimeUTC = o.DateTimeUTC,
                Message = o.Message,
                Cc = o.TransactionIdCC,
                NumberCard = o.Wallet.NumberCard,
                Phone = o.Wallet.PhoneNumber,
                Type = o.Type,
            })
            .ToArrayAsync();

        public static TransactionDTO GetTransactionDTO(this TransactionWallet transactionWallet)
            => new()
            {
                Id = transactionWallet.Id,
                BankId = transactionWallet.BankId,
                Status = transactionWallet.Status,
                Count = transactionWallet.Sum,
                Type = transactionWallet.Type,
                DateTimeUTC = transactionWallet.DateTimeUTC,
                Message = transactionWallet.Message,
            };

        public static async Task<TransactionWallet> AddMoneyTransactionAsync(this InternetDbContext context, int walletId, int bankId, int countMoney, string message, TransactionType type, TransactionStatus status)
        {
            if (!await context.Wallets.AnyAsync(o => o.Id == walletId))
                throw new ArgumentException(null, nameof(walletId));

            if (!await context.Banks.AnyAsync(o => o.Id == bankId))
                throw new ArgumentException(null, nameof(bankId));

            var transactionWallet = new TransactionWallet(bankId, walletId, countMoney, message, status, type);

            await context.TransactionWallets.AddAsync(transactionWallet);
            await context.SaveChangesAsync();

            return transactionWallet;
        }
    }
}
