namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a global, reusable product attribute (e.g., "Color", "Size", "Storage").
/// Attributes are shared across all products in the catalog.
/// A specific product "uses" an attribute by creating a ProductOption that links to it.
/// </summary>
public class Attribute
{
    /// <summary>
    /// Primary key — auto-incrementing integer.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Attribute name (e.g., "Color", "Size", "Storage").
    /// Must be globally unique — enforced by a unique index in the DB.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of what this attribute represents.
    /// (e.g., "Visual color of the product", "Physical storage capacity in GB")
    /// </summary>
    public string? Description { get; set; }

    // --- Navigation Properties ---

    /// <summary>
    /// All ProductOptions that reference this attribute.
    /// One Attribute can be used across many different products.
    /// </summary>
    public ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
}
