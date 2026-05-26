using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired();

        builder.Property(c => c.Symbol)
            .IsRequired();

        builder.HasExactIsoCodeRule(nameof(Currency.ISOCode), 3);

        builder.HasIndex(c => c.ISOCode)
            .IsUnique();

        builder.HasIndex(c => c.IsLocalCurrency)
            .IsUnique()
            .HasFilter("[IsLocalCurrency] = 1");

        builder.Property(c => c.IsLocalCurrency)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired();
    }
}
