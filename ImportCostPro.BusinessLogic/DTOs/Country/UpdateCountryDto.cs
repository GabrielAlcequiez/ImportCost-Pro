using System;

namespace ImportCostPro.BusinessLogic.DTOs.Country
{
    public class UpdateCountryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ISOCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
