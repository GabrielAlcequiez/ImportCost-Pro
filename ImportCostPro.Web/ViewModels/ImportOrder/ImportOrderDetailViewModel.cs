namespace ImportCostPro.Web.ViewModels.ImportOrder
{
    public class ImportOrderDetailViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCodeReference { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitCostFob { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal CustomDutyPercentage { get; set; }
        public decimal DesiredProfitMargin { get; set; }
        public decimal TotalCostFob => Quantity * UnitCostFob;
        public decimal TotalWeight => Quantity * UnitWeight;
    }
}
