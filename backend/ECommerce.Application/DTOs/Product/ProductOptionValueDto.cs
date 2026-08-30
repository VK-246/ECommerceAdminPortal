namespace ECommerce.Application.DTOs.Product;

/// <summary>
/// DTO for reading a single product option value (e.g., "Red", "Blue").
/// Returned as part of ProductOptionDto inside ProductDto.
/// </summary>
public class ProductOptionValueDto
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
}
