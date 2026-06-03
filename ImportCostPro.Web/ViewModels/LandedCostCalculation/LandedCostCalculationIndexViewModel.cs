using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.Web.ViewModels.LandedCostCalculation
{
    public class LandedCostCalculationIndexViewModel
    {
        // For the dropdown select list
        public Guid? SelectedImportOrderId { get; set; }
        public List<SelectListItem> OpenOrders { get; set; } = new();

        // For the history list
        public List<LandedCostHistoryItemViewModel> HistoryItems { get; set; } = new();
    }

    public class LandedCostHistoryItemViewModel
    {
        public Guid CalculationId { get; set; }
        public Guid ImportOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime CalculationDate { get; set; }
        public string ImporterName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public decimal ImportTotalCost { get; set; }
        public string LocalCurrencyCode { get; set; } = string.Empty;
        public ImportOrderStatus OrderStatus { get; set; }
    }
}
