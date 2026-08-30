namespace ECommerce.Application.DTOs.Product;

/// <summary>
/// DTO for reading a product option and its configured values.
/// (e.g., Option: "Color" → Values: ["Red", "Blue", "Black"])
/// Returned as part of ProductDto.
/// </summary>
public class ProductOptionDto
{
    public int Id { get; set; }

    /// <summary>
    /// The attribute name snapshot (e.g., "Color", "Size").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The Id of the global Attribute this option was based on.
    /// Useful for the frontend to know which attribute dropdown was selected.
    /// </summary>
    public int AttributeId { get; set; }

    /// <summary>
    /// The concrete values configured for this option on this product.
    /// </summary>
    public List<ProductOptionValueDto> Values { get; set; } = new();
}
