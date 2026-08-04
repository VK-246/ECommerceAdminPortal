namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents a product in the e-commerce catalog.
/// Products belong to a Category and are linked to the User who created them (audit trail).
/// The AiMetadata field stores AI-generated content as PostgreSQL JSONB.
/// </summary>
public class Product
{
    /// <summary>
    /// Primary key — auto-incrementing integer.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product name (e.g., "Wireless Headphones").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product price — stored as decimal with precision (18,2) for currency accuracy.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Available stock quantity. Defaults to 0.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Product description — can be AI-generated or manually entered.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Raw AI output stored as PostgreSQL JSONB for flexible, schema-free storage.
    /// Contains data like seoScore, keywords, alternativeTitles, model used, etc.
    /// </summary>
    public string? AiMetadata { get; set; }

    /// <summary>
    /// UTC timestamp of when the product was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // --- Foreign Keys ---

    /// <summary>
    /// FK to the Category this product belongs to.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// FK to the User who created this product (audit trail).
    /// Extracted from JWT claims on the server — never accepted from the client.
    /// </summary>
    public Guid CreatedById { get; set; }

    // --- Navigation Properties ---

    /// <summary>
    /// The category this product belongs to.
    /// </summary>
    public Category Category { get; set; } = null!;

    /// <summary>
    /// The user who created this product (audit trail).
    /// </summary>
    public User CreatedBy { get; set; } = null!;
}
