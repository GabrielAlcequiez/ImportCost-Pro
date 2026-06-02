using System.Runtime;
using System.Runtime.CompilerServices;
using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.ExchangeRate;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class ExchangeRateService : IExchangeRateService
    {
          private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateExchangeRateDto> _createValidator;
        private readonly IValidator<UpdateExchangeRateDto> _updateValidator;
        public ExchangeRateService(
            IUnitOfWork unitOfWork,
            IValidator<CreateExchangeRateDto> createValidator,
            IValidator<UpdateExchangeRateDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        public async Task<ExchangeRateDto> CreateExchangeRatesAsync(CreateExchangeRateDto exchangeCreateDto)
        {
            ArgumentNullException.ThrowIfNull(exchangeCreateDto);
            await _createValidator.ValidateAndThrowAsync(exchangeCreateDto);

            var newExchange = new ExchangeRate(
                exchangeCreateDto.SourceCurrencyId,
                exchangeCreateDto.TargetCurrencyId,
                exchangeCreateDto.RateValue,
                exchangeCreateDto.EffectiveDate
            );

            await _unitOfWork.ExchangeRates.AddAsync(newExchange);
            await _unitOfWork.CompleteAsync();

            var exchangeSaved = await _unitOfWork.ExchangeRates.AsQueryable()
                .Include(x => x.SourceCurrency)
                .Include(x => x.TargetCurrency)
                .FirstOrDefaultAsync(x => x.Id == newExchange.Id);
            
            if(exchangeSaved == null) throw new KeyNotFoundException("The saved exchange rate cannot be recovered");
            return new ExchangeRateDto{
                Id = exchangeSaved.Id,
                SourceCurrencyId = exchangeSaved.SourceCurrencyId,
                SourceCurrencyCode = exchangeSaved.SourceCurrency?.ISOCode ?? string.Empty,
                SourceCurrencyName = exchangeSaved.SourceCurrency?.Name ?? string.Empty,
                TargetCurrencyId = exchangeSaved.TargetCurrencyId,
                TargetCurrencyCode = exchangeSaved.TargetCurrency?.ISOCode ?? string.Empty,
                TargetCurrencyName = exchangeSaved.TargetCurrency?.Name ?? string.Empty,
                RateValue = exchangeSaved.RateValue,
                EffectiveDate = exchangeSaved.EffectiveDate,
                IsActive = exchangeSaved.IsActive
            };
        }

        public async Task<bool> DeleteExchangeRateAsync(Guid id)
        {
            var exchange = await _unitOfWork.ExchangeRates.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Exchange rate not found");

            bool isSoftDelete;
            if (await _unitOfWork.ExchangeRates.HasRelatedEntitiesAsync(id))
            {
                await _unitOfWork.ExchangeRates.SoftDeleteAsync(id);
                isSoftDelete = true;
            }
            else
            {
                await _unitOfWork.ExchangeRates.DeleteAsync(id);
                isSoftDelete = false;
            }
            await _unitOfWork.CompleteAsync();
            return isSoftDelete;
        }

        public async Task<IEnumerable<ExchangeRateDto>> GetAllExchangeRatesAsync()
        {
             var exchanges = await _unitOfWork.ExchangeRates.AsQueryable()
                .Include(x => x.SourceCurrency)
                .Include(x => x.TargetCurrency)
                .OrderByDescending(x => x.EffectiveDate)
                .ToListAsync();
            
            var exchangeDtos = exchanges
                .Select(s => new ExchangeRateDto{
                Id = s.Id,
                SourceCurrencyId = s.SourceCurrencyId,
                SourceCurrencyCode = s.SourceCurrency?.ISOCode ?? string.Empty,
                SourceCurrencyName = s.SourceCurrency?.Name ?? string.Empty,
                TargetCurrencyId = s.TargetCurrencyId,
                TargetCurrencyCode = s.TargetCurrency?.ISOCode ?? string.Empty,
                TargetCurrencyName = s.TargetCurrency?.Name ?? string.Empty,
                RateValue = s.RateValue,
                EffectiveDate = s.EffectiveDate,
                IsActive = s.IsActive
            }).ToList();

            return exchangeDtos;
        }

        public async Task<ExchangeRateDto?> GetExchangeRatesByIdAsync(Guid id)
        {
            var exchange = await _unitOfWork.ExchangeRates.AsQueryable()
                .Include(x => x.SourceCurrency)
                .Include(x => x.TargetCurrency)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (exchange == null) return null;

             return new ExchangeRateDto{
                Id = exchange.Id,
                SourceCurrencyId = exchange.SourceCurrencyId,
                SourceCurrencyCode = exchange.SourceCurrency?.ISOCode ?? string.Empty,
                SourceCurrencyName = exchange.SourceCurrency?.Name ?? string.Empty,
                TargetCurrencyId = exchange.TargetCurrencyId,
                TargetCurrencyCode = exchange.TargetCurrency?.ISOCode ?? string.Empty,
                TargetCurrencyName = exchange.TargetCurrency?.Name ?? string.Empty,
                RateValue = exchange.RateValue,
                EffectiveDate = exchange.EffectiveDate,
                IsActive = exchange.IsActive
            };
        }

        public async Task<ExchangeRateDto> UpdateExchangeRatesAsync(Guid id, UpdateExchangeRateDto exchangeUpdateDto)
        {
            ArgumentNullException.ThrowIfNull(exchangeUpdateDto);
            await _updateValidator.ValidateAndThrowAsync(exchangeUpdateDto);

            var rate = await _unitOfWork.ExchangeRates.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Exchange rate not found.");

            rate.Update(
                exchangeUpdateDto.RateValue,
                exchangeUpdateDto.EffectiveDate,
                exchangeUpdateDto.IsActive
            );
            
            await _unitOfWork.CompleteAsync();

            var rateSaved = await _unitOfWork.ExchangeRates.AsQueryable()
                .Include(x => x.SourceCurrency)
                .Include(x => x.TargetCurrency)
                .FirstOrDefaultAsync(x => x.Id == rate.Id);

            if (rateSaved == null)
                throw new KeyNotFoundException("The saved exchange rate cannot be recovered.");

            return new ExchangeRateDto
            {
                Id = rateSaved.Id,
                SourceCurrencyId = rateSaved.SourceCurrencyId,
                SourceCurrencyCode = rateSaved.SourceCurrency?.ISOCode ?? string.Empty,
                SourceCurrencyName = rateSaved.SourceCurrency?.Name ?? string.Empty,
                TargetCurrencyId = rateSaved.TargetCurrencyId,
                TargetCurrencyCode = rateSaved.TargetCurrency?.ISOCode ?? string.Empty,
                TargetCurrencyName = rateSaved.TargetCurrency?.Name ?? string.Empty,
                RateValue = rateSaved.RateValue,
                EffectiveDate = rateSaved.EffectiveDate,
                IsActive = rateSaved.IsActive
            };
        }
    }
}