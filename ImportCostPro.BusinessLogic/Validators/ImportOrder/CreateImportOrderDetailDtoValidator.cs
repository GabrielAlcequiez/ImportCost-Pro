using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.ImportOrder;
using ImportCostPro.Database.Entities.Enums;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.ImportOrder
{
    public class CreateImportOrderDetailDtoValidator : AbstractValidator<CreateImportOrderDetailDto>
    {
        public CreateImportOrderDetailDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.ImportOrderId)
                .NotEmpty().WithMessage("Import order is required.")
                .MustAsync(async (id, ct) =>
                {
                    var order = await unitOfWork.ImportOrders.GetByIdAsync(id);
                    return order != null && order.Status == ImportOrderStatus.Open;
                }).WithMessage("The order does not exist or is not in Open status.");

            // Product existence check will be completed when team delivers IProductRepository
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product is required.");

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
