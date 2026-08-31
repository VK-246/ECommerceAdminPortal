using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the OrderItem entity.
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity)
               .IsRequired();

        builder.Property(oi => oi.UnitPrice)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        // Relationships
        builder.HasOne(oi => oi.ProductVariant)
               .WithMany() // Assuming ProductVariant doesn't have an OrderItems collection to avoid circular loading
               .HasForeignKey(oi => oi.ProductVariantId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a variant if it has been ordered
    }
}
