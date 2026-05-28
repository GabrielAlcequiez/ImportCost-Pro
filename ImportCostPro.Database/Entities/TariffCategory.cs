namespace ImportCostPro.Database.Entities;

public class TariffCategory
{
    public Guid Id { get; private set; }
    public string TariffCode { get; private set; } = string.Empty; // Código Arancelario
    public string Name { get; private set; } = string.Empty; 

    public decimal TariffPercentage { get; private set; } // Porcentaje de Arancel
    public bool ApplyITBIS { get; private set; } // Aplica ITBIS
    public bool ApplyExciseTax { get; private set; } // Aplica impuesto selectivo
    public decimal ExciseTaxPercentage { get; private set; } // Porcentaje de impuesto selectivo
    public bool IsActive { get; private set; }

    
    private TariffCategory() { }
    public TariffCategory(string tariffCode, string name, decimal tariffPercentage, bool applyItbis, bool applyExciseTax)
    {
        Id = Guid.NewGuid();
        TariffCode = tariffCode.Trim();
        Name = name.Trim();
        TariffPercentage = tariffPercentage;
        ApplyITBIS = applyItbis;
        ApplyExciseTax = applyExciseTax;
        IsActive = true;
    }

    public void Update(string tariffCode, string name, decimal tariffPercentage, bool applyItbis, bool applyExciseTax, bool isactive)
    {
        TariffCode = tariffCode.Trim();
        Name = name.Trim();
        TariffPercentage = tariffPercentage;
        ApplyITBIS = applyItbis;
        ApplyExciseTax = applyExciseTax;
        IsActive = isactive;
    }
}