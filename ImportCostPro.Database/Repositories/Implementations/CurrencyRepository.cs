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

        public async Task<bool> HasRelatedEntitiesAsync(Guid id)
        {
            return await _context.Suppliers.AnyAsync(s => s.CurrencyId == id) ||
                   await _context.ExchangeRates.AnyAsync(r => r.SourceCurrencyId == id || r.TargetCurrencyId == id) ||
                   await _context.ImportOrders.AnyAsync(o => o.CurrencyId == id) ||
                   await _context.ImportOrderExpenses.AnyAsync(e => e.CurrencyId == id) ||
                   await _context.LandedCostCalculations.AnyAsync(c => c.LocalCurrencyId == id);
        }

        public async Task<Currency?> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.Set<Currency>().FindAsync(id);
            if (entity != null)
            {
                entity.Update(entity.Name, entity.ISOCode, entity.Symbol, entity.IsLocalCurrency, false);
                await _context.SaveChangesAsync();
                return entity;
            }
            return null;
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var query = _context.Currencies.Where(c => c.Name == name);
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByISOCodeAsync(string isoCode, Guid? excludeId = null)
        {
            var query = _context.Currencies.Where(c => c.ISOCode == isoCode);
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
            return await query.AnyAsync();
        }
    }
}