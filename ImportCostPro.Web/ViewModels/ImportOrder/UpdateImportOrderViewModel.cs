using System.ComponentModel.DataAnnotations;
using ImportCostPro.Database.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.ViewModels.ImportOrder
{
    public class UpdateImportOrderViewModel
    {
        [Required]
        public Guid Id { get; set; }

        public ImportOrderStatus Status { get; set; }

        [Required(ErrorMessage = "El número de orden es requerido.")]
        [StringLength(20, ErrorMessage = "El número de orden no puede exceder 20 caracteres.")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de orden es requerida.")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "El importador es requerido.")]
        public Guid ImporterId { get; set; }

        [Required(ErrorMessage = "El proveedor es requerido.")]
        public Guid SupplierId { get; set; }

        [Required(ErrorMessage = "El país de origen es requerido.")]
        public Guid CountryId { get; set; }

        [Required(ErrorMessage = "La moneda es requerida.")]
        public Guid CurrencyId { get; set; }

        [Required(ErrorMessage = "La tasa de cambio es requerida.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "La tasa de cambio debe ser mayor que 0.")]
        public decimal ExchangeRateValue { get; set; }

        [Required(ErrorMessage = "El modo de transporte es requerido.")]
        public TransportMode TransportMode { get; set; }

        public string? ImporterName { get; set; }
        public string? SupplierName { get; set; }
        public string? CountryName { get; set; }
        public string? CurrencyCode { get; set; }

        public List<ImportOrderDetailViewModel> Details { get; set; } = new();
        public List<ImportOrderExpenseViewModel> Expenses { get; set; } = new();

        public List<SelectListItem> Importers { get; set; } = new();
        public List<SelectListItem> Suppliers { get; set; } = new();
        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
        public List<SelectListItem> TransportModes { get; set; } = new();

        public List<SelectListItem> AvailableProducts { get; set; } = new();
        public List<SelectListItem> ExpenseCurrencies { get; set; } = new();
        public List<SelectListItem> ExpenseTypesList { get; set; } = new();
        public List<SelectListItem> ApportionmentMethodsList { get; set; } = new();
    }
}
