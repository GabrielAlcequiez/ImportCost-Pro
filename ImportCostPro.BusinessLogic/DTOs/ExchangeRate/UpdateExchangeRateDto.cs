using System;

namespace ImportCostPro.BusinessLogic.DTOs.ExchangeRate
{
    public class UpdateExchangeRateDto
    {
        public Guid Id { get; set; }
        public decimal RateValue { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}
