using ImportCostPro.BusinessLogic.DTOs.ExchangeRate;

namespace ImportCostPro.BusinessLogic.Services.Interfaces
{
    public interface IExchangeRateService
    {
        Task<IEnumerable<ExchangeRateDto>> GetAllExchangeRatesAsync();
        Task<ExchangeRateDto?> GetExchangeRatesByIdAsync(Guid id);
        Task<ExchangeRateDto> CreateExchangeRatesAsync(CreateExchangeRateDto exchangeCreateDto);
        Task<ExchangeRateDto> UpdateExchangeRatesAsync(Guid id, UpdateExchangeRateDto exchangeUpdateDto);
        Task<bool> DeleteExchangeRateAsync(Guid id);
    }
}