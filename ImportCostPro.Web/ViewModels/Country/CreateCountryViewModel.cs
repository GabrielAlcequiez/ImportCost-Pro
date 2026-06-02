using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.Web.ViewModels.Country
{
    public class CreateCountryViewModel
    {
        [Required(ErrorMessage = "Debes de ingresar el nombre del país")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Debes ingresar el código ISO")]
        public string ISOCode { get; set; } = string.Empty;
    }
}