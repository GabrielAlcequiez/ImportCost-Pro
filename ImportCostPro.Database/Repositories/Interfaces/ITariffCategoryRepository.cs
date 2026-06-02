using ImportCostPro.Database.Entities;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface ITariffCategoryRepository : IBaseRepository<TariffCategory>
    {
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
        Task<bool> HasRelatedProductsAsync(Guid categoryId);

        Task<bool> HasRelatedEntitiesAsync(Guid id);
        Task<TariffCategory?> SoftDeleteAsync(Guid id);
    }
}