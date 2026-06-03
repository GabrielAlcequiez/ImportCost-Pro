using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Importer;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Importer;

public class UpdateImporterDtoValidator : AbstractValidator<UpdateImporterDto>
{
    public UpdateImporterDtoValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MustAsync(async (dto, name, ct) =>
                !await unitOfWork.Importers.ExistsByNameAsync(name.Trim(), dto.Id))
            .WithMessage("Ya existe un importador con el mismo nombre.");

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("El RNC/NIT es requerido.")
            .MustAsync(async (dto, taxId, ct) =>
                !await unitOfWork.Importers.ExistsByTaxIdAsync(taxId.Trim(), dto.Id))
            .WithMessage("Ya existe un importador con el mismo RNC/NIT.");

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("El país es requerido.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("El correo electrónico debe ser válido.");
    }
}