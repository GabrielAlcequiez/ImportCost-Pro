namespace ImportCostPro.Database.Entities;

public class Country
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ISOCode { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private Country() { }

    public Country(string name, string isoCode)
    {
        Id = Guid.NewGuid();
        Name = name.Trim();
        ISOCode = isoCode.Trim().ToUpperInvariant();
        IsActive = true;
    }

    public void Update(string name, string isoCode, bool isActive)
    {
        Name = name.Trim();
        ISOCode = isoCode.Trim().ToUpperInvariant();
        IsActive = isActive;
    }
}
