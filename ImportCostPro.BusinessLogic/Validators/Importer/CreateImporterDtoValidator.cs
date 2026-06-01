using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Importer;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Importer;

public class CreateImporterDtoValidator : AbstractValidator<CreateImporterDto>
{
    public CreateImporterDtoValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be null or empty.")
            .MustAsync(async (name, ct) =>
                !await unitOfWork.Importers.ExistsByNameAsync(name.Trim()))
            .WithMessage("An importer with the same name already exists.");

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("TaxId cannot be null or empty.")
            .MustAsync(async (taxId, ct) =>
                !await unitOfWork.Importers.ExistsByTaxIdAsync(taxId.Trim()))
            .WithMessage("An importer with the same TaxId already exists.");

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("CountryId is required.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email must be a valid email address.");
    }
}