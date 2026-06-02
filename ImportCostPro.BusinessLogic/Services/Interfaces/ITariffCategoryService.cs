using ImportCostPro.BusinessLogic.DTOs.TariffCategory;

namespace ImportCostPro.BusinessLogic.Services.Interfaces
{
    public interface ITariffCategoryService
    {
        Task<TariffCategoryDto> CreateTariffCategoryAsync(CreateTariffCategoryDto createDto);
        Task<bool> DeleteTariffCategoryAsync(Guid id);
        Task<IEnumerable<TariffCategoryDto>> GetAllTariffCategoriesAsync();
        Task<TariffCategoryDto?> GetTariffCategoryByIdAsync(Guid id);
        Task<TariffCategoryDto> UpdateTariffCategoryAsync(Guid id, UpdateTariffCategoryDto updateDto);

    }
}