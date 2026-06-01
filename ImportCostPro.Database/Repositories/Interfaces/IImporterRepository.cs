using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.Database.Entities;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface IImporterRepository : IBaseRepository<Importer>
    {
        Task<bool> HasRelatedEntitiesAsync(Guid id);
        Task<Importer?> SoftDeleteAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
        Task<bool> ExistsByTaxIdAsync(string taxId, Guid? excludeId = null);
    }
}