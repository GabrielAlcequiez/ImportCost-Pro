using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations;
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("Products");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.CodeReference)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(x => x.CodeReference)
            .IsUnique();

        builder.Property(x => x.CountryId)
            .IsRequired();

        builder.Property(x => x.TariffCategoryId)
            .IsRequired();

        builder.Property(x => x.UnitWeight)
            .IsRequired();

        // Mapeados a decimal(18,4 y 18,2) por defecto para mejorar calculos

        builder.Property(x => x.UnitWeight)
        .IsRequired()
        .HasColumnType("decimal(18,4)"); 

        builder.Property(x => x.Length)
            .HasColumnType("decimal(18,2)"); 

        builder.Property(x => x.Width)
            .HasColumnType("decimal(18,2)"); 

        builder.Property(x => x.Height)
            .HasColumnType("decimal(18,2)"); 

        builder.Property(x => x.UnitOfMeasure)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(250);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}