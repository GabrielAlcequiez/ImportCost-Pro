using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Product;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Validators.Product
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product Id is required for updates.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The product name is required")
                .MaximumLength(150).WithMessage("Name cannot be greater than 150 characters");

            RuleFor(x => x.CodeReference)
                .NotEmpty().WithMessage("The reference code is required")
                .MaximumLength(50).WithMessage("Code reference cannot be greater than 50 characters")
                .MustAsync(async (dto, codeReference, cancellationToken) =>
                {
                    var cleanCode = codeReference.Trim();
                    var exists = await unitOfWork.Products.ExistsByReferenceCodeAsync(cleanCode, dto.Id);
                    return !exists;
                })
                .WithMessage("It already exists a product with that code reference.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("The country is required")
                .MustAsync(async (countryId, cancellationToken) =>
                {
                    var country = await unitOfWork.Countries.GetByIdAsync(countryId);
                    return country != null && country.IsActive;
                })
                .WithMessage("The country need to be already active");

            RuleFor(x => x.TariffCategoryId)
                .NotEmpty().WithMessage("The tariff category is required")
                .MustAsync(async (categoryId, cancellationToken) =>
                {
                    var category = await unitOfWork.TariffCategories.GetByIdAsync(categoryId);
                    return category != null && category.IsActive;
                })
                .WithMessage("The selected tariff category needs to exists.");

            RuleFor(x => x.UnitWeight)
                .NotNull().WithMessage("Unit weight is required")
                .GreaterThan(0).WithMessage("Unit weight needs to be greater than 0.");

            RuleFor(x => x.UnitOfMeasure)
                .NotEmpty().WithMessage("The Unit of Measure is required");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot be greater than 250 characters");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("Status is required.");

            When(x => x.Length.HasValue || x.Width.HasValue || x.Height.HasValue, () =>
            {
                RuleFor(x => x.Length)
                    .NotNull().WithMessage("If length, width or height is provided, all three fields must have a value.")
                    .GreaterThan(0).WithMessage("Length must be greater than 0.");

                RuleFor(x => x.Width)
                    .NotNull().WithMessage("If length, width or height is provided, all three fields must have a value.")
                    .GreaterThan(0).WithMessage("Width must be greater than 0.");

                RuleFor(x => x.Height)
                    .NotNull().WithMessage("If length, width or height is provided, all three fields must have a value.")
                    .GreaterThan(0).WithMessage("Height must be greater than 0.");
            });
        }
    }
}