namespace ImportCostPro.BusinessLogic.DTOs.TariffCategory
{
    public class CreateTariffCategoryDto
    {
        public string TariffCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal TariffPercentage { get; set; }
        public bool ApplyITBIS { get; set; }
        public bool ApplyExciseTax { get; set; }
        public decimal ExciseTaxPercentage { get; set; }
    }
}
