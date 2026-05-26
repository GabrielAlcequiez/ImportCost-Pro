namespace ImportCostPro.Database.Entities;

public class Currency
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ISOCode { get; private set; } = string.Empty;
    public string Symbol { get; private set; } = string.Empty;
    public bool IsLocalCurrency { get; private set; }
    public bool IsActive { get; private set; }

    private Currency() { }

    public Currency(string name, string isoCode, string symbol, bool isLocalCurrency)
    {
        Id = Guid.NewGuid();
        Name = name.Trim();
        ISOCode = isoCode.Trim().ToUpperInvariant();
        Symbol = symbol.Trim();
        IsLocalCurrency = isLocalCurrency;
        IsActive = true;
    }

    public void Update(string name, string isoCode, string symbol, bool isLocalCurrency, bool isActive)
    {
        Name = name.Trim();
        ISOCode = isoCode.Trim().ToUpperInvariant();
        Symbol = symbol.Trim();
        IsLocalCurrency = isLocalCurrency;
        IsActive = isActive;
    }

    public void MarkAsLocal(bool isLocal)
    {
        IsLocalCurrency = isLocal;
    }
}
