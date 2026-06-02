namespace ImportCostPro.Database.Repositories.Interfaces
{
    // Creada exclusivamente para controlar el save changes async, asi tener mas control en transacciones
    public interface IUnitOfWork : IDisposable
    {
        ICountryRepository Countries { get; }
        ICurrencyRepository Currencies { get; }
        IImporterRepository Importers { get; }
        ISupplierRepository Suppliers { get; }
        IProductRepository Products {get;}
        ITariffCategoryRepository TariffCategories {get;}
        IExchangeRateRepository ExchangeRates {get;}
        IImportOrderRepository ImportOrders { get; }
        ITaxConfigurationRepository TaxConfigurations { get; }
        Task<int> CompleteAsync();
    }
}