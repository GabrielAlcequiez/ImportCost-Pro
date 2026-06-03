using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Currency;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateCurrencyDto> _createValidator;
        private readonly IValidator<UpdateCurrencyDto> _updateValidator;

        public CurrencyService(
            IUnitOfWork unitOfWork,
            IValidator<CreateCurrencyDto> createValidator,
            IValidator<UpdateCurrencyDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<List<CurrencyDto>> GetAllCurrenciesAsync()
        {
            var currencies = await _unitOfWork.Currencies.GetAllAsync();
            var dtos = new List<CurrencyDto>();

            foreach (var currency in currencies)
            {
                dtos.Add(new CurrencyDto
                {
                    Id = currency.Id,
                    Name = currency.Name,
                    ISOCode = currency.ISOCode,
                    Symbol = currency.Symbol,
                    IsLocalCurrency = currency.IsLocalCurrency,
                    IsActive = currency.IsActive
                });
            }
            return dtos;
        }

        public async Task<CurrencyDto?> GetCurrencyByIdAsync(Guid id)
        {
            var currency = await _unitOfWork.Currencies.GetByIdAsync(id);
            if (currency == null) return null;

            return new CurrencyDto
            {
                Id = currency.Id,
                Name = currency.Name,
                ISOCode = currency.ISOCode,
                Symbol = currency.Symbol,
                IsLocalCurrency = currency.IsLocalCurrency,
                IsActive = currency.IsActive
            };
        }

        public async Task<CurrencyDto> CreateCurrencyAsync(CreateCurrencyDto currencyCreateDto)
        {
            ArgumentNullException.ThrowIfNull(currencyCreateDto);
            await _createValidator.ValidateAndThrowAsync(currencyCreateDto);

            var nameTrimmed = currencyCreateDto.Name.Trim();
            var isoCodeNormalized = currencyCreateDto.ISOCode.Trim().ToUpperInvariant();
            var symbolTrimmed = currencyCreateDto.Symbol.Trim();

            // If this currency is marked as local, ensure it's the only one
            if (currencyCreateDto.IsLocalCurrency)
            {
                await DeactivateExistingLocalCurrenciesAsync();
                await _unitOfWork.CompleteAsync();
            }


            var newCurrency = new Currency(nameTrimmed, isoCodeNormalized, symbolTrimmed, currencyCreateDto.IsLocalCurrency);
            await _unitOfWork.Currencies.AddAsync(newCurrency);
            await _unitOfWork.CompleteAsync();

            return new CurrencyDto
            {
                Id = newCurrency.Id,
                Name = newCurrency.Name,
                ISOCode = newCurrency.ISOCode,
                Symbol = newCurrency.Symbol,
                IsLocalCurrency = newCurrency.IsLocalCurrency,
                IsActive = newCurrency.IsActive
            };
        }

        public async Task<CurrencyDto> UpdateCurrencyAsync(Guid id, UpdateCurrencyDto currencyUpdateDto)
        {
            ArgumentNullException.ThrowIfNull(currencyUpdateDto);
            await _updateValidator.ValidateAndThrowAsync(currencyUpdateDto);

            var currency = await _unitOfWork.Currencies.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Currency not found.");

            // If we are deactivating a currency that is local, check if another local currency is active
            if (!currencyUpdateDto.IsActive && currencyUpdateDto.IsLocalCurrency)
            {
                throw new InvalidOperationException("Cannot deactivate the current active local currency.");
            }

            // If this currency is being marked as local, ensure it's the only one
            if (currencyUpdateDto.IsLocalCurrency)
            {
                await DeactivateExistingLocalCurrenciesAsync(id);
                await _unitOfWork.CompleteAsync();
            }


            currency.Update(
                currencyUpdateDto.Name.Trim(),
                currencyUpdateDto.ISOCode.Trim().ToUpperInvariant(),
                currencyUpdateDto.Symbol.Trim(),
                currencyUpdateDto.IsLocalCurrency,
                currencyUpdateDto.IsActive
                );

            await _unitOfWork.CompleteAsync();


            return new CurrencyDto
            {
                Id = currency.Id,
                Name = currency.Name,
                ISOCode = currency.ISOCode,
                Symbol = currency.Symbol,
                IsLocalCurrency = currency.IsLocalCurrency,
                IsActive = currency.IsActive
            };
        }

        public async Task<bool> DeleteCurrencyAsync(Guid id)
        {
            var currency = await _unitOfWork.Currencies.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Currency not found.");

            // Do not allow deleting the local currency
            if (currency.IsLocalCurrency)
            {
                throw new InvalidOperationException("Cannot delete the active local currency. Designate another currency as local before deleting this one.");
            }

            bool isSoftDelete;
            if (await _unitOfWork.Currencies.HasRelatedEntitiesAsync(id))
            {
                await _unitOfWork.Currencies.SoftDeleteAsync(id);
                isSoftDelete = true;
            }
            else
            {
                await _unitOfWork.Currencies.DeleteAsync(id);
                isSoftDelete = false;
            }

            await _unitOfWork.CompleteAsync();
            return isSoftDelete;
        }

        private async Task DeactivateExistingLocalCurrenciesAsync(Guid? exceptId = null)
        {
            var existingLocals = await _unitOfWork.Currencies.GetLocalCurrenciesAsync(exceptId);
            foreach (var localCurrency in existingLocals)
            {
                localCurrency.MarkAsLocal(false);
            }
        }
    }
}