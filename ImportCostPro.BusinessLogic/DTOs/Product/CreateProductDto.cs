using System;
using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.BusinessLogic.DTOs.Product
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string CodeReference { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public Guid TariffCategoryId { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public string? Description { get; set; }
    }
}
