using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.TaxConfiguration;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class TaxConfigurationService : ITaxConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateTaxConfigurationDto> _updateValidator;

        // Real-world defaults for the Dominican Republic
        private const decimal DefaultItbisPercentage = 18m;
        private const decimal DefaultCustomsServiceFeePercentage = 3m;

        public TaxConfigurationService(
            IUnitOfWork unitOfWork,
            IValidator<UpdateTaxConfigurationDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _updateValidator = updateValidator;
        }

        public async Task<TaxConfigurationDto> GetCurrentAsync()
        {
            var config = await _unitOfWork.TaxConfigurations.GetCurrentAsync()
                         ?? await InitializeDefaultAsync();

            return MapToDto(config);
        }

        public async Task<TaxConfigurationDto> UpdateAsync(UpdateTaxConfigurationDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await _updateValidator.ValidateAndThrowAsync(dto);

            // If somehow no record exists yet, initialize before updating
            var config = await _unitOfWork.TaxConfigurations.GetCurrentAsync()
                         ?? await InitializeDefaultAsync();

            config.Update(dto.GeneralItbisPercentage, dto.CustomsServiceFeePercentage);
            await _unitOfWork.CompleteAsync();

            return MapToDto(config);
        }

        // Creates a default record the first time the system needs it
        private async Task<TaxConfiguration> InitializeDefaultAsync()
        {
            var defaultConfig = new TaxConfiguration(DefaultItbisPercentage, DefaultCustomsServiceFeePercentage);
            await _unitOfWork.TaxConfigurations.AddAsync(defaultConfig);
            await _unitOfWork.CompleteAsync();
            return defaultConfig;
        }

        private static TaxConfigurationDto MapToDto(TaxConfiguration config) => new()
        {
            Id = config.Id,
            GeneralItbisPercentage = config.GeneralItbisPercentage,
            CustomsServiceFeePercentage = config.CustomsServiceFeePercentage
        };
    }
}