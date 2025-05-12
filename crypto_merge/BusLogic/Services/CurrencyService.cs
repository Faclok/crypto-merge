using InternetDatabase.EntityDB;
using InternetDatabase.Services;
using Microsoft.EntityFrameworkCore;

namespace BusLogic.Services
{
    public class CurrencyService(InternetDbContext context)
    {

        public Task<Currency?> GetCurrencyAsync(string title)
            => context.Currencies.Where(o => o.Title == title).FirstOrDefaultAsync();

        public Task<int> PutCurrencyOnRubAsync(string title, decimal rub)
            => context.Currencies.Where(o => o.Title == title).ExecuteUpdateAsync(s => s.SetProperty(p => p.RubCurrency, rub));
    }
}
