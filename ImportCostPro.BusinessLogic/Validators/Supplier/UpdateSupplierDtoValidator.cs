
using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Supplier;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Supplier
{
    public class UpdateSupplierDtoValidator : AbstractValidator<UpdateSupplierDto>
    {
        public UpdateSupplierDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID del proveedor es requerido para actualizar.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder 150 caracteres.")
                .MustAsync(async(dto, name, ct ) =>
                {
                    bool exists = await unitOfWork.Suppliers.ExistsByNameAsync(name, dto.Id);
                    return !exists;
                }).WithMessage("Ya existe un proveedor con este nombre.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("El país es requerido.")
                .MustAsync(async (id, ct) =>
                {
                    var country = await unitOfWork.Countries.GetByIdAsync(id);
                    if (country == null) return false;
                    return true;
                }).WithMessage("El país seleccionado no existe.");

            // Lo correcto es que no aparezca, pero por si acaso..
            RuleFor(x => x.CurrencyId)
                .NotEmpty().WithMessage("La moneda es requerida.")
                .MustAsync(async (id, ct) =>
                {
                    var currency = await unitOfWork.Currencies.GetByIdAsync(id);
                    if (currency == null) return false;
                    return true;
                })
                .WithMessage("La moneda seleccionada no existe.");

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("El correo electrónico es demasiado largo o inválido.")
                .EmailAddress().WithMessage("El correo electrónico tiene un formato inválido, ingréselo correctamente.");

            RuleFor(x => x.Telephone)
                .MaximumLength(20).WithMessage("El teléfono es demasiado largo.");

            RuleFor(x => x)
                .MustAsync(async (dto, ct) =>
                {
                    var originalSupplier = await unitOfWork.Suppliers.GetByIdAsync(dto.Id);
                    if (originalSupplier == null) return true;

                    bool changingCountry = dto.CountryId != originalSupplier.CountryId;
                    bool changingCurrency = dto.CurrencyId != originalSupplier.CurrencyId;

                    if (changingCountry || changingCurrency)
                    {
                        bool hasOrders = await unitOfWork.Suppliers.HasRelatedEntitiesAsync(dto.Id);
                        if (hasOrders) return false; // Falla la validación
                    }

                    return true;
                })
                .WithMessage("El país y la moneda principal no pueden ser modificados porque ya tiene órdenes de importación registradas.");
                
        }
    }
}