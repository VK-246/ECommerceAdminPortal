using ECommerce.Application.DTOs.Dashboard;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IAppDbContext _context;

    public DashboardService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<KpiDto> GetKpisAsync()
    {
        // Total revenue excludes cancelled orders.
        // Calculated natively in PostgreSQL for maximum performance.
        var totalRevenue = await _context.Orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount);

        // All orders count
        var totalOrders = await _context.Orders.CountAsync();

        // Count of variants with stock < 10
        var lowStockAlerts = await _context.ProductVariants
            .CountAsync(v => v.StockQuantity < 10);

        return new KpiDto
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            LowStockAlerts = lowStockAlerts
        };
    }

    public async Task<List<MonthlySalesDto>> GetMonthlySalesAsync()
    {
        // 1. Get date exactly 12 months ago
        var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);

        // 2. We group by Year and Month in the database to avoid downloading raw orders
        var salesData = await _context.Orders
            .Where(o => o.OrderDate >= twelveMonthsAgo && o.Status != OrderStatus.Cancelled)
            .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Revenue = g.Sum(o => o.TotalAmount)
            })
            // Sort chronologically
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        // 3. Map to DTO, formatting month number to abbreviated string (e.g., "Jan")
        return salesData.Select(s => new MonthlySalesDto
        {
            Year = s.Year,
            // Convert month integer to string (1 -> Jan, 2 -> Feb)
            Month = new DateTime(s.Year, s.Month, 1).ToString("MMM"),
            Revenue = s.Revenue
        }).ToList();
    }

    public async Task<InventoryAlertDto> GetInventoryAlertsAsync()
    {
        // Top 5 variants with lowest stock under 10
        var lowStock = await _context.ProductVariants
            .Include(v => v.Product)
            .Where(v => v.StockQuantity < 10)
            .OrderBy(v => v.StockQuantity)
            .Take(5)
            .Select(v => new VariantAlertDto
            {
                Sku = v.SKU,
                ProductName = v.Product.Name,
                Stock = v.StockQuantity
            })
            .ToListAsync();

        // Best selling variants
        var bestSellers = await _context.OrderItems
            .Include(oi => oi.ProductVariant)
            .ThenInclude(v => v.Product)
            // Only count items from successful orders
            .Where(oi => oi.Order.Status != OrderStatus.Cancelled)
            .GroupBy(oi => new { oi.ProductVariant.SKU, oi.ProductVariant.Product.Name })
            .Select(g => new BestSellerDto
            {
                Sku = g.Key.SKU,
                ProductName = g.Key.Name,
                TotalSold = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(5)
            .ToListAsync();

        return new InventoryAlertDto
        {
            LowStockVariants = lowStock,
            BestSellers = bestSellers
        };
    }
}
