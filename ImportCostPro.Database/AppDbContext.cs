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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
