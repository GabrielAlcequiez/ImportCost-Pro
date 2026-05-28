namespace ImportCostPro.Database.Entities;
public class TaxConfiguration
{
    public Guid Id { get; private set; }
    public decimal GeneralItbisPercentage { get; private set; } 
    public decimal CustomsServiceFeePercentage { get; private set; } 

    private TaxConfiguration(){}
    public TaxConfiguration(decimal generalItbisPercentage, decimal customsServiceFeePercentage)
    {
        ValidatePercentages(generalItbisPercentage, customsServiceFeePercentage);

        Id = Guid.NewGuid();
        GeneralItbisPercentage = generalItbisPercentage;
        CustomsServiceFeePercentage = customsServiceFeePercentage;
    }

    public void Update(decimal generalItbis, decimal customsServiceFee)
    {
        ValidatePercentages(generalItbis, customsServiceFee);

        GeneralItbisPercentage = generalItbis;
        CustomsServiceFeePercentage = customsServiceFee;
    }

    private void ValidatePercentages(decimal itbis, decimal customsFee)
    {
        if (itbis < 0 || itbis > 100)
            throw new ArgumentException("El porcentaje general de ITBIS debe estar entre 0 y 100.");
            
        if (customsFee < 0 || customsFee > 100)
            throw new ArgumentException("El porcentaje de la tasa de servicio aduanal debe estar entre 0 y 100.");
    }
}