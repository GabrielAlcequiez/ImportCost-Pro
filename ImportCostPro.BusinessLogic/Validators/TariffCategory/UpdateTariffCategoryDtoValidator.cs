using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.TariffCategory;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.BusinessLogic.Validators.TariffCategory
{
    public class UpdateTariffCategoryDtoValidator : AbstractValidator<UpdateTariffCategoryDto>
    {
        public UpdateTariffCategoryDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Category Id is required for updates.");

            // 1. Código arancelario (Validando duplicados excluyendo el ID actual)
            RuleFor(x => x.TariffCode)
                .NotEmpty().WithMessage("The tariff code is required.")
                .MaximumLength(20).WithMessage("The tariff code cannot exceed 20 characters.")
                .MustAsync(async (dto, code, cancellationToken) =>
                {
                    var cleanCode = code.Trim();
                    var exists = await unitOfWork.TariffCategories.ExistsByCodeAsync(cleanCode, dto.Id);
                    return !exists; 
                })
                .WithMessage("A tariff category with this code already exists.");

            // 2. Nombre o descripción
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The name or description is required.")
                .MaximumLength(150).WithMessage("The name or description cannot exceed 150 characters.");

            // 3. Porcentaje de arancel
            RuleFor(x => x.TariffPercentage)
                .NotNull().WithMessage("The tariff percentage is required.")
                .GreaterThanOrEqualTo(0).WithMessage("The tariff percentage must be greater than or equal to 0.")
                .LessThanOrEqualTo(100).WithMessage("The tariff percentage cannot exceed 100.");

            // 4. Lógica condicional para el Impuesto Selectivo
            When(x => x.ApplyExciseTax == true, () =>
            {
                RuleFor(x => x.ExciseTaxPercentage)
                    .GreaterThan(0).WithMessage("If excise tax applies, the percentage must be greater than 0.")
                    .LessThanOrEqualTo(100).WithMessage("If excise tax applies, the percentage cannot exceed 100.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.ExciseTaxPercentage)
                    .Equal(0).WithMessage("If excise tax does not apply, the percentage must be 0.");
            });

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("Status is required.");

            // MEGA REGLA DE NEGOCIO (COMENTADA PA NO PERDERME)
            RuleFor(x => x)
                .MustAsync(async (dto, cancellationToken) =>
                {
                    // A. Verificamos si tiene productos asociados (Necesitas crear este método en el repo)
                    bool hasProducts = await unitOfWork.TariffCategories.HasRelatedProductsAsync(dto.Id);
                    
                    // Si no tiene productos, tiene vía libre para modificar lo que quiera
                    if (!hasProducts) return true; 

                    // B. Si TIENE productos, buscamos los valores originales en la BD
                    // Usamos AsNoTracking() para no interferir con el Update del Service más adelante
                    var original = await unitOfWork.TariffCategories.AsQueryable()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == dto.Id);

                    if (original == null) return false; // Por seguridad

                    // C. Verificamos que los campos críticos NO hayan sido alterados
                    bool codeChanged = original.TariffCode != dto.TariffCode.Trim();
                    bool tariffChanged = original.TariffPercentage != dto.TariffPercentage;
                    bool itbisChanged = original.ApplyITBIS != dto.ApplyITBIS;
                    bool exciseTaxChanged = original.ApplyExciseTax != dto.ApplyExciseTax;
                    bool excisePercentageChanged = original.ExciseTaxPercentage != dto.ExciseTaxPercentage;

                    // Si ALGUNO de los campos críticos cambió, la regla falla (retorna false)
                    if (codeChanged || tariffChanged || itbisChanged || exciseTaxChanged || excisePercentageChanged)
                    {
                        return false; 
                    }

                    return true; // Solo intentó cambiar el Nombre o el IsActive, se lo permitimos.
                })
                .WithMessage("Critical fiscal fields (Code, Tariff, ITBIS, Excise Tax) cannot be modified because this category is already associated with existing products. If you need a new fiscal configuration, create a new category.");
        }
    }
}