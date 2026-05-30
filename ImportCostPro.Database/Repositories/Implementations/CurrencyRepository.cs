using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class CurrencyRepository : BaseRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Currency>> GetLocalCurrenciesAsync(Guid? exceptId = null)
        {
            var query = _context.Currencies.Where(x => x.IsLocalCurrency);

            if (exceptId.HasValue)
            {
                query = query.Where(c => c.Id != exceptId.Value);
            }

            return await query.ToListAsync();
        }
    }
}