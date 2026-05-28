using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Identity.Client;

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

    // Navigation Properties for fk
    public Country Country {get; private set;} = null!;
    public Currency Currency {get; private set;} = null!;
    private Supplier(){}

    public Supplier(string name, Guid countryId, string? telephone, string? email, Guid currencyId)
    {
        Id = Guid.NewGuid();
        Name = name.Trim();
        CountryId = countryId;
        Email = email?.Trim();
        Telephone = telephone?.Trim();
        CurrencyId = currencyId;
        IsActive = true;
    }

    public void Update(string name, Guid countryId, string? telephone, string? email, Guid currencyId, bool isactive)
    {
        Name = name.Trim();
        CountryId = countryId;
        Email = email?.Trim();
        Telephone = telephone?.Trim();
        CurrencyId = currencyId;
        IsActive = isactive;
    }
}