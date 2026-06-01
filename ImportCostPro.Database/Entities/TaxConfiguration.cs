namespace ImportCostPro.Database.Entities;
public class TaxConfiguration
{
    public Guid Id { get; private set; }
    public decimal GeneralItbisPercentage { get; private set; } 
    public decimal CustomsServiceFeePercentage { get; private set; } 

    private TaxConfiguration(){}
    public TaxConfiguration(decimal generalItbisPercentage, decimal customsServiceFeePercentage)
    {
        Id = Guid.NewGuid();
        GeneralItbisPercentage = generalItbisPercentage;
        CustomsServiceFeePercentage = customsServiceFeePercentage;
    }

    public void Update(decimal generalItbis, decimal customsServiceFee)
    {
        GeneralItbisPercentage = generalItbis;
        CustomsServiceFeePercentage = customsServiceFee;
    }

 
}