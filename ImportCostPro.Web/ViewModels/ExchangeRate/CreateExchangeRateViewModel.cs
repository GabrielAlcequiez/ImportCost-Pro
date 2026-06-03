using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.ViewModels.ExchangeRate
{
    public class CreateExchangeRateViewModel
    {
        [Required(ErrorMessage = "La moneda origen es requerida")]
        public Guid SourceCurrencyId { get; set; }

        [Required(ErrorMessage = "La moneda destino es requerida")]
        public Guid TargetCurrencyId { get; set; }

        [Required(ErrorMessage = "La tasa de cambio es requerida")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "La tasa debe ser mayor que 0")]
        public decimal RateValue { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; } = DateTime.Today;

        public List<SelectListItem> Currencies { get; set; } = new();
    }
}
