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
                .NotEmpty().WithMessage("Detail Id is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.UnitCostFob)
                .GreaterThan(0).WithMessage("Unit FOB cost must be greater than 0.");

            RuleFor(x => x.UnitWeight)
                .GreaterThan(0).WithMessage("Unit weight must be greater than 0.");

            RuleFor(x => x.CustomDutyPercentage)
                .InclusiveBetween(0, 100)
                .WithMessage("Custom duty percentage must be between 0 and 100.");

            RuleFor(x => x.DesiredProfitMargin)
                .InclusiveBetween(0, 99.99m)
                .WithMessage("Desired profit margin must be between 0 and 99.99%.");
        }
    }
}
