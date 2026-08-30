namespace ECommerce.Domain.Entities;

/// <summary>
/// Links a global Attribute to a specific Product.
/// For example: the "T-Shirt" product has a "Color" option (ProductOption),
/// which in turn holds values like "Red", "Blue", "Black" (ProductOptionValues).
///
/// The "Name" field is a denormalized snapshot of the Attribute's name.
/// This protects historical data if the global Attribute name ever changes.
/// </summary>
public class ProductOption
{
    /// <summary>
    /// Primary key — auto-incrementing integer.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FK to the Product this option belongs to.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// FK to the global Attribute this option represents (e.g., "Color").
    /// </summary>
    public int AttributeId { get; set; }

    /// <summary>
    /// Snapshot of the Attribute name at the time of creation.
    /// (e.g., "Color", "Size") — stored here for performance and historical accuracy.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    // --- Navigation Properties ---

    /// <summary>
    /// The product this option belongs to.
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// The global attribute this option is based on.
    /// </summary>
    public Attribute Attribute { get; set; } = null!;

    /// <summary>
    /// The concrete values for this option on this product.
    /// (e.g., "Red", "Blue", "Black" for a Color option)
    /// </summary>
    public ICollection<ProductOptionValue> Values { get; set; } = new List<ProductOptionValue>();
}
