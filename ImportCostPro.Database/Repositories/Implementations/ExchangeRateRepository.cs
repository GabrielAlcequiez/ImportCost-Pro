using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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
                x.EffectiveDate.Date == effectiveDate.Date &&
                x.IsActive == true);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasRelatedEntitiesAsync(Guid id)
        {
            // Debido a la arquitectura de Captura de Valor Congelado hecha en landed cost, 
            // las tasas de cambio no tienen dependencias  en otras tablas.
            // Retornar false permite que el servicio ejecute un Delete (borrado físico) sin problemas.
            return await Task.FromResult(false);
        }

        public async Task<ExchangeRate?> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.Set<ExchangeRate>().FindAsync(id);

            if (entity != null)
            {
                entity.Update(
                    entity.RateValue,
                    entity.EffectiveDate,
                    false
                );
                await _context.SaveChangesAsync();
                return entity;
            }
            return null;
        }
    }
}