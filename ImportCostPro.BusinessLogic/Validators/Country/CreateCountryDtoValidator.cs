using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Country;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Country;

public class CreateCountryDtoValidator : AbstractValidator<CreateCountryDto>
{
    public CreateCountryDtoValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be null or empty.")
            .MustAsync(async (name, ct) =>
                !await unitOfWork.Countries.ExistsByNameAsync(name.Trim()))
            .WithMessage("A country with the same name already exists.");

        RuleFor(x => x.ISOCode)
            .NotEmpty().WithMessage("ISOCode cannot be null or empty.")
            .Length(2, 3).WithMessage("ISOCode must be 2 or 3 characters long.")
            .MustAsync(async (isoCode, ct) =>
                !await unitOfWork.Countries.ExistsByISOCodeAsync(isoCode.Trim().ToUpperInvariant()))
            .WithMessage("A country with the same ISO code already exists.");
    }
}
