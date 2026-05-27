namespace ImportCostPro.Database.Entities;
public class Product
{
    public Guid Id {get; private set;}
    public string Name {get; private set;}= string.Empty;
    public string CodeReference {get; private set;} = string.Empty;
    public Guid CountryId {get; private set;}
    public Guid TariffCategoryId {get; private set;}
    public decimal UnitWeight {get; private set;}
    public decimal? Length {get; private set;}
    public decimal? Width {get; private set;}
    public decimal? Height {get; private set;}
    public UnitOfMeasure UnitOfMeasure {get; private set;}
    public string? Description {get; private set;}
    public bool IsActive {get; private set;}

    public Product(string name, string codeReference, Guid countryId, Guid tariffCategoryId, decimal unitWeight, decimal? length, decimal? width, decimal? height, UnitOfMeasure uom, string? description)
    {
        Id = Guid.NewGuid();
        Name = name.Trim();
        CodeReference = codeReference.Trim();
        CountryId = countryId;
        TariffCategoryId = tariffCategoryId;
        UnitWeight = unitWeight;
        Length = length;
        Width = width;
        Height = height;
        UnitOfMeasure = uom;
        Description = description?.Trim();
        IsActive = true;
    }

    public void Update(string name, string codeReference, Guid countryId, Guid tariffCategoryId, decimal unitWeight, decimal? length, decimal? width, decimal? height, UnitOfMeasure uom, string? description, bool isactive)
    {
        Name = name.Trim();
        CodeReference = codeReference.Trim();
        CountryId = countryId;
        TariffCategoryId = tariffCategoryId;
        UnitWeight = unitWeight;
        Length = length;
        Width = width;
        Height = height;
        UnitOfMeasure = uom;
        Description = description?.Trim();
        IsActive = isactive;
    }
}