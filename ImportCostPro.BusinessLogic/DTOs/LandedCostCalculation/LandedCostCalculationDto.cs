using System;
using System.Collections.Generic;

namespace ImportCostPro.BusinessLogic.DTOs.LandedCostCalculation
{
    public class LandedCostCalculationDto
    {
        public Guid Id { get; set; }
        public Guid ImportOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime CalculationDate { get; set; }
        
        public Guid LocalCurrencyId { get; set; }
        public string LocalCurrencyCode { get; set; } = string.Empty;
        public decimal ExchangeRateUsed { get; set; }
        public decimal GeneralItbisPercentageUsed { get; set; }
        public decimal CustomsServiceFeePercentageUsed { get; set; }

        public decimal FobTotalOriginal { get; set; }
        public decimal FobTotalLocal { get; set; }
        public decimal FreightTotalLocal { get; set; }
        public decimal InsuranceTotalLocal { get; set; }
        public decimal CifTotalLocal { get; set; }
        public decimal DutyTotalLocal { get; set; }
        public decimal SelectiveTaxTotalLocal { get; set; }
        public decimal CustomsServiceFeeTotalLocal { get; set; }
        public decimal ItbisTotalLocal { get; set; }
        public decimal LocalExpensesTotalLocal { get; set; }
        public decimal ImportTotalCost { get; set; }
        public decimal TotalQuantity { get; set; }

        public List<LandedCostCalculationDetailDto> Details { get; set; } = new();
    }
}
