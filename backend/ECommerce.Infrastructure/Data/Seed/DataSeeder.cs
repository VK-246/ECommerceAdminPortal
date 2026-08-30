using ECommerce.Domain.Entities;
using ECommerce.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data.Seed;

/// <summary>
/// Seeds the database with initial data using EF Core's HasData().
/// Seed data becomes part of the migration itself, ensuring every developer
/// gets the same initial data — more reliable than a separate "run this script" step.
/// 
/// Note: BCrypt hashes below are pre-computed for deterministic migrations.
/// HasData() requires constant values that don't change between runs.
/// </summary>
public static class DataSeeder
{
    // Fixed Guids for seed users — required by HasData() for deterministic migrations.
    // These never change once the migration is created.
    private static readonly Guid AdminUserId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    private static readonly Guid EditorUserId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");

    // Pre-computed BCrypt hashes for seed passwords.
    // Admin@123 and Editor@123 — generated once and hardcoded for deterministic HasData().
    // In production, passwords are hashed at runtime via BCrypt.Net.BCrypt.HashPassword().
    private const string AdminPasswordHash = "$2a$11$WQZF3vQxlPHKjUCeBzq9iu.rNEkBaTMfGHHSipbRoGMiHfApMyPCa";
    private const string EditorPasswordHash = "$2a$11$WQZF3vQxlPHKjUCeBzq9iuJlCNq5HaEsCNXbPz7L9ppbXNqBcWnHK";

    /// <summary>
    /// Applies all seed data to the model builder.
    /// Called from AppDbContext.OnModelCreating().
    /// </summary>
    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedUsers(modelBuilder);
        SeedCategories(modelBuilder);
        SeedAttributes(modelBuilder);
    }

    /// <summary>
    /// Seeds two default users: an Admin and an Editor.
    /// Passwords are pre-hashed with BCrypt — never store plaintext passwords.
    /// </summary>
    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = AdminUserId,
                Email = "admin@ecommerce.com",
                PasswordHash = AdminPasswordHash,
                Role = AppConstants.RoleAdmin,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = EditorUserId,
                Email = "editor@ecommerce.com",
                PasswordHash = EditorPasswordHash,
                Role = AppConstants.RoleEditor,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }

    /// <summary>
    /// Seeds 5 sample product categories for immediate use after migration.
    /// </summary>
    private static void SeedCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices, gadgets, and accessories" },
            new Category { Id = 2, Name = "Clothing", Description = "Apparel, fashion, and wearable items" },
            new Category { Id = 3, Name = "Home & Kitchen", Description = "Household items, kitchen appliances, and decor" },
            new Category { Id = 4, Name = "Books", Description = "Physical and digital books across all genres" },
            new Category { Id = 5, Name = "Sports & Outdoors", Description = "Sports equipment, fitness gear, and outdoor supplies" }
        );
    }

    /// <summary>
    /// Seeds 5 common global attributes used to define product options.
    /// These are the building blocks for product variant configuration.
    /// Example: "Color" attribute → used by T-Shirt, Sneakers, Phone Case products.
    /// </summary>
    private static void SeedAttributes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Attribute>().HasData(
            new Domain.Entities.Attribute { Id = 1, Name = "Color",    Description = "Visual color of the product" },
            new Domain.Entities.Attribute { Id = 2, Name = "Size",     Description = "Physical size (e.g., S, M, L, XL or numeric)" },
            new Domain.Entities.Attribute { Id = 3, Name = "Storage",  Description = "Storage capacity (e.g., 128GB, 256GB, 512GB)" },
            new Domain.Entities.Attribute { Id = 4, Name = "Material", Description = "Primary material composition" },
            new Domain.Entities.Attribute { Id = 5, Name = "Weight",   Description = "Product weight (e.g., Lightweight, Standard, Heavy)" }
        );
    }
}
