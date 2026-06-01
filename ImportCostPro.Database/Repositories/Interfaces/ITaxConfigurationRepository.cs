using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.Database.Entities;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface ITaxConfigurationRepository : IBaseRepository<TaxConfiguration>
    {
        Task<TaxConfiguration?> GetCurrentAsync();
    }
}