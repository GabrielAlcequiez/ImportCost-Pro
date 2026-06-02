using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Web.ViewModels.Currency
{
    public class CurrencyViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ISOCode { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public bool IsLocalCurrency { get; set; }
        public bool IsActive { get; set; }
    }
}
