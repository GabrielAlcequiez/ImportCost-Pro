
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
                .NotEmpty().WithMessage("El código arancelario es requerido.")
                .MaximumLength(20).WithMessage("El código arancelario no puede exceder 20 caracteres.")
                .MustAsync(async (code, cancellationToken) =>
                {
                    var cleanCode = code.Trim();
                    var exists = await unitOfWork.TariffCategories.ExistsByCodeAsync(cleanCode);
                    return !exists;
                })
                .WithMessage("Ya existe una categoría arancelaria con este código.");

            // 2. Nombre o descripción
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre o descripción es requerido.")
                .MaximumLength(150).WithMessage("El nombre o descripción no puede exceder 150 caracteres.");

            // 3. Porcentaje de arancel (Entre 0 y 100)
            RuleFor(x => x.TariffPercentage)
                .NotNull().WithMessage("El porcentaje de arancel es requerido.")
                .GreaterThanOrEqualTo(0).WithMessage("El porcentaje de arancel debe ser mayor o igual a 0.")
                .LessThanOrEqualTo(100).WithMessage("El porcentaje de arancel no puede exceder 100.");

            // 4. Lógica condicional para el Impuesto Selectivo
            When(x => x.ApplyExciseTax == true, () =>
            {
                // Si APLICA el impuesto selectivo
                RuleFor(x => x.ExciseTaxPercentage)
                    .GreaterThan(0).WithMessage("Si aplica impuesto selectivo, el porcentaje debe ser mayor que 0.")
                    .LessThanOrEqualTo(100).WithMessage("Si aplica impuesto selectivo, el porcentaje no puede exceder 100.");
            })
            .Otherwise(() =>
            {
                // Si NO APLICA el impuesto selectivo
                RuleFor(x => x.ExciseTaxPercentage)
                    .Equal(0).WithMessage("Si no aplica impuesto selectivo, el porcentaje debe ser 0.");
            });
        }
    }
}
