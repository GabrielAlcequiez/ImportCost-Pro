using System;

namespace ImportCostPro.BusinessLogic.DTOs.ExchangeRate
{
    public class CreateExchangeRateDto
    {
        public Guid SourceCurrencyId { get; set; }
        public Guid TargetCurrencyId { get; set; }
        public decimal RateValue { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
