using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Country;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateCountryDto> _createValidator;
        private readonly IValidator<UpdateCountryDto> _updateValidator;

        public CountryService(
            IUnitOfWork unitOfWork,
            IValidator<CreateCountryDto> createValidator,
            IValidator<UpdateCountryDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<List<CountryDto>> GetAllCountriesAsync()
        {
            var countries = await _unitOfWork.Countries.GetAllAsync();
            var countryDtos = new List<CountryDto>();

            foreach (var country in countries)
            {
                countryDtos.Add(new CountryDto
                {
                    Id = country.Id,
                    Name = country.Name,
                    ISOCode = country.ISOCode,
                    IsActive = country.IsActive
                });
            }
            return countryDtos;
        }

        public async Task<CountryDto?> GetCountryByIdAsync(Guid id)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(id);
            if (country == null) return null;

            return new CountryDto
            {
                Id = country.Id,
                Name = country.Name,
                ISOCode = country.ISOCode,
                IsActive = country.IsActive
            };
        }

        public async Task<CountryDto> CreateCountryAsync(CreateCountryDto countryCreateDto)
        {
            ArgumentNullException.ThrowIfNull(countryCreateDto);
            await _createValidator.ValidateAndThrowAsync(countryCreateDto);

            var nameTrimmed = countryCreateDto.Name.Trim();
            var isoCodeNormalized = countryCreateDto.ISOCode.Trim().ToUpperInvariant();

            var newCountry = new Country(nameTrimmed, isoCodeNormalized);
            await _unitOfWork.Countries.AddAsync(newCountry);
            await _unitOfWork.CompleteAsync();

            return new CountryDto
            {
                Id = newCountry.Id,
                Name = newCountry.Name,
                ISOCode = newCountry.ISOCode,
                IsActive = newCountry.IsActive
            };
        }

        public async Task<CountryDto> UpdateCountryAsync(Guid id, UpdateCountryDto countryUpdateDto)
        {
            ArgumentNullException.ThrowIfNull(countryUpdateDto);
            await _updateValidator.ValidateAndThrowAsync(countryUpdateDto);

            var country = await _unitOfWork.Countries.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Country not found.");

            var nameTrimmed = countryUpdateDto.Name.Trim();
            var isoCodeNormalized = countryUpdateDto.ISOCode.Trim().ToUpperInvariant();

            country.Update(
                countryUpdateDto.Name.Trim(),
                countryUpdateDto.ISOCode.Trim().ToUpperInvariant(),
                country.IsActive);
            await _unitOfWork.CompleteAsync();
            // Deshabilitada porque al pasarle ocn Update, y luego hacer UpdateAsync, se hacia consulta dos veces...
            // await _unitOfWork.Countries.UpdateAsync(id, country);
            return new CountryDto
            {
                Id = country.Id,
                Name = country.Name,
                ISOCode = country.ISOCode,
                IsActive = country.IsActive
            };
        }

        public async Task<bool> DeleteCountryAsync(Guid id)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Country not found.");

            bool isSoftDelete;
            if (await _unitOfWork.Countries.HasRelatedEntitiesAsync(id))
            {
                await _unitOfWork.Countries.SoftDeleteAsync(id);
                isSoftDelete = true; // por ahora siempre se desactivará, chequear despues
            }
            else
            {
                await _unitOfWork.Countries.DeleteAsync(id);
                isSoftDelete = false;
            }

            await _unitOfWork.CompleteAsync();
            return isSoftDelete;
        }
    }
}