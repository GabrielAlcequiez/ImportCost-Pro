using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Currency;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Currency;

public class UpdateCurrencyDtoValidator : AbstractValidator<UpdateCurrencyDto>
{
    public UpdateCurrencyDtoValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be null or empty.")
            .MustAsync(async (dto, name, ct) =>
                !await unitOfWork.Currencies.ExistsByNameAsync(name.Trim(), dto.Id))
            .WithMessage("A currency with the same name already exists.");

        RuleFor(x => x.ISOCode)
            .NotEmpty().WithMessage("ISOCode cannot be null or empty.")
            .Length(3).WithMessage("ISOCode must be exactly 3 characters long.")
            .MustAsync(async (dto, isoCode, ct) =>
                !await unitOfWork.Currencies.ExistsByISOCodeAsync(isoCode.Trim().ToUpperInvariant(), dto.Id))
            .WithMessage("A currency with the same ISO code already exists.");

        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("Symbol cannot be null or empty.");
    }
}
