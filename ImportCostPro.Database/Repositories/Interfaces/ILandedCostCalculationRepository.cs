using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.Database.Entities;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface ILandedCostCalculationRepository : IBaseRepository<LandedCostCalculation>
    {
        Task<IReadOnlyList<LandedCostCalculation>> GetAllByOrderIdAsync(Guid importOrderId);
        Task<LandedCostCalculation?> GetByIdWithDetailsAsync(Guid id);
        Task<LandedCostCalculation?> GetLatestByOrderIdAsync(Guid importOrderId);
    }
}