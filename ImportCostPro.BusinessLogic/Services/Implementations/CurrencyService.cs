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
        public CurrencyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            if (string.IsNullOrWhiteSpace(currencyCreateDto.Name))
                throw new ArgumentException("Name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(currencyCreateDto.ISOCode))
                throw new ArgumentException("ISOCode cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(currencyCreateDto.Symbol))
                throw new ArgumentException("Symbol cannot be null or empty.");

            var nameTrimmed = currencyCreateDto.Name.Trim();
            var isoCodeNormalized = currencyCreateDto.ISOCode.Trim().ToUpperInvariant();
            var symbolTrimmed = currencyCreateDto.Symbol.Trim();

            if (isoCodeNormalized.Length != 3)
                throw new ArgumentException("ISOCode must be exactly 3 characters long.");

            // Check uniqueness of Name and ISOCode
            // if (await _context.Currencies.AnyAsync(c => c.Name.ToLower() == nameTrimmed.ToLower()))
            //     throw new InvalidOperationException("A currency with the same name already exists.");

            // if (await _context.Currencies.AnyAsync(c => c.ISOCode == isoCodeNormalized))
            //     throw new InvalidOperationException("A currency with the same ISO code already exists.");

            // If this currency is marked as local, ensure it's the only one
            if (currencyCreateDto.IsLocalCurrency)
            {
                await DeactivateExistingLocalCurrenciesAsync();
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

            var currency = await _unitOfWork.Currencies.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Currency not found.");

            if (string.IsNullOrWhiteSpace(currencyUpdateDto.Name))
                throw new ArgumentException("Name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(currencyUpdateDto.ISOCode))
                throw new ArgumentException("ISOCode cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(currencyUpdateDto.Symbol))
                throw new ArgumentException("Symbol cannot be null or empty.");

            // var nameTrimmed = currencyUpdateDto.Name.Trim();
            // var isoCodeNormalized = currencyUpdateDto.ISOCode.Trim().ToUpperInvariant();
            // var symbolTrimmed = currencyUpdateDto.Symbol.Trim();

            // if (isoCodeNormalized.Length != 3)
            //     throw new ArgumentException("ISOCode must be exactly 3 characters long.");

            // // Check uniqueness of Name and ISOCode excluding current record
            // if (await _context.Currencies.AnyAsync(c => c.Id != id && c.Name.ToLower() == nameTrimmed.ToLower()))
            //     throw new InvalidOperationException("A currency with the same name already exists.");

            // if (await _context.Currencies.AnyAsync(c => c.Id != id && c.ISOCode == isoCodeNormalized))
            //     throw new InvalidOperationException("A currency with the same ISO code already exists.");

            // If we are deactivating local status on this currency, we must ensure another local currency exists
            // if (currency.IsLocalCurrency && !currencyUpdateDto.IsLocalCurrency)
            // {
            //     var otherLocalExists = await _context.Currencies.AnyAsync(c => c.Id != id && c.IsLocalCurrency && c.IsActive);
            //     if (!otherLocalExists)
            //     {
            //         throw new InvalidOperationException("Cannot unset local status on the only active local currency. You must designate another local currency first.");
            //     }
            // }

            // If we are deactivating a currency that is local, check if another local currency is active
            if (!currencyUpdateDto.IsActive && currencyUpdateDto.IsLocalCurrency)
            {
                throw new InvalidOperationException("Cannot deactivate the current active local currency.");
            }

            // If this currency is being marked as local, ensure it's the only one
            if (currencyUpdateDto.IsLocalCurrency)
            {
                await DeactivateExistingLocalCurrenciesAsync(id);
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