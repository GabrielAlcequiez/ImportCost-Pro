using ImportCostPro.Database.Entities;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface ICountryRepository : IBaseRepository<Country>
    {
        Task<bool> HasRelatedEntitiesAsync(Guid id);
        Task<Country?> SoftDeleteAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
        Task<bool> ExistsByISOCodeAsync(string isoCode, Guid? excludeId = null);
    }
}