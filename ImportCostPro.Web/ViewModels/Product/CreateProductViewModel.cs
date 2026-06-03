using System.ComponentModel.DataAnnotations;
using ImportCostPro.Database.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.ViewModels.Product
{
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(150, ErrorMessage = "El nombre no puede tener más de 150 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código de referencia es requerido")]
        [StringLength(50, ErrorMessage = "El código de referencia no puede tener más de 50 caracteres")]
        public string CodeReference { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país es requerido")]
        public Guid CountryId { get; set; }

        [Required(ErrorMessage = "La categoría arancelaria es requerida")]
        public Guid TariffCategoryId { get; set; }

        [Required(ErrorMessage = "El peso unitario es requerido")]
        [Range(0.001, double.MaxValue, ErrorMessage = "El peso debe ser mayor que 0")]
        public decimal UnitWeight { get; set; }

        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        [Required(ErrorMessage = "La unidad de medida es requerida")]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [StringLength(250, ErrorMessage = "La descripción no puede tener más de 250 caracteres")]
        public string? Description { get; set; }

        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> TariffCategories { get; set; } = new();
    }
}
