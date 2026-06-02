using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{

    public class LandedCostCalculationRepository(AppDbContext context) : BaseRepository<LandedCostCalculation>(context), ILandedCostCalculationRepository
    {
        public async Task<IReadOnlyList<LandedCostCalculation>> GetAllByOrderIdAsync(Guid importOrderId)
        {
            return await _context.LandedCostCalculations.Where(c => c.ImportOrderId == importOrderId)
             .Include(c => c.ImportOrder)
             .Include(c => c.LocalCurrency)
             .Include(c => c.Details).ThenInclude(d => d.Product)
             .AsNoTracking()
             .OrderByDescending(c => c.CalculationDate)
             .ToListAsync();
        }

        public async Task<LandedCostCalculation?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.LandedCostCalculations
            .Include(c => c.ImportOrder)
            .Include(c => c.LocalCurrency)
            .Include(c => c.Details)
                .ThenInclude(d => d.Product)
                    .ThenInclude(p => p.TariffCategory)
            .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<LandedCostCalculation?> GetLatestByOrderIdAsync(Guid importOrderId)
        {
            return await _context.LandedCostCalculations
            .Where(c => c.ImportOrderId == importOrderId)
            .Include(c => c.LocalCurrency)
            .Include(c => c.Details).ThenInclude(d => d.Product)
            .OrderByDescending(c => c.CalculationDate)
            .FirstOrDefaultAsync();
        }
    }
}