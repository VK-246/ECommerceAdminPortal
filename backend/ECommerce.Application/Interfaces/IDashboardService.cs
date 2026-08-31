using ECommerce.Application.DTOs.Dashboard;

namespace ECommerce.Application.Interfaces;

public interface IDashboardService
{
    Task<KpiDto> GetKpisAsync();
    Task<List<MonthlySalesDto>> GetMonthlySalesAsync();
    Task<InventoryAlertDto> GetInventoryAlertsAsync();
}
