using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // Propiedades de los repos
        public ICountryRepository Countries { get; }
        public ICurrencyRepository Currencies { get; }

        public IImporterRepository Importers { get; }

        public ISupplierRepository Suppliers { get; }

        public IProductRepository Products {get;}
        public ITariffCategoryRepository TariffCategories {get;}
        public IImportOrderRepository ImportOrders { get; }


        //Pon lo tuyo antes de esto  (para que vaya en orden) y borra este comentario cuando lo hagas
        public ITaxConfigurationRepository TaxConfigurations { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Countries = new CountryRepository(_context);
            Currencies = new CurrencyRepository(_context);
            Importers = new ImporterRepository(_context);
            Suppliers = new SupplierRepository(_context);
            Products = new ProductRepository(_context);
            TariffCategories = new TariffCategoryRepository(_context);
            ImportOrders = new ImportOrderRepository(_context);
            TaxConfigurations = new TaxConfigurationRepository(_context);
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}