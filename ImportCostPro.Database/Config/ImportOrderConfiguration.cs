using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations
{
    public class ImportOrderConfiguration : IEntityTypeConfiguration<ImportOrder>
    {
        public void Configure(EntityTypeBuilder<ImportOrder> builder)
        {
            builder.ToTable("ImportOrders");
            builder.HasKey(o => o.Id);

            // Validaciones estructurales en DB
            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(30); // Máximo 30 caracteres (Pág. 76)

            // Índice único para evitar duplicados en la base de datos (Pág. 76)
            builder.HasIndex(o => o.OrderNumber)
                .IsUnique();

            builder.Property(o => o.OrderDate)
                .IsRequired();

            builder.Property(o => o.ExchangeRateValue)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            // Mapeo de Enums a enteros
            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(o => o.TransportMode)
                .IsRequired()
                .HasConversion<int>();

            // Relaciones de claves foráneas con borrado restringido (Pág. 140 - No borrar maestros usados)
            builder.HasOne(o => o.Importer)
                .WithMany()
                .HasForeignKey(o => o.ImporterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Supplier)
                .WithMany()
                .HasForeignKey(o => o.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Country)
                .WithMany()
                .HasForeignKey(o => o.CountryId)
                .OnDelete(DeleteBehavior.Restrict); // <-- Relación con País de origen

            builder.HasOne(o => o.Currency)
                .WithMany()
                .HasForeignKey(o => o.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
