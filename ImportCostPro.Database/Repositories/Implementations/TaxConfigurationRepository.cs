using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class TaxConfigurationRepository(AppDbContext context) : BaseRepository<TaxConfiguration>(context), ITaxConfigurationRepository
    {

        //Get the current tax configuration, there should be only one configuration in the database
        public async Task<TaxConfiguration?> GetCurrentAsync()
        {
            return await _context.TaxConfigurations.FirstOrDefaultAsync();
        }
    }
}