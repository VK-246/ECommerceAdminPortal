using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Interfaces;

/// <summary>
/// Abstracts the database context so the Application layer can query data
/// without taking a direct dependency on EF Core or the Infrastructure project.
///
/// Why this matters: Application layer must not reference Infrastructure.
/// By depending on this interface, AuthService stays decoupled — if we ever
/// switched from EF Core to Dapper, only the implementation changes.
/// </summary>
public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }

    // Variant-related DbSets
    DbSet<Domain.Entities.Attribute> Attributes { get; }
    DbSet<ProductOption> ProductOptions { get; }
    DbSet<ProductOptionValue> ProductOptionValues { get; }
    DbSet<ProductVariant> ProductVariants { get; }

    // Order-related DbSets
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
