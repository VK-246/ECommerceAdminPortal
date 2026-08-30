using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the ProductOption entity.
/// ProductOption links a global Attribute to a specific Product.
/// </summary>
public class ProductOptionConfiguration : IEntityTypeConfiguration<ProductOption>
{
    public void Configure(EntityTypeBuilder<ProductOption> builder)
    {
        builder.ToTable("ProductOptions");

        builder.HasKey(po => po.Id);

        // Name — denormalized snapshot of the Attribute name, required, max 100 chars
        builder.Property(po => po.Name)
            .IsRequired()
            .HasMaxLength(100);

        // FK to Product — cascade delete.
        // If the product is deleted, all its options are deleted too.
        builder.HasOne(po => po.Product)
            .WithMany(p => p.Options)
            .HasForeignKey(po => po.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK to Attribute — restrict delete.
        // Cannot delete a global Attribute if it is still in use by any product option.
        builder.HasOne(po => po.Attribute)
            .WithMany(a => a.ProductOptions)
            .HasForeignKey(po => po.AttributeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for fast lookup by product and by attribute
        builder.HasIndex(po => po.ProductId);
        builder.HasIndex(po => po.AttributeId);
    }
}
