using ImportCostPro.BusinessLogic.DTOs.Importer;

namespace ImportCostPro.BusinessLogic.Services.Interfaces
{
    public interface IImporterService
    {
        Task<List<ImporterDto>> GetAllImportersAsync();
        Task<ImporterDto?> GetImporterByIdAsync(Guid id);
        Task<ImporterDto> CreateImporterAsync(CreateImporterDto dto);
        Task<ImporterDto> UpdateImporterAsync(Guid id, UpdateImporterDto dto);
        Task<bool> DeleteImporterAsync(Guid id);
    }
}