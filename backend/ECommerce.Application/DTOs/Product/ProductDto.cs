namespace ECommerce.Application.DTOs.Product;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Description { get; set; }
    public string? AiMetadata { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    
    public Guid CreatedById { get; set; }
    public string CreatedByEmail { get; set; } = string.Empty;
}
