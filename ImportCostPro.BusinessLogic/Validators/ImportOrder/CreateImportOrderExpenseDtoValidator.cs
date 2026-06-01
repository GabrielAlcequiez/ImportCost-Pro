using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.ImportOrder;
using ImportCostPro.Database.Entities.Enums;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.ImportOrder
{
    public class CreateImportOrderExpenseDtoValidator : AbstractValidator<CreateImportOrderExpenseDto>
    {
        public CreateImportOrderExpenseDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.ImportOrderId)
                .NotEmpty().WithMessage("Import order is required.")
                .MustAsync(async (id, ct) =>
                {
                    var order = await unitOfWork.ImportOrders.GetByIdAsync(id);
                    return order != null && order.Status == ImportOrderStatus.Open;
                }).WithMessage("The order does not exist or is not in Open status.");

            RuleFor(x => x.CurrencyId)
                .NotEmpty().WithMessage("Currency is required.")
                .MustAsync(async (id, ct) =>
                {
                    var currency = await unitOfWork.Currencies.GetByIdAsync(id);
                    return currency != null && currency.IsActive;
                }).WithMessage("The selected currency does not exist or is inactive.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.ExchangeRateValue)
                .GreaterThan(0).WithMessage("Exchange rate must be greater than 0.");

            RuleFor(x => x.ExpenseDate)
                .NotEmpty().WithMessage("Expense date is required.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Expense date cannot be in the future.");
        }
    }
}
