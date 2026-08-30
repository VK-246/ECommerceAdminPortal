namespace ECommerce.Application.DTOs.Product;

/// <summary>
/// DTO returned when reading a product (list or detail views).
/// Price and StockQuantity no longer live here — they live on each ProductVariantDto.
/// PriceRange is computed from the variants and exposed as a convenience field
/// for displaying "from ₹499" in product listing cards.
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AiMetadata { get; set; }
    public DateTime CreatedAt { get; set; }

    // --- Category & Audit ---
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
    public string CreatedByEmail { get; set; } = string.Empty;

    // --- Variant Data ---

    /// <summary>
    /// The options configured for this product (e.g., Color, Size).
    /// </summary>
    public List<ProductOptionDto> Options { get; set; } = new();

    /// <summary>
    /// All sellable variants of this product.
    /// </summary>
    public List<ProductVariantDto> Variants { get; set; } = new();

    /// <summary>
    /// Computed price range from all variants.
    /// Used for "From ₹499" display in product listing cards.
    /// Null if the product has no variants yet.
    /// </summary>
    public PriceRangeDto? PriceRange { get; set; }

    /// <summary>
    /// Total stock across all variants — convenience for the product list view.
    /// </summary>
    public int TotalStock => Variants.Sum(v => v.StockQuantity);
}

/// <summary>
/// Computed price range across all variants of a product.
/// </summary>
public class PriceRangeDto
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }
}
