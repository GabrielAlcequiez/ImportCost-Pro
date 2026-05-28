using System;

namespace ImportCostPro.BusinessLogic.DTOs.ImportOrder
{
    public class UpdateImportOrderDetailDto
    {
        public Guid Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCostFob { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal CustomDutyPercentage { get; set; }
        public decimal DesiredProfitMargin { get; set; }
    }
}
