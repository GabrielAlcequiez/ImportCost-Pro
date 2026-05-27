using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations
{
    public class ImportOrderDetailConfiguration : IEntityTypeConfiguration<ImportOrderDetail>
    {
        public void Configure(EntityTypeBuilder<ImportOrderDetail> builder)
        {
            builder.ToTable("ImportOrderDetails");
            builder.HasKey(d => d.Id);

            // Regla de Negocio: No se puede agregar el mismo producto dos veces a una misma orden 
            builder.HasIndex(d => new { d.ImportOrderId, d.ProductId })
                .IsUnique();

            // Mapeo de precisión para datos de entrada
            builder.Property(d => d.Quantity)
                .IsRequired()
                .HasColumnType("decimal(18,4)"); // Soporta cantidades decimales (metros, galones, etc.)

            builder.Property(d => d.UnitCostFob)
                .IsRequired()
                .HasColumnType("decimal(18,4)"); // Costo unitario FOB con precisión de 4 decimales

            builder.Property(d => d.UnitWeight)
                .IsRequired()
                .HasColumnType("decimal(18,4)"); // Peso unitario en kg

            builder.Property(d => d.CustomDutyPercentage)
                .IsRequired()
                .HasColumnType("decimal(5,2)"); // Porcentaje de arancel (Ej. 20.00%)

            builder.Property(d => d.DesiredProfitMargin)
                .IsRequired()
                .HasColumnType("decimal(5,2)"); // Margen de ganancia (Ej. 35.00%)

            // Mapeo de precisión para resultados financieros (Nullables)
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

            // Relación Cascade: Si se borra la orden cabecera, se borran sus líneas de detalle
            builder.HasOne(d => d.ImportOrder)
                .WithMany(o => o.Details)
                .HasForeignKey(d => d.ImportOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Restrict: No se puede borrar un producto si está siendo referenciado en un detalle de orden
            builder.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
