namespace ImportCostPro.BusinessLogic.DTOs.ExchangeRate
{
    public class UpdateExchangeRateDto
    {
        public Guid Id { get; set; }
        public Guid SourceCurrencyId { get; set; }
        public Guid TargetCurrencyId { get; set; }
        public decimal RateValue { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}
