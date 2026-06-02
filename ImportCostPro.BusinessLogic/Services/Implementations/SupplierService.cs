using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Supplier;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateSupplierDto> _createValidator;
        private readonly IValidator<UpdateSupplierDto> _updateValidator;

        public SupplierService(
            IUnitOfWork unitOfWork,
            IValidator<CreateSupplierDto> createValidator,
            IValidator<UpdateSupplierDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto supplierCreateDto)
        {
            ArgumentNullException.ThrowIfNull(supplierCreateDto);
            await _createValidator.ValidateAndThrowAsync(supplierCreateDto);

            var newSupplier = new Supplier
            (
                supplierCreateDto.Name,
                supplierCreateDto.CountryId,
                supplierCreateDto.Telephone,
                supplierCreateDto.Email,
                supplierCreateDto.CurrencyId
            );

            await _unitOfWork.Suppliers.AddAsync(newSupplier);
            await _unitOfWork.CompleteAsync();

            // IQueryable en acción
            var supplierSaved = await _unitOfWork.Suppliers.AsQueryable()
                .Include(x => x.Country)
                .Include(x => x.Currency)
                .FirstOrDefaultAsync(x => x.Id == newSupplier.Id);

            if (supplierSaved == null)
                throw new KeyNotFoundException("The saved supplier cannot be recovered");

            return new SupplierDto
            {
                Id = supplierSaved.Id,
                Name = supplierSaved.Name,
                CountryId = supplierSaved.CountryId,
                CountryName = supplierSaved.Country.Name,
                Email = supplierSaved.Email,
                Telephone = supplierSaved.Telephone,
                CurrencyId = supplierSaved.CurrencyId,
                CurrencyName = supplierSaved.Currency.Name,
                IsActive = supplierSaved.IsActive

            };
        }

        public async Task<bool> DeleteSupplierAsync(Guid id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Supplier not found");

            bool isSoftDelete;
            if (await _unitOfWork.Suppliers.HasRelatedEntitiesAsync(id))
            {
                await _unitOfWork.Suppliers.SoftDeleteAsync(id);
                isSoftDelete = true;
            }
            else
            {
                await _unitOfWork.Suppliers.DeleteAsync(id);
                isSoftDelete = false;
            }
            await _unitOfWork.CompleteAsync();
            return isSoftDelete;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSupplierAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.AsQueryable()
                .Include(x => x.Country)
                .Include(x => x.Currency)
                .ToListAsync();

            var supplierDtos = suppliers
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CountryId = s.CountryId,
                    CountryName = s.Country.Name,
                    Email = s.Email,
                    Telephone = s.Telephone,
                    CurrencyId = s.CurrencyId,
                    CurrencyName = s.Currency.Name,
                    IsActive = s.IsActive
                }).ToList();
            return supplierDtos;
        }

        public async Task<SupplierDto?> GetSupplierByIdAsync(Guid id)
        {

            var supplier = await _unitOfWork.Suppliers.AsQueryable()
                .Include(x => x.Country)
                .Include(x => x.Currency)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (supplier == null ) return null;

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                CountryId = supplier.CountryId,
                CountryName = supplier.Country.Name,
                Email = supplier.Email,
                Telephone = supplier.Telephone,
                CurrencyId = supplier.CurrencyId,
                CurrencyName = supplier.Currency.Name,
                IsActive = supplier.IsActive
            };
        }

        public async Task<SupplierDto> UpdateSupplierAsync(Guid id, UpdateSupplierDto supplierUpdateDto)
        {
            ArgumentNullException.ThrowIfNull(supplierUpdateDto);
            await _updateValidator.ValidateAndThrowAsync(supplierUpdateDto);

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Supplier not found.");

            supplier.Update(
                supplierUpdateDto.Name.Trim(),
                supplierUpdateDto.CountryId,
                supplierUpdateDto.Email?.Trim(),
                supplierUpdateDto.Telephone?.Trim(),
                supplierUpdateDto.CurrencyId,
                supplierUpdateDto.IsActive
            );
            await _unitOfWork.CompleteAsync();
            // Deshabilitada porque al pasarle ocn Update, y luego hacer UpdateAsync, se hacia consulta dos veces...
            // await _unitOfWork.Countries.UpdateAsync(id, country);

            var supplierSaved = await _unitOfWork.Suppliers.AsQueryable()
               .Include(x => x.Country)
               .Include(x => x.Currency)
               .FirstOrDefaultAsync(x => x.Id == supplier.Id);

            if (supplierSaved == null)
                throw new KeyNotFoundException("The saved supplier cannot be recovered");

            return new SupplierDto
            {
                Id = supplierSaved.Id,
                Name = supplierSaved.Name,
                CountryId = supplierSaved.CountryId,
                CountryName = supplierSaved.Country.Name,
                Email = supplierSaved.Email,
                Telephone = supplierSaved.Telephone,
                CurrencyId = supplierSaved.CurrencyId,
                CurrencyName = supplierSaved.Currency.Name, 
                IsActive = supplierSaved.IsActive

            };
        }
    }
}