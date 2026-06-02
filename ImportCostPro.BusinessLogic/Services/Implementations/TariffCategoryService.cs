using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.TariffCategory;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class TariffCategoryService : ITariffCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateTariffCategoryDto> _createValidator;
        private readonly IValidator<UpdateTariffCategoryDto> _updateValidator;

        public TariffCategoryService(
            IUnitOfWork unitOfWork,
            IValidator<CreateTariffCategoryDto> createValidator,
            IValidator<UpdateTariffCategoryDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<TariffCategoryDto> CreateTariffCategoryAsync(CreateTariffCategoryDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto);
            await _createValidator.ValidateAndThrowAsync(createDto);

            var newCategory = new TariffCategory
            (
                createDto.TariffCode.Trim(),
                createDto.Name.Trim(),
                createDto.TariffPercentage,
                createDto.ApplyITBIS, 
                createDto.ApplyExciseTax,
                createDto.ExciseTaxPercentage
            );

            await _unitOfWork.TariffCategories.AddAsync(newCategory);
            await _unitOfWork.CompleteAsync();

            var categorySaved = await _unitOfWork.TariffCategories.AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == newCategory.Id);

            if (categorySaved == null)
                throw new KeyNotFoundException("The saved tariff category cannot be recovered.");

            // Mapeo manual
            return new TariffCategoryDto
            {
                Id = categorySaved.Id,
                TariffCode = categorySaved.TariffCode,
                Name = categorySaved.Name,
                TariffPercentage = categorySaved.TariffPercentage,
                ApplyITBIS = categorySaved.ApplyITBIS, 
                ApplyExciseTax = categorySaved.ApplyExciseTax,
                ExciseTaxPercentage = categorySaved.ExciseTaxPercentage,
                IsActive = categorySaved.IsActive
            };
        }

        public async Task<bool> DeleteTariffCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.TariffCategories.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Tariff category not found.");

            bool isSoftDelete;
            
            if (await _unitOfWork.TariffCategories.HasRelatedEntitiesAsync(id))
            {
                await _unitOfWork.TariffCategories.SoftDeleteAsync(id);
                isSoftDelete = true;
            }
            else
            {
                await _unitOfWork.TariffCategories.DeleteAsync(id);
                isSoftDelete = false;
            }
            
            await _unitOfWork.CompleteAsync();
            return isSoftDelete;
        }

        public async Task<IEnumerable<TariffCategoryDto>> GetAllTariffCategoriesAsync()
        {
            var categories = await _unitOfWork.TariffCategories.AsQueryable()
                .ToListAsync();

            // Mapeo manual con Select
            var categoryDtos = categories.Select(c => new TariffCategoryDto
            {
                Id = c.Id,
                TariffCode = c.TariffCode,
                Name = c.Name,
                TariffPercentage = c.TariffPercentage,
                ApplyITBIS = c.ApplyITBIS,
                ApplyExciseTax = c.ApplyExciseTax,
                ExciseTaxPercentage = c.ExciseTaxPercentage,
                IsActive = c.IsActive
            }).ToList();

            return categoryDtos;
        }

        public async Task<TariffCategoryDto?> GetTariffCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.TariffCategories.AsQueryable()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return null;

            // Mapeo manual
            return new TariffCategoryDto
            {
                Id = category.Id,
                TariffCode = category.TariffCode,
                Name = category.Name,
                TariffPercentage = category.TariffPercentage,
                ApplyITBIS = category.ApplyITBIS,
                ApplyExciseTax = category.ApplyExciseTax,
                ExciseTaxPercentage = category.ExciseTaxPercentage,
                IsActive = category.IsActive
            };
        }

        public async Task<TariffCategoryDto> UpdateTariffCategoryAsync(Guid id, UpdateTariffCategoryDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto);
            await _updateValidator.ValidateAndThrowAsync(updateDto);

            var category = await _unitOfWork.TariffCategories.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Tariff category not found.");

            category.Update(
                updateDto.TariffCode.Trim(),
                updateDto.Name.Trim(),
                updateDto.TariffPercentage,
                updateDto.ApplyITBIS,
                updateDto.ApplyExciseTax,
                updateDto.ExciseTaxPercentage,
                updateDto.IsActive
            );
            
            await _unitOfWork.CompleteAsync();

            var categorySaved = await _unitOfWork.TariffCategories.AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == category.Id);

            if (categorySaved == null)
                throw new KeyNotFoundException("The saved tariff category cannot be recovered.");

            // Mapeo manual
            return new TariffCategoryDto
            {
                Id = categorySaved.Id,
                TariffCode = categorySaved.TariffCode,
                Name = categorySaved.Name,
                TariffPercentage = categorySaved.TariffPercentage,
                ApplyITBIS = categorySaved.ApplyITBIS,
                ApplyExciseTax = categorySaved.ApplyExciseTax,
                ExciseTaxPercentage = categorySaved.ExciseTaxPercentage,
                IsActive = categorySaved.IsActive
            };
        }
    }
}