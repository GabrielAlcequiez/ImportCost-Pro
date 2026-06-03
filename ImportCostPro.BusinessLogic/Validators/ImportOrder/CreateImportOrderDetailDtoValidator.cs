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
                .NotEmpty().WithMessage("La orden de importación es requerida.")
                .MustAsync(async (id, ct) =>
                {
                    var order = await unitOfWork.ImportOrders.GetByIdAsync(id);
                    return order != null && order.Status == ImportOrderStatus.Open;
                }).WithMessage("La orden no existe o no está en estado Abierto.");

            // Product existence check will be completed when team delivers IProductRepository
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("El producto es requerido.");

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
