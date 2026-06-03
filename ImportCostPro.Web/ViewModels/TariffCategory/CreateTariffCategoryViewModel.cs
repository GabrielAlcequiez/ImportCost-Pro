using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Web.ViewModels.TariffCategory
{
    public class CreateTariffCategoryViewModel
    {
        [Required(ErrorMessage = "El código arancelario es requerido")]
        public string TariffCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El porcentaje arancelario es requerido")]
        [Range(0, 100, ErrorMessage = "Debe estar entre 0 y 100")]
        public decimal TariffPercentage { get; set; }

        public bool ApplyITBIS { get; set; }
        public bool ApplyExciseTax { get; set; }

        [Range(0, 100, ErrorMessage = "Debe estar entre 0 y 100")]
        public decimal ExciseTaxPercentage { get; set; }
    }
}
