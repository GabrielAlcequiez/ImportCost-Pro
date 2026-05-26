namespace ImportCostPro.Database.Entities;
public class Supplier
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid CountryId {get; private set;} // FK - ID del País
    // VALIDACIÓN PENDIENTE
    public string? Email {get; private set;} 
    public string? Telephone {get; private set;}
    public Guid CurrencyId {get; private set;}
    public bool IsActive {get; private set;}

    private Supplier(){}

    public Supplier(string name, Guid countryId, string? telephone, string? email, Guid currencyId)
    {
        Id = Guid.NewGuid();
        Name = name.Trim();
        CountryId = countryId;
        Telephone = telephone;
        Email = email;
        CurrencyId = currencyId;
        IsActive = true;
    }
}