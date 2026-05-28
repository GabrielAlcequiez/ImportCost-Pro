using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations
{
    public class LandedCostCalculationDetailConfiguration : IEntityTypeConfiguration<LandedCostCalculationDetail>
    {
        public void Configure(EntityTypeBuilder<LandedCostCalculationDetail> builder)
        {
            builder.ToTable("LandedCostCalculationDetails");
            builder.HasKey(d => d.Id);

            
            // 1. Cantidad de entrada (Puede requerir decimales según la unidad de medida)
            builder.Property(d => d.Quantity)
                .IsRequired()
                .HasColumnType("decimal(18,4)");

            // 2. Porcentajes congelados [cite: 1858-1859, 1924]
            builder.Property(d => d.TariffPercentageUsed).HasColumnType("decimal(5,2)");
            builder.Property(d => d.ExciseTaxPercentageUsed).HasColumnType("decimal(5,2)");
            builder.Property(d => d.DesiredProfitMargin).HasColumnType("decimal(5,2)");

            // 3. Totales y resultados financieros por producto (A 2 decimales para interfaz) [cite: 1913-1914]
            builder.Property(d => d.FobOriginal).HasColumnType("decimal(18,2)");
            builder.Property(d => d.FobLocal).HasColumnType("decimal(18,2)");
            builder.Property(d => d.FreightAssigned).HasColumnType("decimal(18,2)");
            builder.Property(d => d.InsuranceAssigned).HasColumnType("decimal(18,2)");
            builder.Property(d => d.CifLocal).HasColumnType("decimal(18,2)");
            builder.Property(d => d.DutyCalculated).HasColumnType("decimal(18,2)");
            builder.Property(d => d.SelectiveTaxCalculated).HasColumnType("decimal(18,2)");
            builder.Property(d => d.CustomsServiceFeeCalculated).HasColumnType("decimal(18,2)");
            builder.Property(d => d.ItbisCalculated).HasColumnType("decimal(18,2)");
            builder.Property(d => d.LocalExpensesAssigned).HasColumnType("decimal(18,2)");
            builder.Property(d => d.ImportTotalCost).HasColumnType("decimal(18,2)");
            builder.Property(d => d.UnitLandedCost).HasColumnType("decimal(18,2)");
            builder.Property(d => d.SuggestedSellingPrice).HasColumnType("decimal(18,2)");

            // --- RELACIONES E INTEGRIDAD REFERENCIAL ---

            // Relación con la Cabecera del Cálculo
            // Al borrar el cálculo oficial, sus detalles se destruyen en cascada
            builder.HasOne(d => d.Calculation)
                .WithMany(c => c.Details)
                .HasForeignKey(d => d.LandedCostCalculationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con el Producto Maestro
            // Justificación: No se debe permitir eliminar un producto que esté asociado a un cálculo histórico
            builder.HasOne(d => d.Product)
                .WithMany() // Relación unidireccional
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}