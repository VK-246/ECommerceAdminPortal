using ECommerce.Application.DTOs.Dashboard;
using ECommerce.Application.Interfaces;
using ECommerce.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize] // All dashboard endpoints require login
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("kpis")]
    [ProducesResponseType(typeof(ApiResponse<KpiDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKpis()
    {
        var result = await _dashboardService.GetKpisAsync();
        return Ok(ApiResponse<KpiDto>.Ok(result, "KPIs retrieved successfully."));
    }

    [HttpGet("sales-chart")]
    [ProducesResponseType(typeof(ApiResponse<List<MonthlySalesDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalesChart()
    {
        var result = await _dashboardService.GetMonthlySalesAsync();
        return Ok(ApiResponse<List<MonthlySalesDto>>.Ok(result, "Sales chart data retrieved."));
    }

    [HttpGet("inventory-alerts")]
    [ProducesResponseType(typeof(ApiResponse<InventoryAlertDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInventoryAlerts()
    {
        var result = await _dashboardService.GetInventoryAlertsAsync();
        return Ok(ApiResponse<InventoryAlertDto>.Ok(result, "Inventory alerts retrieved."));
    }
}
