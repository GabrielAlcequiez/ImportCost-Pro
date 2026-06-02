
using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.TariffCategory;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.TariffCategory
{
    public class CreateTariffCategoryDtoValidator : AbstractValidator<CreateTariffCategoryDto>
    {
        public CreateTariffCategoryDtoValidator(IUnitOfWork unitOfWork)
        {
            // 1. Código arancelario
            RuleFor(x => x.TariffCode)
                .NotEmpty().WithMessage("The tariff code is required.")
                .MaximumLength(20).WithMessage("The tariff code cannot exceed 20 characters.")
                .MustAsync(async (code, cancellationToken) =>
                {
                    var cleanCode = code.Trim();
                    var exists = await unitOfWork.TariffCategories.ExistsByCodeAsync(cleanCode);
                    return !exists;
                })
                .WithMessage("A tariff category with this code already exists.");

            // 2. Nombre o descripción
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The name or description is required.")
                .MaximumLength(150).WithMessage("The name or description cannot exceed 150 characters.");

            // 3. Porcentaje de arancel (Entre 0 y 100)
            RuleFor(x => x.TariffPercentage)
                .NotNull().WithMessage("The tariff percentage is required.")
                .GreaterThanOrEqualTo(0).WithMessage("The tariff percentage must be greater than or equal to 0.")
                .LessThanOrEqualTo(100).WithMessage("The tariff percentage cannot exceed 100.");

            // 4. Lógica condicional para el Impuesto Selectivo
            When(x => x.ApplyExciseTax == true, () =>
            {
                // Si APLICA el impuesto selectivo
                RuleFor(x => x.ExciseTaxPercentage)
                    .GreaterThan(0).WithMessage("If selective tax applies, the percentage must be greater than 0.")
                    .LessThanOrEqualTo(100).WithMessage("If excise tax applies, the percentage cannot exceed 100.");
            })
            .Otherwise(() =>
            {
                // Si NO APLICA el impuesto selectivo
                RuleFor(x => x.ExciseTaxPercentage)
                    .Equal(0).WithMessage("If excise tax does not apply, the percentage must be 0.");
            });
        }
    }
}
