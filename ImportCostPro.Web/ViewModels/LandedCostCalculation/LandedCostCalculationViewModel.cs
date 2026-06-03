using System;
using System.Collections.Generic;
using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.Web.ViewModels.LandedCostCalculation
{
    public class LandedCostCalculationViewModel
    {
        public Guid Id { get; set; }
        public Guid ImportOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime CalculationDate { get; set; }
        public ImportOrderStatus OrderStatus { get; set; }

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

        public List<LandedCostCalculationDetailViewModel> Details { get; set; } = new();
    }

    public class LandedCostCalculationDetailViewModel
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
