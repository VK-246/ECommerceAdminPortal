using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Product;

/// <summary>
/// DTO for creating a new product option within a CreateProductDto.
/// Specifies which global attribute to use and the values to create for this product.
/// Example: { attributeId: 1, values: ["Red", "Blue", "Black"] }
/// </summary>
public class CreateProductOptionDto
{
    /// <summary>
    /// The Id of the global Attribute to use (e.g., 1 = Color, 2 = Size).
    /// Must match an existing Attribute in the Attributes table.
    /// </summary>
    [Required]
    public int AttributeId { get; set; }

    /// <summary>
    /// The list of values to create for this option on this product.
    /// (e.g., ["Red", "Blue", "Black"] for a Color option)
    /// Must contain at least one value.
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<string> Values { get; set; } = new();
}
