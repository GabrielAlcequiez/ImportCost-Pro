using ImportCostPro.Database.Entities;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface ICurrencyRepository : IBaseRepository<Currency>
    {
        Task<IReadOnlyList<Currency>> GetLocalCurrenciesAsync(Guid? exceptId = null);
    }
}