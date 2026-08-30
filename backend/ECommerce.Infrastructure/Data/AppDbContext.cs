using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext — the bridge between C# entities and PostgreSQL tables.
/// Registers all entity DbSets and applies Fluent API configurations from separate config files.
/// Implements IAppDbContext so the Application layer can depend on the interface, not this class.
/// </summary>
public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // --- DbSets (one per table) ---

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    // Variant-related DbSets
    public DbSet<Domain.Entities.Attribute> Attributes => Set<Domain.Entities.Attribute>();
    public DbSet<ProductOption> ProductOptions => Set<ProductOption>();
    public DbSet<ProductOptionValue> ProductOptionValues => Set<ProductOptionValue>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    // VariantOptionValue is auto-discovered via the composite key config — no explicit DbSet needed

    /// <summary>
    /// Applies all IEntityTypeConfiguration classes from this assembly,
    /// then seeds the database with initial data.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically discovers and applies all configurations in this assembly
        // (UserConfiguration, CategoryConfiguration, ProductConfiguration)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Seed initial data (Admin/Editor users + 5 categories)
        DataSeeder.Seed(modelBuilder);
    }
}
