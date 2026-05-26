namespace ImportCostPro.Database.Entities;

public class ExchangeRate
{
    public Guid Id { get; private set; }
    public Guid SourceCurrencyId { get; private set; } // Moneda origen (e.g., USD)
    public Guid TargetCurrencyId { get; private set; } // Moneda destino (Moneda local, e.g., DOP)
    public decimal RateValue { get; private set; }       // Valor de la tasa
    public DateTime EffectiveDate { get; private set; }   // Fecha de vigencia
    public bool IsActive { get; private set; }

    // Navigation properties for EF Core relationships
    public Currency SourceCurrency { get; private set; } = null!;
    public Currency TargetCurrency { get; private set; } = null!;

    private ExchangeRate() { }

    public ExchangeRate(Guid sourceCurrencyId, Guid targetCurrencyId, decimal rateValue, DateTime effectiveDate)
    {
        Id = Guid.NewGuid();
        SourceCurrencyId = sourceCurrencyId;
        TargetCurrencyId = targetCurrencyId;
        RateValue = rateValue;
        EffectiveDate = effectiveDate.Date; // Truncating time to keep only the pure date segment
        IsActive = true;                    // Default active status on creation
    }

    public void Update(decimal rateValue, DateTime effectiveDate, bool isActive)
    {
        RateValue = rateValue;
        EffectiveDate = effectiveDate.Date;
        IsActive = isActive;
    }
}