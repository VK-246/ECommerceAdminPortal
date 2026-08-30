using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Product;

/// <summary>
/// DTO for creating a new product with its full variant tree in one request.
/// The request body is a nested JSON structure:
///   Product (metadata) → Options → Values → Variants
///
/// The entire creation is transactional — if any step fails,
/// the whole operation rolls back. No partial products are created.
/// </summary>
public class CreateProductDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// The options to create for this product (e.g., Color, Size).
    /// Can be empty for simple products with no configurable options
    /// (in which case a single default variant must be provided in Variants).
    /// </summary>
    public List<CreateProductOptionDto> Options { get; set; } = new();

    /// <summary>
    /// The variants to create. Must contain at least one variant.
    /// For products with no options, provide a single variant with empty OptionValueIndices.
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "At least one variant is required.")]
    public List<CreateVariantDto> Variants { get; set; } = new();
}
