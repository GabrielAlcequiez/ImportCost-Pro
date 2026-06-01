
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
                .NotEmpty().WithMessage("Supplier Id is required for updates.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be null or empty")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("Country cannot be null or empty, is required.")
                .MustAsync(async (id, ct) =>
                {
                    var country = await unitOfWork.Countries.GetByIdAsync(id);
                    if (country == null) return false;
                    return true;
                }).WithMessage("The selected country does not exist.");

            // Lo correcto es que no aparezca, pero por si acaso..
            RuleFor(x => x.CurrencyId)
                .NotEmpty().WithMessage("Currency cannot be null or empty, is required.")
                .MustAsync(async (id, ct) =>
                {
                    var currency = await unitOfWork.Currencies.GetByIdAsync(id);
                    if (currency == null) return false;
                    return true;
                })
                .WithMessage("The selected currency does not exist.");

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("The email is too large or invalid")
                .EmailAddress().WithMessage("The email has an invalid format, please enter correctly");

            RuleFor(x => x.Telephone)
                .MaximumLength(20).WithMessage("The telephone is too large.");

            // PENDIENTE DE IMPLEMENTAR, RESTRICCIONES IMPORTANTES AL EDITAR
            // SI YA TIENE ORDENES DE IMPORTACIÓN
            // Y SIMILARES

                
        }
    }
}