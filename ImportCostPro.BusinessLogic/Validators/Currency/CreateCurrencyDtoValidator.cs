using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Currency;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Currency;

public class CreateCurrencyDtoValidator : AbstractValidator<CreateCurrencyDto>
{
    public CreateCurrencyDtoValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MustAsync(async (name, ct) =>
                !await unitOfWork.Currencies.ExistsByNameAsync(name.Trim()))
            .WithMessage("Ya existe una moneda con el mismo nombre.");

        RuleFor(x => x.ISOCode)
            .NotEmpty().WithMessage("El código ISO es requerido.")
            .Length(2, 3).WithMessage("El código ISO debe tener 2 o 3 caracteres.")
            .MustAsync(async (isoCode, ct) =>
                !await unitOfWork.Currencies.ExistsByISOCodeAsync(isoCode.Trim().ToUpperInvariant()))
            .WithMessage("Ya existe una moneda con el mismo código ISO.");

        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("El símbolo es requerido.");
    }
}
