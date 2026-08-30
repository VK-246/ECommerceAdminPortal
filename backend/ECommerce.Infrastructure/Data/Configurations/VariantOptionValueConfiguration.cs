using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the VariantOptionValue join table.
/// This is the Many-to-Many bridge between ProductVariant and ProductOptionValue.
///
/// Key design: Composite Primary Key (VariantId, OptionValueId).
/// This naturally prevents the same option value from being applied to
/// the same variant more than once.
/// </summary>
public class VariantOptionValueConfiguration : IEntityTypeConfiguration<VariantOptionValue>
{
    public void Configure(EntityTypeBuilder<VariantOptionValue> builder)
    {
        builder.ToTable("VariantOptionValues");

        // Composite Primary Key — the combination of both FKs is the PK.
        // This is EF Core's way of expressing: HasKey({ VariantId, OptionValueId })
        builder.HasKey(vov => new { vov.VariantId, vov.OptionValueId });

        // FK to ProductVariant — cascade delete.
        // If a variant is deleted, its option value mappings are cleaned up.
        builder.HasOne(vov => vov.ProductVariant)
            .WithMany(pv => pv.VariantOptionValues)
            .HasForeignKey(vov => vov.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK to ProductOptionValue — restrict delete.
        // Cannot delete an option value if a variant is still using it.
        // The variant must be deleted or updated first.
        builder.HasOne(vov => vov.ProductOptionValue)
            .WithMany(pov => pov.VariantOptionValues)
            .HasForeignKey(vov => vov.OptionValueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
