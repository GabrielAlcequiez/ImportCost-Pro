using System;

namespace ImportCostPro.BusinessLogic.DTOs.ImportOrder
{
    public class CreateImportOrderDetailDto
    {
        public Guid ImportOrderId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCostFob { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal CustomDutyPercentage { get; set; }
        public decimal DesiredProfitMargin { get; set; }
    }
}
