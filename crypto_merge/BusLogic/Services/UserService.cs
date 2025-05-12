using InternetDatabase.EntityDB;
using InternetDatabase.Services;
using Microsoft.EntityFrameworkCore;

namespace BusLogic.Services
{
    public class UserService(InternetDbContext context)
    {
        public Task<TelegramUser?> GetByWalletIdAsync(int id)
        {
            return context.Wallets.AsNoTracking().Where(w=>w.Id == id).Select(w=>w.TelegramUser).FirstOrDefaultAsync();
        }

        public Task<bool> ContainsAsync(long chatId)
            => context.Users.AnyAsync(context => context.ChatId == chatId);

        public async Task<string> GetReferralLinkSelf(string botUsername, long chatId)
            => "t.me/" + botUsername + "?start=" + await context.Users.Where(o => o.ChatId == chatId).Select(o => o.Id).FirstOrDefaultAsync();

        public async Task<TelegramUser> CreateAsync(long chatId, string fullName, string? username, int? referralUserId = null)
        {
            if (await context.Users.AnyAsync(o => o.ChatId == chatId))
                throw new ArgumentException("user exits", nameof(chatId));


            var referralUser = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == referralUserId);

            var createUser = new TelegramUser(chatId, referralUser?.Id)
            {
                FullName = fullName,
                Username = username,
                Wallets = []
            };

            await context.Users.AddAsync(createUser);
            await context.SaveChangesAsync();

            return createUser;
        }

        public Task<TelegramUser?> GetByChatIdAsync(long chatId)
        {
           return context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);
        }

        public Task<int> GetCountWallets(long chatId, bool isIgnoreFilter = false)
        {
            IQueryable<TelegramUser> users = context.Users;

            if(isIgnoreFilter)
                return context.Users.IgnoreQueryFilters().Where(u => u.ChatId == chatId).SelectMany(u => u.Wallets).CountAsync();

            return context.Users.Where(u => u.ChatId == chatId).SelectMany(u => u.Wallets).CountAsync();
        }

        public async Task<long> SearchByRequisitesAsync(string requisites, bool isIgnoreFilter = false)
        {

           if(isIgnoreFilter)
                return await context.Wallets
                .IgnoreQueryFilters()
                .Where(w => w.Fio == requisites || w.NumberCard == requisites || w.Login == requisites || w.PhoneNumber == requisites || w.Password == requisites)
                .Where(o => o.TelegramUserId != null)
                .Select(w => w.TelegramUser!.ChatId)
                .FirstOrDefaultAsync();

            return await context.Wallets
                .Where(w => w.Fio == requisites || w.NumberCard == requisites || w.Login == requisites || w.PhoneNumber == requisites || w.Password == requisites)
                .Where(o => o.TelegramUserId != null)
                .Select(w=> w.TelegramUser!.ChatId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> TryUpdateNoteAsync(long chatId, string note)
            => await context.Users.Where(o => o.ChatId == chatId)
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.NoteAdmin, note)) > 0;
    }
}
