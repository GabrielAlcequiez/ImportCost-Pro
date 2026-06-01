using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.ImportOrder;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.ImportOrder
{
    public class CreateImportOrderDtoValidator : AbstractValidator<CreateImportOrderDto>
    {
        public CreateImportOrderDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.OrderNumber)
                .NotEmpty().WithMessage("Order number is required.")
                .MaximumLength(20).WithMessage("Order number cannot exceed 20 characters.")
                .MustAsync(async (num, ct) =>
                    !await unitOfWork.ImportOrders.ExistsByOrderNumberAsync(num))
                .WithMessage("An import order with this number already exists.");

            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("Order date is required.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Order date cannot be in the future.");

            RuleFor(x => x.ImporterId)
                .NotEmpty().WithMessage("Importer is required.")
                .MustAsync(async (id, ct) =>
                {
                    var entity = await unitOfWork.Importers.GetByIdAsync(id);
                    return entity != null && entity.IsActive;
                }).WithMessage("The selected importer does not exist or is inactive.");

            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("Supplier is required.")
                .MustAsync(async (id, ct) =>
                {
                    var entity = await unitOfWork.Suppliers.GetByIdAsync(id);
                    return entity != null && entity.IsActive;
                }).WithMessage("The selected supplier does not exist or is inactive.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("Country of origin is required.")
                .MustAsync(async (id, ct) =>
                {
                    var entity = await unitOfWork.Countries.GetByIdAsync(id);
                    return entity != null && entity.IsActive;
                }).WithMessage("The selected country does not exist or is inactive.");

            RuleFor(x => x.CurrencyId)
                .NotEmpty().WithMessage("Currency is required.")
                .MustAsync(async (id, ct) =>
                {
                    var entity = await unitOfWork.Currencies.GetByIdAsync(id);
                    return entity != null && entity.IsActive;
                }).WithMessage("The selected currency does not exist or is inactive.");

            RuleFor(x => x.ExchangeRateValue)
                .GreaterThan(0).WithMessage("Exchange rate must be greater than 0.");
        }
    }
}
