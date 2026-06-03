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
                .NotEmpty().WithMessage("El ID de categoría es requerido para actualizar.");

            // 1. Código arancelario (Validando duplicados excluyendo el ID actual)
            RuleFor(x => x.TariffCode)
                .NotEmpty().WithMessage("El código arancelario es requerido.")
                .MaximumLength(20).WithMessage("El código arancelario no puede exceder 20 caracteres.")
                .MustAsync(async (dto, code, cancellationToken) =>
                {
                    var cleanCode = code.Trim();
                    var exists = await unitOfWork.TariffCategories.ExistsByCodeAsync(cleanCode, dto.Id);
                    return !exists; 
                })
                .WithMessage("Ya existe una categoría arancelaria con este código.");

            // 2. Nombre o descripción
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre o descripción es requerido.")
                .MaximumLength(150).WithMessage("El nombre o descripción no puede exceder 150 caracteres.");

            // 3. Porcentaje de arancel
            RuleFor(x => x.TariffPercentage)
                .NotNull().WithMessage("El porcentaje de arancel es requerido.")
                .GreaterThanOrEqualTo(0).WithMessage("El porcentaje de arancel debe ser mayor o igual a 0.")
                .LessThanOrEqualTo(100).WithMessage("El porcentaje de arancel no puede exceder 100.");

            // 4. Lógica condicional para el Impuesto Selectivo
            When(x => x.ApplyExciseTax == true, () =>
            {
                RuleFor(x => x.ExciseTaxPercentage)
                    .GreaterThan(0).WithMessage("Si aplica impuesto selectivo, el porcentaje debe ser mayor que 0.")
                    .LessThanOrEqualTo(100).WithMessage("Si aplica impuesto selectivo, el porcentaje no puede exceder 100.");
            }).Otherwise(() =>
            {
                RuleFor(x => x.ExciseTaxPercentage)
                    .Equal(0).WithMessage("Si no aplica impuesto selectivo, el porcentaje debe ser 0.");
            });

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("El estado es requerido.");

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
                .WithMessage("Los campos fiscales críticos (Código, Arancel, ITBIS, Impuesto Selectivo) no pueden ser modificados porque esta categoría ya está asociada a productos existentes. Si necesita una nueva configuración fiscal, cree una nueva categoría.");
        }
    }
}