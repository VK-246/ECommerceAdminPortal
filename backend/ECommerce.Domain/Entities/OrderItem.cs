using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a specific variant sold in an Order.
/// </summary>
public class OrderItem
{
    public Guid Id { get; set; }
    
    public Guid OrderId { get; set; }
    
    public int ProductVariantId { get; set; }
    
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    // --- Navigation Properties ---
    public Order Order { get; set; } = null!;
    public ProductVariant ProductVariant { get; set; } = null!;
}
