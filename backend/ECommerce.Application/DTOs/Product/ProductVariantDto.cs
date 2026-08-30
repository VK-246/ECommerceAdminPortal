namespace ECommerce.Application.DTOs.Product;

/// <summary>
/// DTO for reading a product variant — the actual sellable unit.
/// Returned inside ProductDto as a list of all variants for the product.
/// </summary>
public class ProductVariantDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The option values that define this variant.
    /// Example: [{ optionName: "Color", value: "Red" }, { optionName: "Size", value: "M" }]
    /// Used by the frontend to label each variant row in the grid.
    /// </summary>
    public List<VariantOptionValueDto> OptionValues { get; set; } = new();
}

/// <summary>
/// Describes one option-value pair that makes up a variant's "identity".
/// Example: optionName: "Color", value: "Red"
/// </summary>
public class VariantOptionValueDto
{
    public string OptionName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
