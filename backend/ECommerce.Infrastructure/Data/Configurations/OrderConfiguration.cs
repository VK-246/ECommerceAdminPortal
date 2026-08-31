using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the Order entity.
/// Defines constraints, indices, and relationships cleanly away from the domain model.
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderDate)
               .IsRequired();

        builder.Property(o => o.TotalAmount)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Status)
               .IsRequired()
               .HasConversion<string>(); // Store enum as string in the database for readability

        // Relationships
        builder.HasMany(o => o.OrderItems)
               .WithOne(oi => oi.Order)
               .HasForeignKey(oi => oi.OrderId)
               .OnDelete(DeleteBehavior.Cascade); // Deleting an order deletes its items
               
        // Index for performance (Querying by date)
        builder.HasIndex(o => o.OrderDate);
    }
}
