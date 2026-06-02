using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Web.ViewModels.Currency
{
    public class CreateCurrencyViewModel
    {
        [Required(ErrorMessage = "El nombre de la moneda es requerido")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código ISO es requerido")]
        public string ISOCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "El símbolo es requerido")]
        public string Symbol { get; set; } = string.Empty;

        public bool IsLocalCurrency { get; set; }
    }
}
