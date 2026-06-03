using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.ImportOrder;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.ImportOrder
{
    public class UpdateImportOrderDtoValidator : AbstractValidator<UpdateImportOrderDto>
    {
        public UpdateImportOrderDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID es requerido.");

            RuleFor(x => x.OrderNumber)
                .NotEmpty().WithMessage("El número de orden es requerido.")
                .MaximumLength(20).WithMessage("El número de orden no puede exceder 20 caracteres.")
                .MustAsync(async (dto, num, ct) =>
                    !await unitOfWork.ImportOrders.ExistsByOrderNumberAsync(num, dto.Id))
                .WithMessage("Ya existe una orden de importación con este número.");

            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("La fecha de orden es requerida.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("La fecha de orden no puede ser en el futuro.");

            RuleFor(x => x.ImporterId)
                .NotEmpty().WithMessage("El importador es requerido.")
                .MustAsync(async (id, ct) =>
                {
                    var entity = await unitOfWork.Importers.GetByIdAsync(id);
                    return entity != null && entity.IsActive;
                }).WithMessage("El importador seleccionado no existe o está inactivo.");

            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("El proveedor es requerido.")
                .MustAsync(async (id, ct) =>
                {
                    var entity = await unitOfWork.Suppliers.GetByIdAsync(id);
                    return entity != null && entity.IsActive;
                }).WithMessage("El proveedor seleccionado no existe o está inactivo.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("El país de origen es requerido.")
                .MustAsync(async (id, ct) =>
                {
                    var entity = await unitOfWork.Countries.GetByIdAsync(id);
                    return entity != null && entity.IsActive;
                }).WithMessage("El país seleccionado no existe o está inactivo.");

            RuleFor(x => x.CurrencyId)
                .NotEmpty().WithMessage("La moneda es requerida.")
                .MustAsync(async (id, ct) =>
                {
                    var entity = await unitOfWork.Currencies.GetByIdAsync(id);
                    return entity != null && entity.IsActive;
                }).WithMessage("La moneda seleccionada no existe o está inactiva.");

            RuleFor(x => x.ExchangeRateValue)
                .GreaterThan(0).WithMessage("La tasa de cambio debe ser mayor que 0.");
        }
    }
}
