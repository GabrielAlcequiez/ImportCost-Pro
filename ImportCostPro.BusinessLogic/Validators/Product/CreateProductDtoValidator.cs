using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Product;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Product
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator(IUnitOfWork unitOfWork)
        {

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del producto es requerido.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder 150 caracteres.");

            // 2. Código o referencia (Con el flujo asíncrono y Trim() que limpiamos)
            RuleFor(x => x.CodeReference)
                .NotEmpty().WithMessage("El código de referencia es requerido.")
                .MaximumLength(50).WithMessage("El código de referencia no puede exceder 50 caracteres.")
                .MustAsync(async (codeReference, cancellationToken) =>
                {
                    var cleanCode = codeReference.Trim();
                    var exists = await unitOfWork.Products.ExistsByReferenceCodeAsync(cleanCode);
                    return !exists; 
                })
                .WithMessage("Ya existe un producto con ese código de referencia.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("El país es requerido.")
                .MustAsync(async (countryId, cancellationToken) =>
                {
                    var country = await unitOfWork.Countries.GetByIdAsync(countryId);
                    return country != null && country.IsActive;
                })
                .WithMessage("El país debe estar activo.");

            RuleFor(x => x.TariffCategoryId)
                .NotEmpty().WithMessage("La categoría arancelaria es requerida.")
                .MustAsync(async (categoryId, cancellationToken) =>
                {
                    var category = await unitOfWork.TariffCategories.GetByIdAsync(categoryId);
                    return category != null && category.IsActive;
                })
                .WithMessage("La categoría arancelaria seleccionada debe existir.");


            RuleFor(x => x.UnitWeight)
                .NotNull().WithMessage("El peso unitario es requerido.")
                .GreaterThan(0).WithMessage("El peso unitario debe ser mayor que 0.");


            RuleFor(x => x.UnitOfMeasure)
                .IsInEnum().WithMessage("La unidad de medida no es válida.");


            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("La descripción no puede exceder 250 caracteres.");


            When(x => x.Length.HasValue || x.Width.HasValue || x.Height.HasValue, () =>
            {
                RuleFor(x => x.Length)
                    .NotNull().WithMessage("Si se coloca largo, ancho o alto, los tres campos deben tener valor.")
                    .GreaterThan(0).WithMessage("El largo debe ser mayor que 0.");

                RuleFor(x => x.Width)
                    .NotNull().WithMessage("Si se coloca largo, ancho o alto, los tres campos deben tener valor.")
                    .GreaterThan(0).WithMessage("El ancho debe ser mayor que 0.");

                RuleFor(x => x.Height)
                    .NotNull().WithMessage("Si se coloca largo, ancho o alto, los tres campos deben tener valor.")
                    .GreaterThan(0).WithMessage("El alto debe ser mayor que 0.");
            });
        }
    }
}