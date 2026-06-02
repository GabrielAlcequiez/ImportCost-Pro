using System.Data;
using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.ExchangeRate;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.ExchangeRate
{
    public class CreateExchangeRateDtoValidator : AbstractValidator<CreateExchangeRateDto>
    {
        public CreateExchangeRateDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.SourceCurrencyId)
                .NotEmpty().WithMessage("The source currency is required.")
                .MustAsync(async (id, ct) =>
                {
                    var currency = await unitOfWork.Currencies.GetByIdAsync(id);
                    return currency != null && currency.IsActive;
                }).WithMessage("The selected source currency must exist and be active");

            RuleFor(x => x.TargetCurrencyId)
                .NotEmpty().WithMessage("The target currency is required.")
                .MustAsync(async (id, cancellationToken) =>
                {
                    var currency = await unitOfWork.Currencies.GetByIdAsync(id);
                    return currency != null && currency.IsActive;
                }).WithMessage("The selected target currency must exist and be active.")
                .NotEqual(x => x.SourceCurrencyId).WithMessage("The source currency cannot be the same as the target currency.");
        
            RuleFor(x => x.RateValue)
                .NotNull().WithMessage("The rate value is required.")
                .GreaterThan(0).WithMessage("The rate value must be greater than 0.");

            RuleFor(x=>x.EffectiveDate)
                .NotEmpty().WithMessage("The effective date is required");

            // validacion para evitar tasas donde moneda origen, destino y fhecha sean iguales
            RuleFor(X => X)
                .MustAsync(async (dto, ct) =>
                {
                    var exists = await unitOfWork.ExchangeRates.ExistsActiveRateAsync(
                        dto.SourceCurrencyId,
                        dto.TargetCurrencyId,
                        dto.EffectiveDate);

                    return !exists;
                }).WithMessage("An active exchange rate already exists for this source currency, target currency, and effective date.");
        }
    }
}