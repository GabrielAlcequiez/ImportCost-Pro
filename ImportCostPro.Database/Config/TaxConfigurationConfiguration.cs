using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations;

public class TaxConfigurationConfiguration : IEntityTypeConfiguration<TaxConfiguration>
{
    public void Configure(EntityTypeBuilder<TaxConfiguration> builder)
    {
        builder.ToTable("TaxConfigurations");

        // Llave primaria
        builder.HasKey(t => t.Id);

        // Porcentaje General de ITBIS
        builder.Property(t => t.GeneralItbisPercentage)
            .IsRequired()
            .HasColumnType("decimal(5,2)"); 

        // Porcentaje de Tasa de Servicio Aduanal
        builder.Property(t => t.CustomsServiceFeePercentage)
            .IsRequired()
            .HasColumnType("decimal(5,4)"); 
    }
}