using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Importer;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Importer;

public class UpdateImporterDtoValidator : AbstractValidator<UpdateImporterDto>
{
    public UpdateImporterDtoValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be null or empty.")
            .MustAsync(async (dto, name, ct) =>
                !await unitOfWork.Importers.ExistsByNameAsync(name.Trim(), dto.Id))
            .WithMessage("An importer with the same name already exists.");

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("TaxId cannot be null or empty.")
            .MustAsync(async (dto, taxId, ct) =>
                !await unitOfWork.Importers.ExistsByTaxIdAsync(taxId.Trim(), dto.Id))
            .WithMessage("An importer with the same TaxId already exists.");

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("CountryId is required.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email must be a valid email address.");
    }
}