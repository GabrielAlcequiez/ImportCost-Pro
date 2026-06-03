using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.ImportOrder;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.ImportOrder
{
    public class UpdateImportOrderExpenseDtoValidator : AbstractValidator<UpdateImportOrderExpenseDto>
    {
        public UpdateImportOrderExpenseDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID del gasto es requerido.");

            RuleFor(x => x.CurrencyId)
                .NotEmpty().WithMessage("La moneda es requerida.")
                .MustAsync(async (id, ct) =>
                {
                    var currency = await unitOfWork.Currencies.GetByIdAsync(id);
                    return currency != null && currency.IsActive;
                }).WithMessage("La moneda seleccionada no existe o está inactiva.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("El monto debe ser mayor que 0.");

            RuleFor(x => x.ExchangeRateValue)
                .GreaterThan(0).WithMessage("La tasa de cambio debe ser mayor que 0.");

            RuleFor(x => x.ExpenseDate)
                .NotEmpty().WithMessage("La fecha del gasto es requerida.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("La fecha del gasto no puede ser en el futuro.");
        }
    }
}
