using ImportCostPro.Database.Entities;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface ISupplierRepository : IBaseRepository<Supplier>
    {
        Task<bool> HasRelatedEntitiesAsync(Guid id);
        Task<Supplier?> SoftDeleteAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
    }
}