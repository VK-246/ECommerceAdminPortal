using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Product;

/// <summary>
/// DTO for creating a single variant within a CreateProductDto.
/// Each variant specifies its own SKU, price, stock, and which option values it represents.
///
/// The optionValueIndices array maps to the values of each option in the same
/// order as the options array in CreateProductDto.
///
/// Example for a T-Shirt with options [Color: Red/Blue, Size: S/M]:
///   { sku: "TSHIRT-RED-S", price: 499, stockQuantity: 10,
///     optionValueIndices: [0, 0] }  → Color[0]=Red, Size[0]=S
///   { sku: "TSHIRT-BLU-M", price: 499, stockQuantity: 5,
///     optionValueIndices: [1, 1] }  → Color[1]=Blue, Size[1]=M
/// </summary>
public class CreateVariantDto
{
    [Required]
    [MaxLength(100)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [Range(0, 10000000)]
    public decimal Price { get; set; }

    [Range(0, 1000000)]
    public int StockQuantity { get; set; }

    /// <summary>
    /// Index into each option's Values array that this variant represents.
    /// Length must equal the number of options in CreateProductDto.Options.
    /// </summary>
    public List<int> OptionValueIndices { get; set; } = new();
}
