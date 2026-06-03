using System.Data;
using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.ExchangeRate;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.BusinessLogic.Validators.ExchangeRate
{
    public class UpdateExchangeRateDtoValidator : AbstractValidator<UpdateExchangeRateDto>
    {
        public UpdateExchangeRateDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x=>x.Id)
                .NotEmpty().WithMessage("El ID de la tasa de cambio es requerido para actualizar.");
            RuleFor(x => x.SourceCurrencyId)
                .NotEmpty().WithMessage("La moneda origen es requerida.")
                .MustAsync(async (id, ct) =>
                {
                    var currency = await unitOfWork.Currencies.GetByIdAsync(id);
                    return currency != null && currency.IsActive;
                }).WithMessage("La moneda origen debe existir y estar activa.");

            RuleFor(x => x.TargetCurrencyId)
                .NotEmpty().WithMessage("La moneda destino es requerida.")
                .MustAsync(async (id, cancellationToken) =>
                {
                    var currency = await unitOfWork.Currencies.GetByIdAsync(id);
                    return currency != null && currency.IsActive;
                }).WithMessage("La moneda destino debe existir y estar activa.")
                .NotEqual(x => x.SourceCurrencyId).WithMessage("La moneda origen no puede ser igual a la moneda destino.");
        
            RuleFor(x => x.RateValue)
                .NotNull().WithMessage("El valor de la tasa es requerido.")
                .GreaterThan(0).WithMessage("El valor de la tasa debe ser mayor que 0.");

            RuleFor(x=>x.EffectiveDate)
                .NotEmpty().WithMessage("La fecha de vigencia es requerida.");

            // validacion para evitar tasas donde moneda origen, destino y fhecha sean iguales
            RuleFor(X => X)
                .MustAsync(async (dto, ct) =>
                {
                    var exists = await unitOfWork.ExchangeRates.ExistsActiveRateAsync(
                        dto.SourceCurrencyId,
                        dto.TargetCurrencyId,
                        dto.EffectiveDate,
                        dto.Id);

                    return !exists;
                }).WithMessage("Ya existe una tasa de cambio activa para esta moneda origen, moneda destino y fecha de vigencia.")
                .WithName("UniqueRateConstraint");
        }       
    }
}