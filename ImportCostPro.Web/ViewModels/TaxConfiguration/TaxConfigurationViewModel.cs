namespace ImportCostPro.Web.ViewModels.TaxConfiguration
{
    public class TaxConfigurationViewModel
    {
        public Guid Id { get; set; }
        public decimal GeneralItbisPercentage { get; set; }
        public decimal CustomsServiceFeePercentage { get; set; }
    }
}
