using ImportCostPro.BusinessLogic.DTOs.TaxConfiguration;

namespace ImportCostPro.BusinessLogic.Services.Interfaces
{
    public interface ITaxConfigurationService
    {
        Task<TaxConfigurationDto> GetCurrentAsync();
        Task<TaxConfigurationDto> UpdateAsync(UpdateTaxConfigurationDto dto);
    }
}