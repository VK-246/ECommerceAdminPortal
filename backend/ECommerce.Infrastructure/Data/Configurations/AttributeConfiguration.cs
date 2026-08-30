using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the Attribute entity.
/// Attributes are global, reusable dimension names (e.g., "Color", "Size").
/// Note: The fully-qualified type name is used here to avoid ambiguity
/// with System.Attribute which exists in the global namespace.
/// </summary>
public class AttributeConfiguration : IEntityTypeConfiguration<ECommerce.Domain.Entities.Attribute>
{
    public void Configure(EntityTypeBuilder<ECommerce.Domain.Entities.Attribute> builder)
    {
        builder.ToTable("Attributes");

        builder.HasKey(a => a.Id);

        // Name — required, max 100 chars, globally unique
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Unique index — prevents duplicate attribute names (e.g., two "Color" attributes)
        builder.HasIndex(a => a.Name)
            .IsUnique();

        // Description — optional
        builder.Property(a => a.Description)
            .HasMaxLength(500);
    }
}
