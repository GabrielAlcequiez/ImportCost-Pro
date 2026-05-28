using System;
using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.BusinessLogic.DTOs.ImportOrder
{
    public class UpdateImportOrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public TransportMode TransportMode { get; set; }

        public Guid ImporterId { get; set; }
        public Guid SupplierId { get; set; }
        public Guid CountryId { get; set; }
        public Guid CurrencyId { get; set; }
        public decimal ExchangeRateValue { get; set; }
    }
}
