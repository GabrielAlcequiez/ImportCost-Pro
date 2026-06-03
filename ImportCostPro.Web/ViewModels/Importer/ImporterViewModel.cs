using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Web.ViewModels.Importer
{
    public class ImporterViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }
}
