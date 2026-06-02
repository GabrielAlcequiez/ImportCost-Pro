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
                .NotEmpty().WithMessage("The product name is required")
                .MaximumLength(150).WithMessage("Name cannot be greater than 150 characters");

            // 2. Código o referencia (Con el flujo asíncrono y Trim() que limpiamos)
            RuleFor(x => x.CodeReference)
                .NotEmpty().WithMessage("The reference code is required")
                .MaximumLength(50).WithMessage("Code reference cannot be greater than 50 characters")
                .MustAsync(async (codeReference, cancellationToken) =>
                {
                    var cleanCode = codeReference.Trim();
                    var exists = await unitOfWork.Products.ExistsByReferenceCodeAsync(cleanCode);
                    return !exists; 
                })
                .WithMessage("It already exits a product with that code reference.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("The country is required")
                .MustAsync(async (countryId, cancellationToken) =>
                {
                    var country = await unitOfWork.Countries.GetByIdAsync(countryId);
                    return country != null && country.IsActive;
                })
                .WithMessage("The country need to be already active");

            // 4. Categoría arancelaria (Debe existir y estar activa)
            // RuleFor(x => x.TariffCategoryId)
            //     .NotEmpty().WithMessage("La categoría arancelaria es requerida.")
            //     .MustAsync(async (categoryId, cancellationToken) =>
            //     {
            //         var category = await unitOfWork.TariffCategories.GetByIdAsync(categoryId);
            //         return category != null && category.IsActive;
            //     })
            //     .WithMessage("La categoría arancelaria seleccionada debe existir y estar activa.");


            RuleFor(x => x.UnitWeight)
                .NotNull().WithMessage("Unit weight is required")
                .GreaterThan(0).WithMessage("Unit weight needs to be greather than 0.");


            RuleFor(x => x.UnitOfMeasure)
                .NotEmpty().WithMessage("The Unit of Measure is required");


            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot be greater than 250 characters");


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