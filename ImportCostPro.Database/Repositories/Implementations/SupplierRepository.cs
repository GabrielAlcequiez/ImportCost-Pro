using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var query = _context.Suppliers.Where(c => c.Name == name);
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
            return await query.AnyAsync();
        }


        public async Task<bool> HasRelatedEntitiesAsync(Guid id)
        {
            return await _context.ImportOrders.AnyAsync(o => o.SupplierId == id);
        }

        public async Task<Supplier?> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.Set<Supplier>().FindAsync(id);
            if (entity != null)
            {
                entity.Update(
                    entity.Name, 
                    entity.CountryId, 
                    entity.Telephone, 
                    entity.Email, 
                    entity.CurrencyId, 
                    false);

                await _context.SaveChangesAsync();
                return entity;
            }
            return null;
        }

    }
}