using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations;

public class ImporterConfiguration : IEntityTypeConfiguration<Importer>
{
    public void Configure(EntityTypeBuilder<Importer> builder)
    {
        builder.ToTable("Importers", table =>
        {
            table.HasCheckConstraint("CK_Importers_CountryId_NotEmpty", "[CountryId] <> '00000000-0000-0000-0000-000000000000'");
        });

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.TaxId)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(i => i.TaxId)
            .IsUnique();

        builder.Property(i => i.Phone)
            .HasMaxLength(20);

        builder.Property(i => i.Email)
            .HasMaxLength(100);

        builder.Property(i => i.Address)
            .HasMaxLength(250);

        builder.Property(i => i.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(i => i.Country)
            .WithMany()
            .HasForeignKey(i => i.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
