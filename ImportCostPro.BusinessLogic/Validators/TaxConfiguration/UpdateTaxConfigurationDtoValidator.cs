using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.TaxConfiguration;

namespace ImportCostPro.BusinessLogic.Validators.TaxConfiguration
{
    public class UpdateTaxConfigurationDtoValidator : AbstractValidator<UpdateTaxConfigurationDto>
    {
        public UpdateTaxConfigurationDtoValidator()
        {
            RuleFor(x => x.GeneralItbisPercentage)
                .InclusiveBetween(0, 100)
                .WithMessage("El porcentaje de ITBIS debe estar entre 0 y 100.");

            RuleFor(x => x.CustomsServiceFeePercentage)
                .InclusiveBetween(0, 100)
                .WithMessage("El porcentaje de tasa de servicio aduanero debe estar entre 0 y 100.");
        }
    }
}