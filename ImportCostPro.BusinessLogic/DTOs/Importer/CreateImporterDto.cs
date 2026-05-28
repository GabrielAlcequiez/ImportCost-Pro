using System;

namespace ImportCostPro.BusinessLogic.DTOs.Importer
{
    public class CreateImporterDto
    {
        public string Name { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public Guid CountryId { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
