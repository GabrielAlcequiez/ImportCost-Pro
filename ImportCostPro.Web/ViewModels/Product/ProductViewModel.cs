using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.Web.ViewModels.Product
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CodeReference { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string TariffCategoryName { get; set; } = string.Empty;
        public string TariffCode { get; set; } = string.Empty;
        public decimal TariffPercentage { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
