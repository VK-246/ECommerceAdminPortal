using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the ProductOptionValue entity.
/// These are the concrete values for a product option (e.g., "Red", "Large").
/// </summary>
public class ProductOptionValueConfiguration : IEntityTypeConfiguration<ProductOptionValue>
{
    public void Configure(EntityTypeBuilder<ProductOptionValue> builder)
    {
        builder.ToTable("ProductOptionValues");

        builder.HasKey(pov => pov.Id);

        // Value — required, max 100 chars
        builder.Property(pov => pov.Value)
            .IsRequired()
            .HasMaxLength(100);

        // FK to ProductOption — cascade delete.
        // If an option is deleted (e.g., the "Color" option is removed from a product),
        // all its values ("Red", "Blue", etc.) are deleted too.
        builder.HasOne(pov => pov.ProductOption)
            .WithMany(po => po.Values)
            .HasForeignKey(pov => pov.ProductOptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for fast lookup of all values belonging to an option
        builder.HasIndex(pov => pov.ProductOptionId);
    }
}
