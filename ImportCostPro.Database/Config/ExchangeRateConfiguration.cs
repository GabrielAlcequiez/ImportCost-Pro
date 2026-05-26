using ImportCostPro.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Configurations;

public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.ToTable("ExchangeRates");
        builder.HasKey(e => e.Id);

        // Technical Spec: Use decimal for financial values, avoid float/double
        // decimal(18,4) offers ideal precision for international currency multipliers
        builder.Property(e => e.RateValue)
            .IsRequired()
            .HasColumnType("decimal(18,4)");

        builder.Property(e => e.EffectiveDate)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired();

        // The rate value must be greater than 0
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_ExchangeRate_Value_Positive", 
            "RateValue > 0"
        ));

        // Setting up foreign key relationships pointing to the Currencies table
        // We use Restrict to block physical deletion of a currency if rates depend on it
        builder.HasOne(e => e.SourceCurrency)
            .WithMany()
            .HasForeignKey(e => e.SourceCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TargetCurrency)
            .WithMany()
            .HasForeignKey(e => e.TargetCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}