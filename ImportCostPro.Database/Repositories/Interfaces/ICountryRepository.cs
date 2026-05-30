using ImportCostPro.Database.Entities;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface ICountryRepository : IBaseRepository<Country>
    {
        // metodos adicionales necesarios en caso de serlos
        Task<bool> HasRelatedEntitiesAsync(Guid id);
        Task<Country?> SoftDeleteAsync(Guid id);
    }
}