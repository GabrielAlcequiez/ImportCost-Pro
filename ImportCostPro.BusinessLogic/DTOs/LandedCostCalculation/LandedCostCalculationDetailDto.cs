using System;

namespace ImportCostPro.BusinessLogic.DTOs.LandedCostCalculation
{
    public class LandedCostCalculationDetailDto
    {
        public Guid Id { get; set; }
        public Guid LandedCostCalculationId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCodeReference { get; set; } = string.Empty;

        public decimal Quantity { get; set; }
        
        public decimal TariffPercentageUsed { get; set; }
        public decimal ExciseTaxPercentageUsed { get; set; }

        public decimal FobOriginal { get; set; }
        public decimal FobLocal { get; set; }
        public decimal FreightAssigned { get; set; }
        public decimal InsuranceAssigned { get; set; }
        public decimal CifLocal { get; set; }
        public decimal DutyCalculated { get; set; }
        public decimal SelectiveTaxCalculated { get; set; }
        public decimal CustomsServiceFeeCalculated { get; set; }
        public decimal ItbisCalculated { get; set; }
        public decimal LocalExpensesAssigned { get; set; }
        public decimal ImportTotalCost { get; set; }
        public decimal UnitLandedCost { get; set; }
        
        public decimal DesiredProfitMargin { get; set; }
        public decimal SuggestedSellingPrice { get; set; }
    }
}
