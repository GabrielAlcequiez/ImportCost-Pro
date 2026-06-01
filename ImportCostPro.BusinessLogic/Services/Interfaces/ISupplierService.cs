using ImportCostPro.BusinessLogic.DTOs.Supplier;

namespace ImportCostPro.BusinessLogic.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDto>> GetAllSupplierAsync();
        Task<SupplierDto?> GetSupplierByIdAsync(Guid id);
        Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto supplierCreateDto);
        Task<SupplierDto> UpdateSupplierAsync(Guid id, UpdateSupplierDto supplierUpdateDto);
        Task<bool> DeleteSupplierAsync(Guid id);        
    }
}