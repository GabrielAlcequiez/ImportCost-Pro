using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class ImporterRepository(AppDbContext context) : BaseRepository<Importer>(context), IImporterRepository
    {
        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var querty = _context.Importers.Where(i => i.Name.ToLower() == name.ToLower());
            if (excludeId.HasValue)
            {
                querty = querty.Where(i => i.Id != excludeId.Value);
            }
            return await querty.AnyAsync();
 }

        public async Task<bool> ExistsByTaxIdAsync(string taxId, Guid? excludeId = null)
        {
            var query = _context.Importers.Where(i => i.TaxId == taxId);
            if (excludeId.HasValue)
            {
                query = query.Where(i => i.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> HasRelatedEntitiesAsync(Guid id)
        {
            return await _context.ImportOrders.AnyAsync(o => o.ImporterId == id);
        }

        public async Task<Importer?> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.Set<Importer>().FindAsync(id);
            if (entity != null)
            {
                entity.Update(entity.Name, entity.TaxId, entity.CountryId,
                              entity.Phone, entity.Email, entity.Address, false);
                return entity;
            }
            return null;
        }
    }
}