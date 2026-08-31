using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MockQueryable.Moq;

namespace ECommerce.Tests.Application.Services;

public class DashboardServiceTests
{
    private readonly Mock<IAppDbContext> _mockContext;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _mockContext = new Mock<IAppDbContext>();
        _service = new DashboardService(_mockContext.Object);
    }

    [Fact]
    public async Task GetKpisAsync_ShouldReturnCorrectTotalRevenue_AndExcludeCancelledOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = Guid.NewGuid(), Status = OrderStatus.Delivered, TotalAmount = 100m },
            new Order { Id = Guid.NewGuid(), Status = OrderStatus.Pending, TotalAmount = 50m },
            new Order { Id = Guid.NewGuid(), Status = OrderStatus.Cancelled, TotalAmount = 500m } // Should be ignored
        };

        var mockOrders = orders.BuildMockDbSet();
        _mockContext.Setup(c => c.Orders).Returns(mockOrders.Object);

        // Also need to mock ProductVariants for the LowStock calculation
        var variants = new List<ProductVariant>();
        var mockVariants = variants.BuildMockDbSet();
        _mockContext.Setup(c => c.ProductVariants).Returns(mockVariants.Object);

        // Act
        var kpis = await _service.GetKpisAsync();

        // Assert
        Assert.Equal(150m, kpis.TotalRevenue); // 100 + 50
        Assert.Equal(3, kpis.TotalOrders); // Total orders is 3 regardless of status
    }

    [Fact]
    public async Task GetInventoryAlertsAsync_ShouldReturnVariants_WhenStockIsBelowTen()
    {
        // Arrange
        var variants = new List<ProductVariant>
        {
            new ProductVariant { Id = 1, SKU = "LOW-1", StockQuantity = 5, Product = new Product { Name = "Low 1" } },
            new ProductVariant { Id = 2, SKU = "LOW-2", StockQuantity = 9, Product = new Product { Name = "Low 2" } },
            new ProductVariant { Id = 3, SKU = "OK-1", StockQuantity = 10, Product = new Product { Name = "Ok 1" } } // Should be ignored
        };

        var mockVariants = variants.BuildMockDbSet();
        _mockContext.Setup(c => c.ProductVariants).Returns(mockVariants.Object);

        var orderItems = new List<OrderItem>();
        var mockOrderItems = orderItems.BuildMockDbSet();
        _mockContext.Setup(c => c.OrderItems).Returns(mockOrderItems.Object);

        // Act
        var alerts = await _service.GetInventoryAlertsAsync();

        // Assert
        Assert.Equal(2, alerts.LowStockVariants.Count);
        Assert.Contains(alerts.LowStockVariants, v => v.Sku == "LOW-1");
        Assert.DoesNotContain(alerts.LowStockVariants, v => v.Sku == "OK-1");
    }

    [Fact]
    public async Task GetMonthlySalesAsync_ShouldExcludeCancelledOrders()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var orders = new List<Order>
        {
            new Order { Id = Guid.NewGuid(), Status = OrderStatus.Delivered, TotalAmount = 100m, OrderDate = now },
            new Order { Id = Guid.NewGuid(), Status = OrderStatus.Cancelled, TotalAmount = 500m, OrderDate = now }
        };

        var mockOrders = orders.BuildMockDbSet();
        _mockContext.Setup(c => c.Orders).Returns(mockOrders.Object);

        // Act
        var sales = await _service.GetMonthlySalesAsync();

        // Assert
        Assert.Single(sales);
        Assert.Equal(100m, sales[0].Revenue);
    }
}
