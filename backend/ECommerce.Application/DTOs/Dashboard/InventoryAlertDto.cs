namespace ECommerce.Application.DTOs.Dashboard;

public class InventoryAlertDto
{
    public List<VariantAlertDto> LowStockVariants { get; set; } = new();
    public List<BestSellerDto> BestSellers { get; set; } = new();
}

public class VariantAlertDto
{
    public string Sku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Stock { get; set; }
}

public class BestSellerDto
{
    public string Sku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int TotalSold { get; set; }
}
