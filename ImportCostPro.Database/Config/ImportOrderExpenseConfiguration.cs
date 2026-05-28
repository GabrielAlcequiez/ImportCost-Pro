using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations
{
    public class ImportOrderExpenseConfiguration : IEntityTypeConfiguration<ImportOrderExpense>
    {
        public void Configure(EntityTypeBuilder<ImportOrderExpense> builder)
        {
            builder.ToTable("ImportOrderExpenses");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.ExchangeRateValue)
                .IsRequired()
                .HasColumnType("decimal(18,4)"); // Precisión para conversión de divisas

            builder.Property(e => e.ExpenseDate)
                .IsRequired();

            // Mapeo de Enums a enteros en DB
            builder.Property(e => e.ExpenseType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.ApportionmentMethod)
                .IsRequired()
                .HasConversion<int>();

            // Relación Cascade: Si se borra la orden cabecera, se borran sus gastos asociados
            builder.HasOne(e => e.ImportOrder)
                .WithMany(o => o.Expenses)
                .HasForeignKey(e => e.ImportOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Restrict: No se puede borrar una moneda si hay gastos registrados en ella
            builder.HasOne(e => e.Currency)
                .WithMany()
                .HasForeignKey(e => e.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
