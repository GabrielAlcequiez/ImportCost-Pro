using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.BusinessLogic.DTOs.Country;

namespace ImportCostPro.BusinessLogic.Services.Interfaces
{
    public interface ICountryService
    {
        Task<List<CountryDto>> GetAllCountriesAsync();
        Task<CountryDto?> GetCountryByIdAsync(Guid id);
        Task<CountryDto> CreateCountryAsync(CreateCountryDto countryCreateDto);
        Task<CountryDto> UpdateCountryAsync(Guid id, UpdateCountryDto countryUpdateDto);
        Task<bool> DeleteCountryAsync(Guid id);
    }
}