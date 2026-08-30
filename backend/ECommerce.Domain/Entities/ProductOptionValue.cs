namespace ECommerce.Domain.Entities;

/// <summary>
/// A concrete value for a specific ProductOption.
/// Example: For the "Color" option on a T-Shirt, the values are "Red", "Blue", "Black".
///
/// These values are the building blocks that define what variants exist.
/// Each variant is a unique combination of one ProductOptionValue per option.
/// </summary>
public class ProductOptionValue
{
    /// <summary>
    /// Primary key — auto-incrementing integer.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FK to the ProductOption this value belongs to.
    /// </summary>
    public int ProductOptionId { get; set; }

    /// <summary>
    /// The actual value text (e.g., "Red", "Large", "256GB").
    /// Max 100 characters.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    // --- Navigation Properties ---

    /// <summary>
    /// The option this value belongs to.
    /// </summary>
    public ProductOption ProductOption { get; set; } = null!;

    /// <summary>
    /// The join table entries linking this value to specific variants.
    /// A value can be part of multiple variants (e.g., "Red" appears in Red-S, Red-M, Red-L).
    /// </summary>
    public ICollection<VariantOptionValue> VariantOptionValues { get; set; } = new List<VariantOptionValue>();
}
