using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.BusinessLogic.DTOs.Country;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _repository;

        public CountryService(ICountryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CountryDto>> GetAllCountriesAsync()
        {
            var countries = await _repository.GetAllAsync();
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
            var country = await _repository.GetByIdAsync(id);
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

            if (string.IsNullOrWhiteSpace(countryCreateDto.Name))
                throw new ArgumentException("Name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(countryCreateDto.ISOCode))
                throw new ArgumentException("ISOCode cannot be null or empty.");

            var nameTrimmed = countryCreateDto.Name.Trim();
            var isoCodeNormalized = countryCreateDto.ISOCode.Trim().ToUpperInvariant();

            if (isoCodeNormalized.Length < 2 || isoCodeNormalized.Length > 3)
                throw new ArgumentException("ISOCode must be 2 or 3 characters long.");

            // if (await _repository.GetAllAsync(c => c.Name.ToLower() == nameTrimmed.ToLower()))
            //     throw new InvalidOperationException("A country with the same name already exists.");

            // if (await _context.Countries.AnyAsync(c => c.ISOCode == isoCodeNormalized))
            //     throw new InvalidOperationException("A country with the same ISO code already exists.");

            var newCountry = new Country(nameTrimmed, isoCodeNormalized);
            await _repository.AddAsync(newCountry);

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

            var country = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Country not found.");

            if (string.IsNullOrWhiteSpace(countryUpdateDto.Name))
                throw new ArgumentException("Name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(countryUpdateDto.ISOCode))
                throw new ArgumentException("ISOCode cannot be null or empty.");

            var nameTrimmed = countryUpdateDto.Name.Trim();
            var isoCodeNormalized = countryUpdateDto.ISOCode.Trim().ToUpperInvariant();

            if (isoCodeNormalized.Length < 2 || isoCodeNormalized.Length > 3)
                throw new ArgumentException("ISOCode must be 2 or 3 characters long.");

            // // Check uniqueness of Name excluding current record
            // if (await _context.Countries.AnyAsync(c => c.Id != id && c.Name.ToLower() == nameTrimmed.ToLower()))
            //     throw new InvalidOperationException("A country with the same name already exists.");

            // // Check uniqueness of ISO code excluding current record
            // if (await _context.Countries.AnyAsync(c => c.Id != id && c.ISOCode == isoCodeNormalized))
            //     throw new InvalidOperationException("A country with the same ISO code already exists.");


            country.Update(
                countryUpdateDto.Name.Trim(), 
                countryUpdateDto.ISOCode.Trim().ToUpperInvariant(), 
                country.IsActive);

            await _repository.UpdateAsync(id, country);
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
            var country = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Country not found.");


            if(await _repository.HasRelatedEntitiesAsync(id))
            {
                await _repository.SoftDeleteAsync(id);
                return false; // por ahora siempre se desactivará, chequear despues
            }
            else
            {
                await _repository.DeleteAsync(id);
                return true;
            }
        }
    }
}