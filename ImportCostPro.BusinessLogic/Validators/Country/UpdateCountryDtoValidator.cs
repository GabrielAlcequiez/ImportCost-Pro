using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Country;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Country;

public class UpdateCountryDtoValidator : AbstractValidator<UpdateCountryDto>
{
    public UpdateCountryDtoValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MustAsync(async (dto, name, ct) =>
                !await unitOfWork.Countries.ExistsByNameAsync(name.Trim(), dto.Id))
            .WithMessage("Ya existe un país con el mismo nombre.");

        RuleFor(x => x.ISOCode)
            .NotEmpty().WithMessage("El código ISO es requerido.")
            .Length(2, 3).WithMessage("El código ISO debe tener 2 o 3 caracteres.")
            .MustAsync(async (dto, isoCode, ct) =>
                !await unitOfWork.Countries.ExistsByISOCodeAsync(isoCode.Trim().ToUpperInvariant(), dto.Id))
            .WithMessage("Ya existe un país con el mismo código ISO.");
    }
}
