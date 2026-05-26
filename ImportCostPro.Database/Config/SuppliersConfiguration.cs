using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SuppliersConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("Suppliers");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x=>x.CountryId)
            .IsRequired();

        builder.Property(x=>x.CurrencyId)
            .IsRequired();

        builder.Property(x=>x.Email)
            .HasMaxLength(100);

        builder.Property(x=>x.Telephone)
            .HasMaxLength(20);

        builder.Property(x=>x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}