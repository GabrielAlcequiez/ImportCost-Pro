using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations;

public class TariffCategoryConfiguration : IEntityTypeConfiguration<TariffCategory>
{
    public void Configure(EntityTypeBuilder<TariffCategory> builder)
    {
        builder.ToTable("TariffCategories");
        builder.HasKey(c => c.Id);

        builder.Property(x => x.TariffCode)
            .IsRequired()
            .HasMaxLength(20);
        builder.HasIndex(x => x.TariffCode)
            .IsUnique();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.TariffPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");
        
        builder.Property(x=>x.ApplyITBIS)
            .IsRequired();

        builder.Property(x=>x.ApplyExciseTax)
            .IsRequired();

        // Condicional por el momento, dejarlo como requerido
        // según reglas asumo no tendria problema en dejarlo asi.
        builder.Property(x=>x.ExciseTaxPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
