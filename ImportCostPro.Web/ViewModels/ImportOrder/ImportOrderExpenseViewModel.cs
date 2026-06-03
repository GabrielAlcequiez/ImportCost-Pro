using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.Web.ViewModels.ImportOrder
{
    public class ImportOrderExpenseViewModel
    {
        public Guid Id { get; set; }
        public Guid CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public ExpenseType ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public decimal ExchangeRateValue { get; set; }
        public ApportionmentMethod ApportionmentMethod { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal AmountInLocalCurrency => Amount * ExchangeRateValue;
    }
}
