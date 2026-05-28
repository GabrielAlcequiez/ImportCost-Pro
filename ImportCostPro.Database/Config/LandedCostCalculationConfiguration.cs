using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations
{
    public class LandedCostCalculationConfiguration : IEntityTypeConfiguration<LandedCostCalculation>
    {
        public void Configure(EntityTypeBuilder<LandedCostCalculation> builder)
        {
            builder.ToTable("LandedCostCalculations");
            builder.HasKey(c => c.Id);

            // 1. Relación 1 a 1: Una orden solo puede tener UN cálculo oficial [cite: 1864]
            builder.HasIndex(c => c.ImportOrderId)
                .IsUnique();

            builder.HasOne(c => c.ImportOrder)
                .WithMany() // Unidireccional desde la orden
                .HasForeignKey(c => c.ImportOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Relación con la Moneda Local
            builder.HasOne(c => c.LocalCurrency)
                .WithMany()
                .HasForeignKey(c => c.LocalCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.CalculationDate)
                .IsRequired();

            // 3. Precisiones de variables congeladas (Porcentajes y Tasas)
            builder.Property(c => c.ExchangeRateUsed).HasColumnType("decimal(18,4)");
            builder.Property(c => c.GeneralItbisPercentageUsed).HasColumnType("decimal(5,2)");
            builder.Property(c => c.CustomsServiceFeePercentageUsed).HasColumnType("decimal(5,2)");

            builder.Property(c => c.FobTotalOriginal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.FobTotalLocal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.FreightTotalLocal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.InsuranceTotalLocal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CifTotalLocal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.DutyTotalLocal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.SelectiveTaxTotalLocal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CustomsServiceFeeTotalLocal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.ItbisTotalLocal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.LocalExpensesTotalLocal).HasColumnType("decimal(18,2)");
            builder.Property(c => c.ImportTotalCost).HasColumnType("decimal(18,2)");
            
            // Cantidades pueden requerir más decimales (ej. 15.5500 kg o metros)
            builder.Property(c => c.TotalQuantity).HasColumnType("decimal(18,4)");

            // 5. Cascada hacia los detalles del cálculo
            // Al borrar el cálculo oficial, se debe destruir su desglose de líneas
            builder.HasMany(c => c.Details)
                .WithOne(d => d.Calculation)
                .HasForeignKey(d => d.LandedCostCalculationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}