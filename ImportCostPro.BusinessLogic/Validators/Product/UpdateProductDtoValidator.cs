using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.Product;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.BusinessLogic.Validators.Product
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID del producto es requerido para actualizar.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del producto es requerido.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder 150 caracteres.");

            RuleFor(x => x.CodeReference)
                .NotEmpty().WithMessage("El código de referencia es requerido.")
                .MaximumLength(50).WithMessage("El código de referencia no puede exceder 50 caracteres.")
                .MustAsync(async (dto, codeReference, cancellationToken) =>
                {
                    var cleanCode = codeReference.Trim();
                    var exists = await unitOfWork.Products.ExistsByReferenceCodeAsync(cleanCode, dto.Id);
                    return !exists;
                })
                .WithMessage("Ya existe un producto con ese código de referencia.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("El país es requerido.")
                .MustAsync(async (dto, countryId, cancellationToken) =>
                {
                    var country = await unitOfWork.Countries.GetByIdAsync(countryId);
                    if (country == null) return false;
                    if (country.IsActive) return true;

                    var product = await unitOfWork.Products.GetByIdAsync(dto.Id);
                    return product != null && product.CountryId == countryId;
                })
                .WithMessage("El país debe estar activo.");

            RuleFor(x => x.TariffCategoryId)
                .NotEmpty().WithMessage("La categoría arancelaria es requerida.")
                .MustAsync(async (dto, categoryId, cancellationToken) =>
                {
                    var category = await unitOfWork.TariffCategories.GetByIdAsync(categoryId);
                    if (category == null) return false;
                    if (category.IsActive) return true;

                    var product = await unitOfWork.Products.GetByIdAsync(dto.Id);
                    return product != null && product.TariffCategoryId == categoryId;
                })
                .WithMessage("La categoría arancelaria seleccionada debe existir.");

            RuleFor(x => x.UnitWeight)
                .NotNull().WithMessage("El peso unitario es requerido.")
                .GreaterThan(0).WithMessage("El peso unitario debe ser mayor que 0.");

            RuleFor(x => x.UnitOfMeasure)
                .IsInEnum().WithMessage("La unidad de medida no es válida.");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("La descripción no puede exceder 250 caracteres.");

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("El estado es requerido.");

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

            RuleFor(x => x)
                .MustAsync(async (dto, cancellationToken) =>
                {
                    bool hasOrders = await unitOfWork.Products.HasRelatedEntitiesAsync(dto.Id);
                    if (!hasOrders) return true;

                    var original = await unitOfWork.Products.AsQueryable()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == dto.Id);

                    if (original == null) return false;

                    bool codeChanged = original.CodeReference != dto.CodeReference.Trim();
                    bool countryChanged = original.CountryId != dto.CountryId;
                    bool tariffChanged = original.TariffCategoryId != dto.TariffCategoryId;
                    bool weightChanged = original.UnitWeight != dto.UnitWeight;
                    bool lengthChanged = original.Length != dto.Length;
                    bool widthChanged = original.Width != dto.Width;
                    bool heightChanged = original.Height != dto.Height;

                    if (codeChanged || countryChanged || tariffChanged || weightChanged || lengthChanged || widthChanged || heightChanged)
                        return false;

                    return true;
                })
                .WithMessage("No se puede modificar este campo porque el producto ya está asociado a órdenes de importación.");
        }
    }
}