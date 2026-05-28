using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImportCostPro.BusinessLogic.DTOs.Currency;

namespace ImportCostPro.BusinessLogic.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<List<CurrencyDto>> GetAllCurrenciesAsync();
        Task<CurrencyDto?> GetCurrencyByIdAsync(Guid id);
        Task<CurrencyDto> CreateCurrencyAsync(CreateCurrencyDto currencyCreateDto);
        Task<CurrencyDto> UpdateCurrencyAsync(Guid id, UpdateCurrencyDto currencyUpdateDto);
        Task<bool> DeleteCurrencyAsync(Guid id);
    }
}