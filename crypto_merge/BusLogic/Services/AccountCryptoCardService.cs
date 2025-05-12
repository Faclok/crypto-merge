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
    public class AccountCryptoCardService(InternetDbContext context)
    {

        public Task<AccountCryptoCard[]> GetAllAsync()
            => context.AccountCryptoCards.AsNoTracking().ToArrayAsync();

        public async Task CreateAsync(string name)
        {
            await context.AccountCryptoCards.AddAsync(new() { Name = name });
            await context.SaveChangesAsync();
        }

        public async Task<bool> PutNameAsync(int id, string name)
            => await context.AccountCryptoCards.Where(o => o.Id == id).ExecuteUpdateAsync(s => s.SetProperty(p => p.Name, name)) > 0;
    }
}
