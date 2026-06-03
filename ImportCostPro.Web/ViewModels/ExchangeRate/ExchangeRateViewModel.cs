namespace ImportCostPro.Web.ViewModels.ExchangeRate
{
    public class ExchangeRateViewModel
    {
        public Guid Id { get; set; }
        public Guid SourceCurrencyId { get; set; }
        public string SourceCurrencyCode { get; set; } = string.Empty;
        public string SourceCurrencyName { get; set; } = string.Empty;
        public Guid TargetCurrencyId { get; set; }
        public string TargetCurrencyCode { get; set; } = string.Empty;
        public string TargetCurrencyName { get; set; } = string.Empty;
        public decimal RateValue { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}
