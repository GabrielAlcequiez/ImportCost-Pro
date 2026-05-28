namespace ImportCostPro.BusinessLogic.DTOs.Currency
{
    public class CreateCurrencyDto
    {
        public string Name { get; set; } = string.Empty;
        public string ISOCode { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public bool IsLocalCurrency { get; set; }
    }
}
