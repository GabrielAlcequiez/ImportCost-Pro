namespace ImportCostPro.Web.ViewModels.Supplier
{
    public class SupplierViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public Guid CurrencyId { get; set; }
        public string CurrencyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
