using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Product;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDto> _updateValidator;

        public ProductService(
            IUnitOfWork unitOfWork,
            IValidator<CreateProductDto> createValidator,
            IValidator<UpdateProductDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto productCreateDto)
        {
            ArgumentNullException.ThrowIfNull(productCreateDto);
            await _createValidator.ValidateAndThrowAsync(productCreateDto);

            var newProduct = new Product
            (
                productCreateDto.Name.Trim(),
                productCreateDto.CodeReference.Trim(),
                productCreateDto.CountryId,
                productCreateDto.TariffCategoryId,
                productCreateDto.UnitWeight,
                productCreateDto.Length,
                productCreateDto.Width,
                productCreateDto.Height,
                productCreateDto.UnitOfMeasure,
                productCreateDto.Description?.Trim()
            );

            await _unitOfWork.Products.AddAsync(newProduct);
            await _unitOfWork.CompleteAsync();

            // IQueryable en acción para traer nombres relacionados (País y Categoría Arancelaria)
            var productSaved = await _unitOfWork.Products.AsQueryable()
                .Include(x => x.Country)
                .Include(x => x.TariffCategory) // ◄ Ya integrado
                .FirstOrDefaultAsync(x => x.Id == newProduct.Id);

            if (productSaved == null)
                throw new KeyNotFoundException("The saved product cannot be recovered.");

            return MapToDto(productSaved);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Product not found.");

            bool isSoftDelete;
            
            if (await _unitOfWork.Products.HasRelatedEntitiesAsync(id))
            {
                await _unitOfWork.Products.SoftDeleteAsync(id);
                isSoftDelete = true;
            }
            else
            {
                await _unitOfWork.Products.DeleteAsync(id);
                isSoftDelete = false;
            }
            
            await _unitOfWork.CompleteAsync();
            return isSoftDelete;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductAsync()
        {
            var products = await _unitOfWork.Products.AsQueryable()
                .Include(x => x.Country)
                .Include(x => x.TariffCategory)
                .ToListAsync();

            var productDtos = products.Select(p => MapToDto(p)).ToList();
            return productDtos;
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.AsQueryable()
                .Include(x => x.Country)
                .Include(x => x.TariffCategory) 
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null) return null;

            return MapToDto(product);
        }

        public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto productUpdateDto)
        {
            ArgumentNullException.ThrowIfNull(productUpdateDto);
            await _updateValidator.ValidateAndThrowAsync(productUpdateDto);

            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Product not found.");

            product.Update(
                productUpdateDto.Name.Trim(),
                productUpdateDto.CodeReference.Trim(),
                productUpdateDto.CountryId,
                productUpdateDto.TariffCategoryId,
                productUpdateDto.UnitWeight,
                productUpdateDto.Length,
                productUpdateDto.Width,
                productUpdateDto.Height,
                productUpdateDto.UnitOfMeasure,
                productUpdateDto.Description?.Trim(),
                productUpdateDto.IsActive
            );
            
            await _unitOfWork.CompleteAsync();

            var productSaved = await _unitOfWork.Products.AsQueryable()
               .Include(x => x.Country)
               .Include(x => x.TariffCategory) 
               .FirstOrDefaultAsync(x => x.Id == product.Id);

            if (productSaved == null)
                throw new KeyNotFoundException("The saved product cannot be recovered.");

            return MapToDto(productSaved);
        }

        // SALIA MEJOR USAR AUTOMAPPER PERO BUENO, ES TEMPORAL
        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                CodeReference = product.CodeReference,
                CountryId = product.CountryId,
                CountryName = product.Country?.Name ?? "N/A",
                TariffCategoryId = product.TariffCategoryId, 
                TariffCategoryName = product.TariffCategory?.Name ?? "N/A", 
                UnitWeight = product.UnitWeight,
                Length = product.Length,
                Width = product.Width,
                Height = product.Height,
                UnitOfMeasure = product.UnitOfMeasure,
                Description = product.Description,
                IsActive = product.IsActive
            };
        }
    }
}