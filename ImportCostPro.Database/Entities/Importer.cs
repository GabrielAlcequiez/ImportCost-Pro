namespace ImportCostPro.Database.Entities;

public class Importer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;
    public Guid CountryId { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }

    public Country Country { get; private set; } = null!;

    private Importer() { }

    public Importer(string name, string taxId, Guid countryId, string? phone, string? email, string? address)
    {
        Id = Guid.NewGuid();
        Name = name.Trim();
        TaxId = taxId.Trim();
        CountryId = countryId;
        Phone = phone?.Trim();
        Email = email?.Trim();
        Address = address?.Trim();
        IsActive = true;
    }

    public void Update(string name, Guid countryId, string? phone, string? email, string? address, bool isActive)
    {
        Name = name.Trim();
        CountryId = countryId;
        Phone = phone?.Trim();
        Email = email?.Trim();
        Address = address?.Trim();
        IsActive = isActive;
    }
}
