using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Product;

/// <summary>
/// DTO for updating a single product variant's price, stock, and SKU.
/// Used by: PATCH /api/products/{id}/variants/{variantId}
/// Only the variant-level fields can be changed here.
/// To change options or add/remove variants, a full product update flow is needed.
/// </summary>
public class UpdateVariantDto
{
    [Required]
    [MaxLength(100)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [Range(0, 10000000)]
    public decimal Price { get; set; }

    [Range(0, 1000000)]
    public int StockQuantity { get; set; }
}
