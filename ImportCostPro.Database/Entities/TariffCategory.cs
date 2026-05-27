namespace ImportCostPro.Database.Entities;

public class TariffCategory
{
    public Guid Id { get; private set; }
    public string TariffCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    public decimal TariffPercentage { get; private set; }
    public bool ApplyITBIS { get; private set; }
    public bool ApplyExciseTax { get; private set; }
    public decimal ExciseTaxPercentage { get; private set; }
    public bool IsActive {get; private set; }

    public TariffCategory(string tariffCode, string name, decimal tariffPercentage, bool applyItbis, bool applyExciseTax)
    {
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