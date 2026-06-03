
using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Web.ViewModels.Country
{
    public class UpdateCountryViewModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string ISOCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}