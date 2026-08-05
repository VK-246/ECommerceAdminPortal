using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace ECommerce.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockRepo;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepo = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockRepo.Object);
    }

    /// <summary>
    /// Proves that the Business Logic layer enforces relational integrity.
    /// A category cannot be deleted if it has products assigned to it.
    /// </summary>
    [Fact]
    public async Task DeleteCategory_WithProducts_ThrowsBadRequestException()
    {
        // Arrange
        int categoryId = 1;
        var category = new Category { Id = categoryId, Name = "Electronics" };
        
        _mockRepo.Setup(r => r.GetByIdAsync(categoryId))
                 .ReturnsAsync(category);
                 
        _mockRepo.Setup(r => r.HasProductsAsync(categoryId))
                 .ReturnsAsync(true); // Pretend it has products

        // Act
        Func<Task> act = async () => await _service.DeleteAsync(categoryId);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
                 .WithMessage("Cannot delete a category that has products assigned to it.");
                 
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Never);
    }

    /// <summary>
    /// Proves the happy path for deletion when no products depend on the category.
    /// </summary>
    [Fact]
    public async Task DeleteCategory_WithNoProducts_Succeeds()
    {
        // Arrange
        int categoryId = 1;
        var category = new Category { Id = categoryId, Name = "Electronics" };
        
        _mockRepo.Setup(r => r.GetByIdAsync(categoryId))
                 .ReturnsAsync(category);
                 
        _mockRepo.Setup(r => r.HasProductsAsync(categoryId))
                 .ReturnsAsync(false); // No products

        // Act
        await _service.DeleteAsync(categoryId);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(category), Times.Once);
    }

    /// <summary>
    /// Proves that requesting a non-existent category throws a standard NotFoundException.
    /// </summary>
    [Fact]
    public async Task GetById_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(99))
                 .ReturnsAsync((Category?)null);

        // Act
        Func<Task> act = async () => await _service.GetByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
