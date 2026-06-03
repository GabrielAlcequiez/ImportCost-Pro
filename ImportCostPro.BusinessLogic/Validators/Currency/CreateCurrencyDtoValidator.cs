using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Currency;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Currency;

public class CreateCurrencyDtoValidator : AbstractValidator<CreateCurrencyDto>
{
    public CreateCurrencyDtoValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be null or empty.")
            .MustAsync(async (name, ct) =>
                !await unitOfWork.Currencies.ExistsByNameAsync(name.Trim()))
            .WithMessage("A currency with the same name already exists.");

        RuleFor(x => x.ISOCode)
            .NotEmpty().WithMessage("ISOCode cannot be null or empty.")
            .Length(2, 3).WithMessage("ISOCode must be 2 or 3 characters long.")
            .MustAsync(async (isoCode, ct) =>
                !await unitOfWork.Currencies.ExistsByISOCodeAsync(isoCode.Trim().ToUpperInvariant()))
            .WithMessage("A currency with the same ISO code already exists.");

        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("Symbol cannot be null or empty.");
    }
}
