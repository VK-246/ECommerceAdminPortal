using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Product;

public class UpdateProductDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Range(0, 1000000)]
    public decimal Price { get; set; }
    
    [Range(0, 100000)]
    public int StockQuantity { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
    
    public string? Description { get; set; }
}
