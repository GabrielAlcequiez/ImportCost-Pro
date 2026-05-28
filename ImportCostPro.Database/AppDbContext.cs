using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ImportCostPro.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Importer> Importers => Set<Importer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<TariffCategory> TariffCategories => Set<TariffCategory>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<ImportOrder> ImportOrders => Set<ImportOrder>();
    public DbSet<ImportOrderDetail> ImportOrderDetails => Set<ImportOrderDetail>();
    public DbSet<ImportOrderExpense> ImportOrderExpenses => Set<ImportOrderExpense>();
    public DbSet<TaxConfiguration> TaxConfigurations => Set<TaxConfiguration>();
    public DbSet<LandedCostCalculation> LandedCostCalculations => Set<LandedCostCalculation>();
    public DbSet<LandedCostCalculationDetail> LandedCostCalculationDetails => Set<LandedCostCalculationDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
