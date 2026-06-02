using ImportCostPro.Database.Entities;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<bool> HasRelatedEntitiesAsync(Guid id);
        Task<Product?> SoftDeleteAsync(Guid id);
        Task<bool> ExistsByReferenceCodeAsync(string referenceCode, Guid? excludeId =null);
    }
}