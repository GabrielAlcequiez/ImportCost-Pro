using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Implementations;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface IExchangeRateRepository : IBaseRepository<ExchangeRate>
    {
        Task<bool> ExistsActiveRateAsync(Guid sourceCurrencyId, Guid targetCurrencyId, DateTime effectiveDate, Guid? excludeId = null);
    }
}