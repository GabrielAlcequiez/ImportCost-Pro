using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Supplier;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Supplier
{
    public class CreateSupplierDtoValidator : AbstractValidator<CreateSupplierDto>
    {
        public CreateSupplierDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder 150 caracteres.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("El país es requerido.")
                .MustAsync(async (id, ct) =>
                {
                    var country = await unitOfWork.Countries.GetByIdAsync(id);
                    if(country == null) return false;
                    return country.IsActive;
                }).WithMessage("El país seleccionado no existe o está inactivo.");

            // Lo correcto es que no aparezca, pero por si acaso..
            RuleFor(x => x.CurrencyId)
                .NotEmpty().WithMessage("La moneda es requerida.")
                .MustAsync(async (id, ct) =>
                {
                    var currency = await unitOfWork.Currencies.GetByIdAsync(id);
                    if (currency == null) return false;
                    return currency.IsActive;
                })
                .WithMessage("La moneda seleccionada no existe o está inactiva.");

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("El correo electrónico es demasiado largo o inválido.")
                .EmailAddress().WithMessage("El correo electrónico tiene un formato inválido, ingréselo correctamente.");
        
            RuleFor(x=>x.Telephone)
                .MaximumLength(20).WithMessage("El teléfono es demasiado largo.");

            
        }


    }
}