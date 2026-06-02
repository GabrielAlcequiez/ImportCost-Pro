using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<bool> ExistsByReferenceCodeAsync(string codeReference, Guid? excludeId = null)
        {
            var query = _context.Products.Where(p => p.CodeReference == codeReference);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasRelatedEntitiesAsync(Guid id)
        {
            return await _context.ImportOrderDetails.AnyAsync(o => o.ProductId == id);
        }

        public async Task<Product?> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.Set<Product>().FindAsync(id);
            if (entity != null)
            {
                entity.Update(
                    entity.Name,
                    entity.CodeReference,
                    entity.CountryId,
                    entity.TariffCategoryId,
                    entity.UnitWeight,
                    entity?.Length,
                    entity?.Width,
                    entity?.Height,
                    entity.UnitOfMeasure,
                    entity?.Description,
                    false);

                await _context.SaveChangesAsync();
                return entity;
            }
            return null;
        }
    }
}