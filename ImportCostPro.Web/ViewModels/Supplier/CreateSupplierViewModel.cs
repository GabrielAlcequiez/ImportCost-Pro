using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.ViewModels.Supplier
{
    public class CreateSupplierViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país es requerido")]
        public Guid CountryId { get; set; }

        public string? Email { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
        public string? Telephone { get; set; }

        [Required(ErrorMessage = "La moneda es requerida")]
        public Guid CurrencyId { get; set; }

        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
    }
}
