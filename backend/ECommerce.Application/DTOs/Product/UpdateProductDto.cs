using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Product;

/// <summary>
/// DTO for updating a product's metadata (name, description, category).
/// Note: Variants are NOT updated here — use PATCH /products/{id}/variants/{variantId}.
/// This separation keeps updates focused and avoids re-saving the entire
/// variant tree when only the product name changes.
/// </summary>
public class UpdateProductDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    public string? Description { get; set; }
}
