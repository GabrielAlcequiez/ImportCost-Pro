using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.Web.ViewModels.ImportOrder
{
    public class ImportOrderIndexViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public ImportOrderStatus Status { get; set; }
        public string ImporterName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal ExchangeRateValue { get; set; }
        public int DetailCount { get; set; }
    }
}
