using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the User entity.
/// Defines table mapping, column constraints, and indexes.
/// 
/// Why Fluent API instead of Data Annotations?
/// Data Annotations would require importing framework attributes into the Domain layer,
/// breaking Clean Architecture's rule that Domain has zero external dependencies.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("Users");

        // Primary key — Guid for non-sequential, enumeration-resistant IDs
        builder.HasKey(u => u.Id);

        // Email — required, max 256 chars, unique index for fast login lookups
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        // PasswordHash — required, stored as text (BCrypt hashes are variable length)
        builder.Property(u => u.PasswordHash)
            .IsRequired();

        // Role — stored as string for human-readable DB values ("Admin" / "Editor")
        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);

        // CreatedAt — UTC timestamp, auto-set
        builder.Property(u => u.CreatedAt)
            .IsRequired();
    }
}
