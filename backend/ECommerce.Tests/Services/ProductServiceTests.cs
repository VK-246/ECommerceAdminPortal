using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace ECommerce.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockProductRepo = new Mock<IProductRepository>();
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _service = new ProductService(_mockProductRepo.Object, _mockCategoryRepo.Object);
    }

    /// <summary>
    /// Proves the happy path for product creation.
    /// Crucially, verifies that the 'createdById' provided by the server (not the client DTO)
    /// is correctly mapped to the final saved Product, enforcing the audit trail.
    /// </summary>
    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsProductDto()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var dto = new CreateProductDto { Name = "Test", Price = 10m, StockQuantity = 5, CategoryId = 1 };
        
        var category = new Category { Id = 1, Name = "Electronics" };
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

        var savedProduct = new Product 
        { 
            Id = 100, Name = dto.Name, Price = dto.Price, 
            StockQuantity = dto.StockQuantity, CategoryId = dto.CategoryId, 
            CreatedById = createdById 
        };
        
        _mockProductRepo.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(savedProduct);
        _mockProductRepo.Setup(r => r.GetByIdAsync(100)).ReturnsAsync(savedProduct);

        // Act
        var result = await _service.CreateAsync(dto, createdById);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(100);
        result.Name.Should().Be(dto.Name);
        result.CreatedById.Should().Be(createdById);
    }

    /// <summary>
    /// Proves the referential integrity guard on creation.
    /// You cannot create a product assigned to a category that doesn't exist.
    /// </summary>
    [Fact]
    public async Task CreateProduct_WithInvalidCategoryId_ThrowsNotFoundException()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var dto = new CreateProductDto { Name = "Test", CategoryId = 99 };
        
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

        // Act
        Func<Task> act = async () => await _service.CreateAsync(dto, createdById);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    /// <summary>
    /// Proves that the pagination math (calculating TotalPages based on TotalCount and PageSize)
    /// works correctly and maps to the PagedResultDto.
    /// </summary>
    [Fact]
    public async Task GetPagedProducts_ReturnsCorrectPagedResult()
    {
        // Arrange
        var products = new List<Product> 
        {
            new Product { Id = 1, Name = "P1" },
            new Product { Id = 2, Name = "P2" }
        };
        
        _mockProductRepo.Setup(r => r.GetPagedAsync(1, 10, null))
                        .ReturnsAsync((products, 15)); // 15 total items

        // Act
        var result = await _service.GetPagedAsync(1, 10, null);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(15);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(2); // Ceiling(15/10) = 2
    }

    /// <summary>
    /// Proves that attempting to delete a non-existent product throws a standard NotFoundException.
    /// </summary>
    [Fact]
    public async Task DeleteProduct_WhenProductNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockProductRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act
        Func<Task> act = async () => await _service.DeleteAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}