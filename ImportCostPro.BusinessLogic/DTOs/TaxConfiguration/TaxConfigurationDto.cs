using System;

namespace ImportCostPro.BusinessLogic.DTOs.TaxConfiguration
{
    public class TaxConfigurationDto
    {
        public Guid Id { get; set; }
        public decimal GeneralItbisPercentage { get; set; }
        public decimal CustomsServiceFeePercentage { get; set; }
    }
}
