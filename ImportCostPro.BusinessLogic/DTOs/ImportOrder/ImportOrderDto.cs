using System;
using System.Collections.Generic;
using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.BusinessLogic.DTOs.ImportOrder
{
    public class ImportOrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public ImportOrderStatus Status { get; set; }
        public TransportMode TransportMode { get; set; }

        public Guid ImporterId { get; set; }
        public string ImporterName { get; set; } = string.Empty;
        
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        
        public Guid CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        
        public Guid CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string CurrencyName { get; set; } = string.Empty;
        public decimal ExchangeRateValue { get; set; }

        public List<ImportOrderDetailDto> Details { get; set; } = new();
        public List<ImportOrderExpenseDto> Expenses { get; set; } = new();
    }
}
