using System.ComponentModel.DataAnnotations.Schema;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a customer's order.
/// </summary>
public class Order
{
    public Guid Id { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    public OrderStatus Status { get; set; }

    // --- Navigation Properties ---
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
