using ImportCostPro.BusinessLogic.DTOs.Product;

namespace ImportCostPro.BusinessLogic.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductAsync();
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<ProductDto> CreateProductAsync(CreateProductDto producCreateDto);
        Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto productUpdateDto);
        Task<bool> DeleteProductAsync(Guid id); 
    }
}