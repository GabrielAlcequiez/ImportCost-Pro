using ImportCostPro.BusinessLogic.DTOs.LandedCostCalculation;

namespace ImportCostPro.BusinessLogic.Services.Interfaces
{
    public interface ILandedCostCalculationService
    {
        Task<List<LandedCostCalculationDto>> GetAllByOrderIdAsync(Guid importOrderId);
        Task<LandedCostCalculationDto?> GetByIdAsync(Guid id);
        Task<LandedCostCalculationDto> CalculateAsync(Guid importOrderId);
    }
}