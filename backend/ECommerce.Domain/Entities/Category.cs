namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a product category (e.g., Electronics, Clothing).
/// Categories organize products and support the category filter on the product list page.
/// </summary>
public class Category
{
    /// <summary>
    /// Primary key — auto-incrementing integer for human-readable IDs.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Category name (e.g., "Electronics", "Clothing").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the category.
    /// </summary>
    public string? Description { get; set; }

    // --- Navigation Properties ---

    /// <summary>
    /// Products belonging to this category (one-to-many).
    /// Delete is restricted — cannot delete a category that has products.
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
