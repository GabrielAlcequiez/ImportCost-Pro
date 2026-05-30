using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class CountryRepository : BaseRepository<Country>, ICountryRepository
    {
        public CountryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> HasRelatedEntitiesAsync(Guid id)
        {
            return await _context.Suppliers.AnyAsync(s => s.CountryId == id) ||
                await _context.Importers.AnyAsync(i => i.CountryId == id) ||
                await _context.Products.AnyAsync(p => p.CountryId == id) ||
                await _context.ImportOrders.AnyAsync(o => o.CountryId == id);
        }

        public async Task<Country?> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.Set<Country>().FindAsync(id);
            if (entity != null)
            {
                entity.Update(entity.Name, entity.ISOCode, false);
                await _context.SaveChangesAsync();
                return entity;
            }
            return null;
        }
    }

}