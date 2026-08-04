using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the Category entity.
/// Configures column constraints and the one-to-many relationship with Products.
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Table name
        builder.ToTable("Categories");

        // Primary key — auto-incrementing integer for human-readable IDs
        builder.HasKey(c => c.Id);

        // Name — required, max 100 chars
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Description — optional, max 500 chars
        builder.Property(c => c.Description)
            .HasMaxLength(500);

        // One-to-many: Category has many Products
        // DeleteBehavior.Restrict prevents accidental cascade deletion of products
        // when a category is removed. The user must handle products first.
        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
