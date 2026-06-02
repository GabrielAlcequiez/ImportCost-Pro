namespace ImportCostPro.Database.Repositories.Interfaces
{
    // Creada exclusivamente para controlar el save changes async, asi tener mas control en transacciones
    public interface IUnitOfWork : IDisposable
    {
        ICountryRepository Countries { get; }
        ICurrencyRepository Currencies { get; }
        IImporterRepository Importers { get; }
        ISupplierRepository Suppliers { get; }
        IImportOrderRepository ImportOrders { get; }
        ILandedCostCalculationRepository LandedCostCalculations { get; }
        ITaxConfigurationRepository TaxConfigurations { get; }
        Task<int> CompleteAsync();
    }
}