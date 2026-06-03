using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Importer;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class ImporterService : IImporterService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateImporterDto> _createValidator;
        private readonly IValidator<UpdateImporterDto> _updateValidator;

        public ImporterService(IUnitOfWork unitOfWork, IValidator<CreateImporterDto> createValidator, IValidator<UpdateImporterDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<ImporterDto> CreateImporterAsync(CreateImporterDto dto)
        {
           ArgumentNullException.ThrowIfNull(dto);
            await _createValidator.ValidateAndThrowAsync(dto);

            var importer = new Database.Entities.Importer(
                dto.Name.Trim(),
                dto.TaxId.Trim(),
                dto.CountryId,
                dto.Phone?.Trim(),
                dto.Email?.Trim(),
                dto.Address?.Trim());

                await _unitOfWork.Importers.AddAsync(importer);
                await _unitOfWork.CompleteAsync();

            return new ImporterDto
            {
                Id = importer.Id,
                Name = importer.Name,
                TaxId = importer.TaxId,
                CountryId = importer.CountryId,
                Phone = importer.Phone,
                Email = importer.Email,
                Address = importer.Address,
                IsActive = importer.IsActive
            };

        }

        public async Task<bool> DeleteImporterAsync(Guid id)
        {
            var importer = await _unitOfWork.Importers.GetByIdAsync(id)
                 ?? throw new KeyNotFoundException("Importer not found.");

            if (await _unitOfWork.Importers.HasRelatedEntitiesAsync(id))
            {
                await _unitOfWork.Importers.SoftDeleteAsync(id);
            }
            else
            {
                await _unitOfWork.Importers.DeleteAsync(id);
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<List<ImporterDto>> GetAllImportersAsync()
        {
            var importers = await _unitOfWork.Importers.GetAllAsync();
            var dtos = new List<ImporterDto>();
            foreach (var importer in importers)
            {
                dtos.Add(new ImporterDto
                {
                    Id = importer.Id,
                    Name = importer.Name,
                    TaxId = importer.TaxId,
                    CountryId = importer.CountryId,
            CountryName = importer.Country?.Name ?? string.Empty,
                    Phone = importer.Phone,
                    Email = importer.Email,
                    Address = importer.Address,
                    IsActive = importer.IsActive
                });
            }

            return dtos;
        }

        public async Task<ImporterDto?> GetImporterByIdAsync(Guid id)
        {
            var importer = await _unitOfWork.Importers.GetByIdAsync(id);
            if (importer == null) return null;
            return new ImporterDto
            {
                Id = importer.Id,
                Name = importer.Name,
                TaxId = importer.TaxId,
                CountryId = importer.CountryId,
                CountryName = importer.Country?.Name ?? string.Empty,
                Phone = importer.Phone,
                Email = importer.Email,
                Address = importer.Address,
                IsActive = importer.IsActive
            };
        }

        public async Task<ImporterDto> UpdateImporterAsync(Guid id, UpdateImporterDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await _updateValidator.ValidateAndThrowAsync(dto);

            var importer = await _unitOfWork.Importers.GetByIdAsync(id) ?? throw new KeyNotFoundException("Importer not found");
            importer.Update(
                 dto.Name.Trim(),
                 dto.TaxId.Trim(),
                 dto.CountryId,
                 dto.Phone?.Trim(),
                 dto.Email?.Trim(),
                 dto.Address?.Trim(),
                 dto.IsActive);
            await _unitOfWork.CompleteAsync();

            return new ImporterDto
            {
                Id = importer.Id,
                Name = importer.Name,
                TaxId = importer.TaxId,
                CountryId = importer.CountryId,
                CountryName = importer.Country?.Name ?? string.Empty,
                Phone = importer.Phone,
                Email = importer.Email,
                Address = importer.Address,
                IsActive = importer.IsActive
            };

        }
    }
}
