namespace ECommerce.Domain.Entities;

/// <summary>
/// The actual sellable unit of a product.
/// While a Product is a catalog concept (e.g., "Classic T-Shirt"),
/// a ProductVariant is what actually gets added to a cart and shipped.
///
/// Each variant has a globally unique SKU, its own price, and its own stock quantity.
/// A variant is defined by a unique combination of option values
/// (e.g., Color=Red + Size=Medium → SKU: TSHIRT-RED-M).
///
/// Business Rule: Every product must have at least one variant,
/// even if it has no configurable options (it just has a single "default" variant).
/// </summary>
public class ProductVariant
{
    /// <summary>
    /// Primary key — auto-incrementing integer.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FK to the parent Product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Stock Keeping Unit — globally unique identifier for this sellable unit.
    /// Used across orders, inventory systems, and shipping.
    /// Example: "TSHIRT-RED-M", "IPHONE-256-BLU"
    /// </summary>
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// The selling price for this specific variant.
    /// Stored as decimal (18,2) for currency precision.
    /// Different variants of the same product can have different prices.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Current stock level for this specific variant.
    /// Defaults to 0. Managed independently per variant.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// UTC timestamp of when this variant was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // --- Navigation Properties ---

    /// <summary>
    /// The parent product this variant belongs to.
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// The option value combinations that define this variant.
    /// (e.g., [Color=Red, Size=Medium])
    /// </summary>
    public ICollection<VariantOptionValue> VariantOptionValues { get; set; } = new List<VariantOptionValue>();
}
