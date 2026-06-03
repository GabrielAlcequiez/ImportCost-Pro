using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Web.ViewModels.TaxConfiguration
{
    public class UpdateTaxConfigurationViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El porcentaje de ITBIS general es requerido")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        [Display(Name = "ITBIS General (%)")]
        public decimal GeneralItbisPercentage { get; set; }

        [Required(ErrorMessage = "El porcentaje de honorarios de aduana es requerido")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        [Display(Name = "Honorarios de Aduana (%)")]
        public decimal CustomsServiceFeePercentage { get; set; }
    }
}
