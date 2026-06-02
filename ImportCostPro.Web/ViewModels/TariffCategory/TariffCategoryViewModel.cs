using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Web.ViewModels.TariffCategory
{
    public class TariffCategoryViewModel
    {
        public Guid Id { get; set; }
        public string TariffCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal TariffPercentage { get; set; }
        public bool ApplyITBIS { get; set; }
        public bool ApplyExciseTax { get; set; }
        public decimal ExciseTaxPercentage { get; set; }
        public bool IsActive { get; set; }
    }
}
