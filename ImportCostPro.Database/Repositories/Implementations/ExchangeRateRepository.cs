using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class ExchangeRateRepository : BaseRepository<ExchangeRate>, IExchangeRateRepository
    {
        public ExchangeRateRepository(AppDbContext context) : base(context)
        {
            
        }
        public async Task<bool> ExistsActiveRateAsync(Guid sourceCurrencyId, Guid targetCurrencyId, DateTime effectiveDate, Guid? excludeId = null)
        {
            var query = _context.ExchangeRates.Where(x => 
                x.SourceCurrencyId == sourceCurrencyId &&
                x.TargetCurrencyId == targetCurrencyId &&
                x.EffectiveDate == effectiveDate &&
                x.IsActive == true);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}