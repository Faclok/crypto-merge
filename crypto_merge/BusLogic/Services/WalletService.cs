using BusLogic.Requests;
using InternetDatabase.EntityDB;
using InternetDatabase.EntityDTO;
using InternetDatabase.EntityDTO.Deposit;
using InternetDatabase.Extensions;
using InternetDatabase.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BusLogic.Services;

public class  WalletService(InternetDbContext context)
{

    public Task<bool> CheckAsync(Wallet wallet)
    {
        return context.Wallets
            .IgnoreQueryFilters()
            .Where(o => o.PhoneNumber == wallet.PhoneNumber && o.Bank == wallet.Bank || o.NumberCard == wallet.NumberCard)
            .AnyAsync();
    }
    public async Task<Wallet> CreateAsync(Wallet wallet)
    {
        wallet.Status = WalletStatus.Connection;
        context.Wallets.Add(wallet);

        await context.SaveChangesAsync();
        return wallet;
    } 
    public Task<Wallet[]> GetByStatus(WalletStatus status)
    {
        return context.Wallets.AsNoTracking().Where(w => w.Status == status).ToArrayAsync();
    }

    public Task<int> GetMyWalletAll(long chatId)
    {
        return context.Wallets.Where(o => o.TelegramUser != null && o.TelegramUser.ChatId == chatId).CountAsync();
    }

    public Task<int> GetMyConnectedWallets(long chatId)
    {
        return context.Wallets.Where(o => o.TelegramUser != null && o.TelegramUser.ChatId == chatId).Where(o => o.Status == WalletStatus.Connected).CountAsync();
    }

    public Task<bool> AnyPhoneNumberAsync(string phoneNumber)
        => context.Wallets.AnyAsync(w=> w.PhoneNumber == phoneNumber);

    public Task<bool> AnyCardNumAsync(string cardNum)
        => context.Wallets.AnyAsync(w => w.NumberCard == cardNum);

    public Task<TransactionDTO[]> GetTransactionWalletReferralYesterdayAsync(long chatId)
        => GetTransactionWalletReferralBackAsync(chatId, DateTime.UtcNow.AddDays(-1));

    public Task<TransactionDTO[]> GetTransactionWalletReferralThreeDayBackAsync(long chatId)
        => GetTransactionWalletReferralBackAsync(chatId, DateTime.UtcNow.AddDays(-3));

    public Task<TransactionDTO[]> GetTransactionWalletReferralBackAsync(long chatId, DateTime yesterday)
        => GetTransactionWalletBaseAsync(chatId)
            .Where(o => o.Type == TransactionType.Referral && o.DateTimeUTC >= yesterday)
            .GetTransactionDTOsAsync();

    public async Task UpdateStatusAsync(int Id, WalletStatus walletStatus)
     {  
         var count = await context.Wallets.Where(w => w.Id == Id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.Status, walletStatus));

        if(walletStatus == WalletStatus.Connected)
        {
            var wallet = await context.Wallets
                .Include(o => o.TelegramUser)
                .Include(o => o.TransactionWallets)
                .FirstAsync(o => o.Id == Id);

            if(wallet.TelegramUser != null && wallet.TelegramUser.BalanceTemp != 0m)
            {
                wallet.TransactionWallets.Add(new(1, wallet.Id, wallet.TelegramUser.BalanceTemp, "Ваш баланс", TransactionStatus.Completed, TransactionType.None));
                wallet.LimitFix += wallet.TelegramUser.BalanceTemp;
                wallet.TelegramUser.BalanceTemp = 0m;
            }

            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> TryDelete(int id)
    {
        await context.Wallets.Where(w => w.Id == id).SelectMany(o => o.TransactionWallets).ExecuteDeleteAsync();
        var result = await context.Wallets.Where(w => w.Id == id).ExecuteDeleteAsync();

       return result > 0;
    }

    public IEnumerable<Wallet> GetByChatId(long chatId, WalletStatus walletStatus = WalletStatus.Connected)
    {
        return context.Wallets.AsNoTracking()
                .Where(w => w.TelegramUser != null && w.TelegramUser.ChatId == chatId && w.Status == walletStatus)
                .AsEnumerable();
    }

    public Task<Wallet?> GetById(long Id)
    {
        return context.Wallets.AsNoTracking().SingleOrDefaultAsync(w => w.Id == Id);
    }

    public async Task<decimal> GetSumMoneyInReferralAsync(long chatId)
    {
        return await context.Users.Where(o => o.ChatId == chatId)
                .SelectMany(o => o.Wallets)
                .SelectMany(o => o.TransactionWallets)
                .Where(o => o.Type == TransactionType.Referral)
                .SumAsync(o => o.Sum);
    }

    public async Task<decimal> GetTotalBalanceAsync(long chatId)
    {
        return await context.Wallets
                .Where(o => o.TelegramUser != null && o.TelegramUser.ChatId == chatId)
                .SelectMany(o => o.TransactionWallets)
                .Where(o => o.Status == TransactionStatus.Completed)
                .SumAsync(o => o.Sum);
    }

    public async Task<decimal> GetTotalBalanceAsync(int walletId)
    {
        return await context.TransactionWallets
                .Where(o => o.WalletId == walletId)
                .Where(o => o.Status == TransactionStatus.Completed)
                .SumAsync(o => o.Sum);
    }

    public async Task<decimal> GetTotalBalanceAsTransactionAsync(int transactionId)
    {
        if (await context.TransactionWallets.AsNoTracking().FirstOrDefaultAsync(o => o.Id == transactionId) is not { } transaction)
            return 0m;

        return await GetTotalBalanceAsync(transaction.WalletId ?? 0);
    }

    public async Task<decimal> GetSumMoneyInReferralAndTempAsync(long chatId)
    {
        return await context.Users.Where(o => o.ChatId == chatId)
                .SelectMany(o => o.Wallets)
                .SelectMany(o => o.TransactionWallets)
                .Where(o => o.Type == TransactionType.Referral)
                .SumAsync(o => o.Sum) + await context.Users.Where(o => o.ChatId == chatId).SumAsync(o => o!.BalanceTemp);
    }

    public async Task<decimal> GetTotalBalanceAndTempAsync(long chatId)
    {
        return await context.Wallets
                .Where(o => o.TelegramUser != null && o.TelegramUser.ChatId == chatId)
                .SelectMany(o => o.TransactionWallets)
                .Where(o => o.Status == TransactionStatus.Completed)
                .SumAsync(o => o.Sum) + await context.Users.Where(o => o.ChatId == chatId).SumAsync(o => o.BalanceTemp);
    }

    public async Task<decimal> GetTotalBalanceAndTempAsync(int walletId)
    {
        return await context.TransactionWallets
                .Where(o => o.WalletId == walletId)
                .Where(o => o.Status == TransactionStatus.Completed)
                .SumAsync(o => o.Sum) + await context.Wallets.Where(o => o.Id == walletId).Where(o => o.TelegramUser != null).Select(o => o.TelegramUser).SumAsync(o => o!.BalanceTemp);
    }

    public async Task<decimal> GetTotalBalanceAsTransactionAndTempAsync(int transactionId)
    {
        if (await context.TransactionWallets.AsNoTracking().FirstOrDefaultAsync(o => o.Id == transactionId) is not { } transaction)
            return 0m;

        return await GetTotalBalanceAndTempAsync(transaction.WalletId ?? 0);
    }

    public async Task<decimal> GetLimitYesterdayAsync(int walletId, string titleCurrency)
    {
        var limitRub = GetDayDepositSum(walletId) + await GetLimitYesterdayAsync(walletId);

        if (await context.Currencies.AsNoTracking().FirstOrDefaultAsync(o => o.Title == titleCurrency) is not { } currency)
            return 0m;

        if (limitRub == 0m)
            return 0m;

        return limitRub / currency.RubCurrency;
    }

    public async Task<decimal> GetLimitYesterdayOnTransactionAsync(int transactionId, string titleCurrency)
    {
        if (await context.TransactionWallets.AsNoTracking().FirstOrDefaultAsync(o => o.Id == transactionId) is not { } transaction)
            return 0m;

        var limitRub = GetDayDepositSum(transaction.WalletId ?? 0) + await GetLimitYesterdayAsync(transaction.WalletId ?? 0);

        if (await context.Currencies.AsNoTracking().FirstOrDefaultAsync(o => o.Title == titleCurrency) is not { } currency)
            return 0m;

        if (limitRub == 0m)
            return 0m;

        return limitRub / currency.RubCurrency;
    }

    public async Task<decimal> GetBalanceBoostOnTransaction(int transactionId, string titleCurrency)
    {
        if (await context.Currencies.AsNoTracking().FirstOrDefaultAsync(o => o.Title == titleCurrency) is not { } currency)
            return 0m;

        var boost = await context.TransactionWallets
            .Where(t => t.Id == transactionId)
            .Select(t => t.Wallet!.BalanceBoost)
            .FirstOrDefaultAsync();

        if (boost == 0m)
            return 0m;

        return boost / currency.RubCurrency;
    }

    public async Task<decimal> GetBalanceBoost(int walletId, string titleCurrency)
    {
        if (await context.Currencies.AsNoTracking().FirstOrDefaultAsync(o => o.Title == titleCurrency) is not { } currency)
            return 0m;

        var boost = await context.Wallets
            .Where(o => o.Id == walletId)
            .Select(t => t.BalanceBoost)
            .FirstOrDefaultAsync();

        if (boost == 0m)
            return 0m;

        return boost / currency.RubCurrency;
    }

    public async Task<int> PutBalanceBoost(int id, decimal sum, string currencyTitle)
    {
        if (await context.Currencies.FirstOrDefaultAsync(o => o.Title == currencyTitle) is not { } currency)
            return 0;

        return await context.Wallets
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.BalanceBoost, sum * currency.RubCurrency));
    }

    public async Task<int> PutCCID(int id, string cc)
    {
        return await context.Wallets
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.CryptoCardId, cc));
    }

    public async Task<bool> PutAccountCryptoCardId(int id, int accountCryptoCardId)
    {
        if(!await context.AccountCryptoCards.AnyAsync(o => o.Id == accountCryptoCardId))
            return false;

        await context.Wallets
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.AccountCryptoCardId, accountCryptoCardId));

        return true;
    }

    public Task<decimal> GetLimitYesterdayAsync(int walletId)
        => context.Wallets.Where(o => o.Id == walletId).Select(o => o.LimitFix).SumAsync();

    public decimal GetDayDepositSum(int walletId)
    {
        var date = DateTime.UtcNow.AddHours(3).Date.AddMinutes(-20);
        var now = date.AddDays(1);
        return context.TransactionWallets
            .Where(o => o.WalletId == walletId)
            .Where(o => o.Status == TransactionStatus.Completed && o.Type == TransactionType.Deposit)
            .AsEnumerable()
            .Where(o => o.DateTimeUTC.AddHours(3) > date && o.DateTimeUTC.AddHours(3) <= now)
            .Sum(o => o.Sum);
    }

    public async Task<TransactionDTO[]> GetTransactionWalletAsync(int walletId)
    {
        return await GetTransactionWalletBaseAsync(walletId)
            .GetTransactionDTOsAsync();
    }

    public Task<TransactionDTO[]> GetTransactionWalletYesterdayAsync(long chatId)
        => GetTransactionWalletBackAsync(chatId, DateTime.UtcNow.AddDays(-1));

    public Task<TransactionDTO[]> GetTransactionWalletThreeDayBackAsync(long chatId)
        => GetTransactionWalletBackAsync(chatId, DateTime.UtcNow.AddDays(-3));

    public Task<TransactionDTO[]> GetTransactionWalletBackAsync(long chatId, DateTime dateTime)
        => GetTransactionWalletBaseAsync(chatId)
            .Where(o => o.DateTimeUTC >= dateTime)
            .GetTransactionDTOsAsync();

    private IQueryable<TransactionWallet> GetTransactionWalletBaseAsync(long chatId)
        => context.Users
            .AsNoTracking()
            .Where(u => u.ChatId == chatId)
            .SelectMany(o => o.Wallets)
            .Include(o => o.TransactionWallets)
            .SelectMany(o => o.TransactionWallets);

    private IQueryable<TransactionWallet> GetTransactionWalletBaseAsync(int walletId)
        => context.Users
            .AsNoTracking()
            .Where(u => u.Id == walletId)
            .SelectMany(o => o.Wallets)
            .Include(o => o.TransactionWallets)
            .SelectMany(o => o.TransactionWallets);

    public Task<TransactionWallet> AddMoneyTransactionAsync(int walletId, int bankId, int countMoney, string message, TransactionType type = TransactionType.None, TransactionStatus status = TransactionStatus.Completed)
    {
        return context.AddMoneyTransactionAsync(walletId, bankId, countMoney, message, type, status);
    }

    public async Task<TransactionDTO?> CreateTransaction(long chatId, decimal sum, string[] banks)
    {
        if (await context.Wallets
            .Where(o => o.TelegramUser != null && o.TelegramUser.ChatId == chatId)
            .Where(o => o.Status == WalletStatus.Connected)
            .Include(o => o.TransactionWallets)
            .FirstOrDefaultAsync() is not { } wallet)
            return null;

        var transaction = new TransactionWallet(1, wallet.Id, sum, "Пополнение", banks);

        wallet.TransactionWallets.Add(transaction);
        await context.SaveChangesAsync();
        return transaction.GetTransactionDTO();
    }

    public Task<bool> IsCheckAsync(string cc)
        => context.TransactionWallets.AnyAsync(o => o.TransactionIdCC == cc);

    public async Task<TransactionDTO?> CreateTransactionCC(RequestTransaction requestTransaction)
    {
        if (await context.Wallets
            .Include(o => o.TransactionWallets)
            .FirstOrDefaultAsync(o => o.PhoneNumber == requestTransaction.RequestProperty || o.NumberCard == requestTransaction.RequestProperty) is not { } wallet)
            return null;

        var transaction = new TransactionWallet(1, wallet.Id, -requestTransaction.Count, "Вывод", requestTransaction.TransactionIdCC);

        wallet.TransactionWallets.Add(transaction);
        await context.SaveChangesAsync();
        return transaction.GetTransactionDTO();
    }

    public async Task<TransactionDTO?> CreateTransactionCC(RequestTransaction requestTransaction, DateTime dateTime)
    {
        if (await context.Wallets
            .Include(o => o.TransactionWallets)
            .FirstOrDefaultAsync(o => o.PhoneNumber == requestTransaction.RequestProperty || o.NumberCard == requestTransaction.RequestProperty) is not { } wallet)
            return null;

        var transaction = new TransactionWallet(1, wallet.Id, -requestTransaction.Count, "Вывод", requestTransaction.TransactionIdCC, dateTime);

        wallet.TransactionWallets.Add(transaction);
        await context.SaveChangesAsync();
        return transaction.GetTransactionDTO();
    }

    public async Task<TransactionCCSearchDTO[]> SearchTransactions(string? request, string? cc, long? chatId, decimal? sum)
    {
        IQueryable<TransactionWallet> transactions = context.TransactionWallets;

        if (request != null)
            transactions = transactions.Where(o => o.Wallet != null && (o.Wallet.PhoneNumber == request || o.Wallet.NumberCard == request));

        if (cc != null)
            transactions = transactions.Where(o => o.TransactionIdCC == cc);

        if (chatId != null)
            transactions = transactions.Where(o => o.Wallet != null && o.Wallet.TelegramUser != null && o.Wallet.TelegramUser.ChatId == chatId);

        if (sum != null)
            transactions = transactions.Where(o => o.Sum == sum);

        return await transactions
            .OrderByDescending(o => o.DateTimeUTC)
            .GetTransactionCCSearchDTOsAsync();
    }

    public Task<decimal> GetTotalDepositSumAsync(long chatId, DateTime? startDate = null)
    {
        if (startDate is null)
            return GetTransactionWalletBaseAsync(chatId).Where(t => t.Type == TransactionType.Deposit).SumAsync(t => t.Sum);
        return GetTransactionWalletBaseAsync(chatId).Where(t => t.Type == TransactionType.Deposit&& t.DateTimeUTC > startDate).SumAsync(t => t.Sum);
    }

    public async Task<TransactionDTO?> GetTransactionByIdCCAsync(string idCC)
    {
        return (await context.TransactionWallets.FirstOrDefaultAsync(o => o.TransactionIdCC == idCC))?.GetTransactionDTO();
        
    }

    public async Task<bool> UpdateTransactionStatus(int id, TransactionStatus status)
    {
        return (await context.TransactionWallets
            .Where(t=>t.Id == id)
            .ExecuteUpdateAsync(s=>s.SetProperty(t=>t.Status,status))) > 0;
    }

    public async Task<bool> DeleteTransaction(int id)
    {
        return (await context.TransactionWallets.Where(t => t.Id == id).ExecuteUpdateAsync(s => s.SetProperty(p => p.SoftDeleted, true))) > 0;
    }

    public Task<Wallet?> GetAsync(long? id, string? login, string? numberCart, string? teleNumber)
    {
        IQueryable<Wallet> test = context.Wallets;
        
        if (id is not null)
            test= test.Where(w => w.TelegramUser != null && w.TelegramUser!.ChatId == id);
        if (login is not null)
            test = test.Where(w => w.Login == login);
        if (numberCart is not null)
            test=test.Where(w=>w.NumberCard == numberCart);
        if (teleNumber is not null)
            test = test.Where(w=>w.PhoneNumber == teleNumber);

        return test.FirstOrDefaultAsync();

    }

    public Task<long> GetAuthorChatId(int id)
    {
        return context.Wallets.Where(o => o.Id == id && o.TelegramUserId != null)
            .Select(o => o.TelegramUser!.ChatId)
            .FirstOrDefaultAsync();
    }

    public Task<Wallet[]> GetAsync(long chatId, bool isIgnoreFilter = false)
    {
        if(isIgnoreFilter)
            return context.Users.IgnoreQueryFilters().Where(o => o.ChatId == chatId)
            .Include(o => o.Wallets)
            .SelectMany(o => o.Wallets)
            .ToArrayAsync();

        return context.Users.Where(o => o.ChatId == chatId)
            .Include(o => o.Wallets)
            .SelectMany(o => o.Wallets)
            .ToArrayAsync();
    }

    public async Task<bool> TrySetBalanceAsync(long chatId, decimal sum, string comment)
    {
        var total = await GetTotalBalanceAndTempAsync(chatId);

        var wallets = await GetAsync(chatId);

        wallets = wallets.Where(o => o.Status == WalletStatus.Connected).ToArray();

        if (wallets.Length <= 0)
            return true;

        for (int i = 0; i < wallets.Length; i++)
        {
            var currentWallet = wallets[i];
            var balance = await GetTotalBalanceAndTempAsync(currentWallet.Id);
            if (i == wallets.Length - 1)
            {

                currentWallet.TransactionWallets.Add(new(1, currentWallet.Id, sum - balance, comment, TransactionType.None));
                break;
            }
            currentWallet.TransactionWallets.Add(new(1, currentWallet.Id, -balance, comment, TransactionType.None));
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryUpBalanceAsync(long chatId, decimal sum, string comment)
    {
        var total = await GetTotalBalanceAndTempAsync(chatId);

        var wallets = await GetAsync(chatId);

        wallets = wallets.Where(o => o.Status == WalletStatus.Connected).ToArray();

        if (wallets.Length <= 0)
            return true;

        var wallet = wallets[0];

        wallet.TransactionWallets.Add(new(1, wallet.Id, sum, comment, TransactionType.None));

        await context.SaveChangesAsync();
        return true;
    }

    public Task<TransactionWallet[]> Search(string idCC)
    {
        return context.TransactionWallets
            .Where(w => w.TransactionIdCC == idCC).ToArrayAsync();
    }

    public async Task<WalletDTO[]> GetLimitsAsync(decimal rubCurrency)
    {
        var result = await context.Wallets
            .AsNoTracking()
            .Where(o => !o.IsCheckYesterday)
            .Select(o => new WalletDTO()
            {
                Id = o.Id,
                NumberCard = o.NumberCard,
                Phone = o.PhoneNumber,
                Balance = o.TransactionWallets.Where(o => o.Status == TransactionStatus.Completed).Sum(o => o.Sum),
                ChatId = o.TelegramUser!.ChatId,
                AccountCryptoCardId = o.AccountCryptoCardId,
            }).ToArrayAsync();

        return result.Select(o =>
        {
            o.Balance /= rubCurrency;
            return o;
        }).ToArray();
    }

    public Task<int> GetLimitsCountAsync()
    {
        return context.Wallets
            .AsNoTracking()
            .Where(o => !o.IsCheckYesterday)
            .CountAsync();
    }

    public  async Task PutLimitAsync(int walletId)
    {
        var balance = await GetTotalBalanceAndTempAsync(walletId);
        await context.Wallets.Where(o => o.Id == walletId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.LimitFix, balance)
                                          .SetProperty(p => p.IsCheckYesterday, true));
    }

    public Task<int> GetCountTransactionOnWaiting(int id)
    {
        return context.Wallets.Where(o => o.Id == id)
            .SelectMany(o => o.TransactionWallets)
            .Where(o => o.Status == TransactionStatus.Wait || o.Status == TransactionStatus.InProgress)
            .CountAsync();
    }

    public async Task<int> SoftDelete(int id)
    {
        if (await context.Wallets.Include(o => o.TransactionWallets).Include(o => o.TelegramUser).FirstOrDefaultAsync(o => o.Id == id) is not { TelegramUser: not null } wallet)
            return 0;

        using var transaction = await context.Database.BeginTransactionAsync();

        var balance = await GetTotalBalanceAsync(id);

        wallet.TransactionWallets.Add(new TransactionWallet(1, id, -balance, "Удаление банка", TransactionStatus.Completed, TransactionType.None));
        wallet.Status = WalletStatus.Stop;
        wallet.SoftDeleted = true;
        wallet.TelegramUser.BalanceTemp += balance;

        await context.SaveChangesAsync();

        await transaction.CommitAsync();

        return 1;
    }
}
