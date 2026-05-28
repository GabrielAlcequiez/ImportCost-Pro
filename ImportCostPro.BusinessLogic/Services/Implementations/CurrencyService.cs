using ImportCostPro.BusinessLogic.DTOs.Currency;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database;
using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class CurrencyService(AppDbContext context) : ICurrencyService
    {
        private readonly AppDbContext _context = context;

        public async Task<List<CurrencyDto>> GetAllCurrenciesAsync()
        {
            var currencies = await _context.Currencies.ToListAsync();
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
            var currency = await _context.Currencies.FindAsync(id);
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
            if (await _context.Currencies.AnyAsync(c => c.Name.ToLower() == nameTrimmed.ToLower()))
                throw new InvalidOperationException("A currency with the same name already exists.");

            if (await _context.Currencies.AnyAsync(c => c.ISOCode == isoCodeNormalized))
                throw new InvalidOperationException("A currency with the same ISO code already exists.");

            // If this currency is marked as local, ensure it's the only one
            if (currencyCreateDto.IsLocalCurrency)
            {
                await DeactivateExistingLocalCurrenciesAsync();
            }

            var newCurrency = new Currency(nameTrimmed, isoCodeNormalized, symbolTrimmed, currencyCreateDto.IsLocalCurrency);
            _context.Currencies.Add(newCurrency);
            await _context.SaveChangesAsync();

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

            var currency = await _context.Currencies.FindAsync(id)
                ?? throw new KeyNotFoundException("Currency not found.");

            if (string.IsNullOrWhiteSpace(currencyUpdateDto.Name))
                throw new ArgumentException("Name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(currencyUpdateDto.ISOCode))
                throw new ArgumentException("ISOCode cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(currencyUpdateDto.Symbol))
                throw new ArgumentException("Symbol cannot be null or empty.");

            var nameTrimmed = currencyUpdateDto.Name.Trim();
            var isoCodeNormalized = currencyUpdateDto.ISOCode.Trim().ToUpperInvariant();
            var symbolTrimmed = currencyUpdateDto.Symbol.Trim();

            if (isoCodeNormalized.Length != 3)
                throw new ArgumentException("ISOCode must be exactly 3 characters long.");

            // Check uniqueness of Name and ISOCode excluding current record
            if (await _context.Currencies.AnyAsync(c => c.Id != id && c.Name.ToLower() == nameTrimmed.ToLower()))
                throw new InvalidOperationException("A currency with the same name already exists.");

            if (await _context.Currencies.AnyAsync(c => c.Id != id && c.ISOCode == isoCodeNormalized))
                throw new InvalidOperationException("A currency with the same ISO code already exists.");

            // If we are deactivating local status on this currency, we must ensure another local currency exists
            if (currency.IsLocalCurrency && !currencyUpdateDto.IsLocalCurrency)
            {
                var otherLocalExists = await _context.Currencies.AnyAsync(c => c.Id != id && c.IsLocalCurrency && c.IsActive);
                if (!otherLocalExists)
                {
                    throw new InvalidOperationException("Cannot unset local status on the only active local currency. You must designate another local currency first.");
                }
            }

            // If this currency is being marked as local, ensure it's the only one
            if (currencyUpdateDto.IsLocalCurrency)
            {
                await DeactivateExistingLocalCurrenciesAsync(id);
            }

            // If we are deactivating a currency that is local, check if another local currency is active
            if (!currencyUpdateDto.IsActive && currencyUpdateDto.IsLocalCurrency)
            {
                throw new InvalidOperationException("Cannot deactivate the current active local currency.");
            }

            currency.Update(nameTrimmed, isoCodeNormalized, symbolTrimmed, currencyUpdateDto.IsLocalCurrency, currencyUpdateDto.IsActive);
            await _context.SaveChangesAsync();

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
            var currency = await _context.Currencies.FindAsync(id)
                ?? throw new KeyNotFoundException("Currency not found.");

            // Do not allow deleting the local currency
            if (currency.IsLocalCurrency)
            {
                throw new InvalidOperationException("Cannot delete the active local currency. Designate another currency as local before deleting this one.");
            }

            var hasRelatedEntities = await _context.Suppliers.AnyAsync(s => s.CurrencyId == id) ||
                                     await _context.ExchangeRates.AnyAsync(r => r.SourceCurrencyId == id || r.TargetCurrencyId == id) ||
                                     await _context.ImportOrders.AnyAsync(o => o.CurrencyId == id) ||
                                     await _context.ImportOrderExpenses.AnyAsync(e => e.CurrencyId == id) ||
                                     await _context.LandedCostCalculations.AnyAsync(c => c.LocalCurrencyId == id);

            if (hasRelatedEntities)
            {
                // Soft delete: deactivate
                currency.Update(currency.Name, currency.ISOCode, currency.Symbol, currency.IsLocalCurrency, false);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                // Hard delete: remove
                _context.Currencies.Remove(currency);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        private async Task DeactivateExistingLocalCurrenciesAsync(Guid? exceptId = null)
        {
            var query = _context.Currencies.Where(c => c.IsLocalCurrency);
            if (exceptId.HasValue)
            {
                query = query.Where(c => c.Id != exceptId.Value);
            }

            var existingLocals = await query.ToListAsync();
            foreach (var localCurrency in existingLocals)
            {
                localCurrency.MarkAsLocal(false);
            }
        }
    }
}