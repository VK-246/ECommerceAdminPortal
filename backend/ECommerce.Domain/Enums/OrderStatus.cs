namespace ECommerce.Domain.Enums;

/// <summary>
/// Represents the fulfillment status of an Order.
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Shipped = 1,
    Delivered = 2,
    Cancelled = 3
}
