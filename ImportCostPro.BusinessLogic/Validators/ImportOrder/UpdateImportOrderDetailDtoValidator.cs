using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.ImportOrder;

namespace ImportCostPro.BusinessLogic.Validators.ImportOrder
{
    // No IUnitOfWork needed — the service already guarantees the detail exists
    // and the parent order is Open before this validator runs
    public class UpdateImportOrderDetailDtoValidator : AbstractValidator<UpdateImportOrderDetailDto>
    {
        public UpdateImportOrderDetailDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID del detalle es requerido.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor que 0.");

            RuleFor(x => x.UnitCostFob)
                .GreaterThan(0).WithMessage("El costo unitario FOB debe ser mayor que 0.");

            RuleFor(x => x.UnitWeight)
                .GreaterThan(0).WithMessage("El peso unitario debe ser mayor que 0.");

            RuleFor(x => x.CustomDutyPercentage)
                .InclusiveBetween(0, 100)
                .WithMessage("El porcentaje de arancel debe estar entre 0 y 100.");

            RuleFor(x => x.DesiredProfitMargin)
                .InclusiveBetween(0, 99.99m)
                .WithMessage("El margen de ganancia deseado debe estar entre 0 y 99.99%.");
        }
    }
}
