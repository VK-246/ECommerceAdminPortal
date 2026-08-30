using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the ProductVariant entity.
/// ProductVariant is the actual sellable unit — it holds SKU, Price, and StockQuantity.
/// </summary>
public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(pv => pv.Id);

        // SKU — required, max 100 chars, globally unique across all products
        // This is the key identifier used in orders, shipping, and inventory systems.
        builder.Property(pv => pv.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(pv => pv.SKU)
            .IsUnique();

        // Price — decimal (18,2) — standard for currency precision
        builder.Property(pv => pv.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        // StockQuantity — defaults to 0
        builder.Property(pv => pv.StockQuantity)
            .IsRequired()
            .HasDefaultValue(0);

        // CreatedAt — UTC timestamp
        builder.Property(pv => pv.CreatedAt)
            .IsRequired();

        // FK to Product — cascade delete.
        // Deleting a product removes all its variants.
        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for fast lookup of all variants belonging to a product
        builder.HasIndex(pv => pv.ProductId);
    }
}
