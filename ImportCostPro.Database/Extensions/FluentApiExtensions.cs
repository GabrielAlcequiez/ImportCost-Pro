using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Database.Extensions;

public static class FluentApiExtensions
{
    public static void HasExactIsoCodeRule<TEntity>(this EntityTypeBuilder<TEntity> builder, string propertyName, int length)
        where TEntity : class
    {
        builder.HasStringLengthRule(propertyName, length, length);
    }

    public static void HasStringLengthRule<TEntity>(this EntityTypeBuilder<TEntity> builder, string propertyName, int minLength, int maxLength)
        where TEntity : class
    {
        builder.Property(propertyName)
            .IsRequired()
            .HasMaxLength(maxLength);

        string tableName = builder.Metadata.GetTableName() ?? typeof(TEntity).Name;
        builder.ToTable(table => table.HasCheckConstraint(
            $"CK_{tableName}_{propertyName}_Length",
            $"LEN([{propertyName}]) >= {minLength} AND LEN([{propertyName}]) <= {maxLength}"
        ));
    }
}
