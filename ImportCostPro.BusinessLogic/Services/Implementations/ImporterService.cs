using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Importer;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

            var importerSaved = await _unitOfWork.Importers.AsQueryable()
                .Include(x => x.Country)
                .FirstOrDefaultAsync(x => x.Id == importer.Id);

            if (importerSaved == null)
                throw new KeyNotFoundException("The saved importer cannot be recovered");

            return new ImporterDto
            {
                Id = importerSaved.Id,
                Name = importerSaved.Name,
                TaxId = importerSaved.TaxId,
                CountryId = importerSaved.CountryId,
                CountryName = importerSaved.Country.Name,
                Phone = importerSaved.Phone,
                Email = importerSaved.Email,
                Address = importerSaved.Address,
                IsActive = importerSaved.IsActive
            };

        }

        public async Task<bool> DeleteImporterAsync(Guid id)
        {
            var importer = await _unitOfWork.Importers.GetByIdAsync(id)
                 ?? throw new KeyNotFoundException("Importer not found.");

            bool isSoftDelete;
            if (await _unitOfWork.Importers.HasRelatedEntitiesAsync(id))
            {
                await _unitOfWork.Importers.SoftDeleteAsync(id);
                isSoftDelete = true;
            }
            else
            {
                await _unitOfWork.Importers.DeleteAsync(id);
                isSoftDelete = false;
            }
            await _unitOfWork.CompleteAsync();
            return isSoftDelete;
        }

        public async Task<List<ImporterDto>> GetAllImportersAsync()
        {
            var importers = await _unitOfWork.Importers.AsQueryable()
                .Include(x => x.Country)
                .ToListAsync();

            return importers.Select(i => new ImporterDto
            {
                Id = i.Id,
                Name = i.Name,
                TaxId = i.TaxId,
                CountryId = i.CountryId,
                CountryName = i.Country.Name,
                Phone = i.Phone,
                Email = i.Email,
                Address = i.Address,
                IsActive = i.IsActive
            }).ToList();
        }

        public async Task<ImporterDto?> GetImporterByIdAsync(Guid id)
        {
            var importer = await _unitOfWork.Importers.AsQueryable()
                .Include(x => x.Country)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (importer == null) return null;
            return new ImporterDto
            {
                Id = importer.Id,
                Name = importer.Name,
                TaxId = importer.TaxId,
                CountryId = importer.CountryId,
                CountryName = importer.Country.Name,
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

            var importerSaved = await _unitOfWork.Importers.AsQueryable()
                .Include(x => x.Country)
                .FirstOrDefaultAsync(x => x.Id == importer.Id);

            if (importerSaved == null)
                throw new KeyNotFoundException("The saved importer cannot be recovered");

            return new ImporterDto
            {
                Id = importerSaved.Id,
                Name = importerSaved.Name,
                TaxId = importerSaved.TaxId,
                CountryId = importerSaved.CountryId,
                CountryName = importerSaved.Country.Name,
                Phone = importerSaved.Phone,
                Email = importerSaved.Email,
                Address = importerSaved.Address,
                IsActive = importerSaved.IsActive
            };

        }
    }
}
