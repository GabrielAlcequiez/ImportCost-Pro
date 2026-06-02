using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class TariffCategoryRepository : BaseRepository<TariffCategory>, ITariffCategoryRepository
    {
        public TariffCategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }
        public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null)
        {
            var query = _context.TariffCategories.Where(t => t.TariffCode == code);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasRelatedEntitiesAsync(Guid id)
        {
            return await _context.Products.AnyAsync(s => s.TariffCategoryId == id);
        }

        public async Task<bool> HasRelatedProductsAsync(Guid categoryId)
        {
            return await _context.Products.AnyAsync(p => p.TariffCategoryId == categoryId);
        }

        public async Task<TariffCategory?> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.Set<TariffCategory>().FindAsync(id);
            if (entity != null)
            {
                entity.Update(
                    entity.TariffCode,
                    entity.Name,
                    entity.TariffPercentage,
                    entity.ApplyITBIS,
                    entity.ApplyExciseTax,
                    entity.ExciseTaxPercentage,
                    false);

                await _context.SaveChangesAsync();
                return entity;
            }
            return null;
        }
    }
}