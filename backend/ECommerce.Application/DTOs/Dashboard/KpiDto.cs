namespace ECommerce.Application.DTOs.Dashboard;

public class KpiDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int LowStockAlerts { get; set; }
}
