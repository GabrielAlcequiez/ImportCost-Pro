using System;

namespace ImportCostPro.BusinessLogic.DTOs.Supplier
{
    public class CreateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public Guid CurrencyId { get; set; }
    }
}
