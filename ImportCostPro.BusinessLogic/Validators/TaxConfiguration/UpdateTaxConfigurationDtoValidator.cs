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
                .WithMessage("ITBIS percentage must be between 0 and 100.");

            RuleFor(x => x.CustomsServiceFeePercentage)
                .InclusiveBetween(0, 100)
                .WithMessage("Customs service fee percentage must be between 0 and 100.");
        }
    }
}