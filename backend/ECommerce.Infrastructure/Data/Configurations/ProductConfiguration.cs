using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the Product entity.
/// Configures JSONB column, decimal precision, foreign keys, and indexes.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table name
        builder.ToTable("Products");

        // Primary key — auto-incrementing integer
        builder.HasKey(p => p.Id);

        // Name — required, max 200 chars
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Description — optional, stored as text (no length limit for AI-generated content)
        builder.Property(p => p.Description);

        // AiMetadata — stored as PostgreSQL JSONB for flexible, schema-free AI output storage.
        // JSONB supports efficient querying and indexing within JSON structures.
        builder.Property(p => p.AiMetadata)
            .HasColumnType("jsonb");

        // CreatedAt — UTC timestamp
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        // --- Foreign Keys ---

        // FK to Category — index for fast "filter by category" queries
        builder.HasIndex(p => p.CategoryId);

        // FK to User (audit trail) — index for fast "who created this?" queries
        // DeleteBehavior.Restrict prevents deleting a user who has created products
        builder.HasOne(p => p.CreatedBy)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.CreatedById);
    }
}
