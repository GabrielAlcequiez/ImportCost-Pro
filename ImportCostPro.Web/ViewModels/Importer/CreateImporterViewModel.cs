using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.ViewModels.Importer
{
    public class CreateImporterViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El RNC / cédula fiscal es requerido")]
        public string TaxId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país es requerido")]
        public Guid CountryId { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        public string? Email { get; set; }

        public string? Address { get; set; }

        // Para llenar el <select> en la vista
        public List<SelectListItem> Countries { get; set; } = new();
    }
}
