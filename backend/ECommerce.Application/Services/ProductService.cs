using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Shared.Exceptions;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResultDto<ProductDto>> GetPagedAsync(int page, int pageSize, int? categoryId)
    {
        var (items, totalCount) = await _productRepository.GetPagedAsync(page, pageSize, categoryId);

        var productDtos = items.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Description = p.Description,
            AiMetadata = p.AiMetadata,
            CreatedAt = p.CreatedAt,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? "Unknown",
            CreatedById = p.CreatedById,
            CreatedByEmail = p.CreatedBy?.Email ?? "Unknown"
        }).ToList();

        return new PagedResultDto<ProductDto>
        {
            Items = productDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found.");

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Description = product.Description,
            AiMetadata = product.AiMetadata,
            CreatedAt = product.CreatedAt,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? "Unknown",
            CreatedById = product.CreatedById,
            CreatedByEmail = product.CreatedBy?.Email ?? "Unknown"
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, Guid createdById)
    {
        var categoryExists = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (categoryExists == null)
        {
            throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");
        }

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            CreatedById = createdById,
            CreatedAt = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.CreateAsync(product);

        // Fetch it again to get the included Navigation properties populated for the DTO
        var fetchedProduct = await _productRepository.GetByIdAsync(createdProduct.Id);

        return new ProductDto
        {
            Id = fetchedProduct!.Id,
            Name = fetchedProduct.Name,
            Price = fetchedProduct.Price,
            StockQuantity = fetchedProduct.StockQuantity,
            Description = fetchedProduct.Description,
            CreatedAt = fetchedProduct.CreatedAt,
            CategoryId = fetchedProduct.CategoryId,
            CategoryName = fetchedProduct.Category?.Name ?? "Unknown",
            CreatedById = fetchedProduct.CreatedById,
            CreatedByEmail = fetchedProduct.CreatedBy?.Email ?? "Unknown"
        };
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found.");

        if (product.CategoryId != dto.CategoryId)
        {
            var categoryExists = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (categoryExists == null)
            {
                throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");
            }
            product.CategoryId = dto.CategoryId;
        }

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.Description = dto.Description;

        var updatedProduct = await _productRepository.UpdateAsync(product);

        return new ProductDto
        {
            Id = updatedProduct.Id,
            Name = updatedProduct.Name,
            Price = updatedProduct.Price,
            StockQuantity = updatedProduct.StockQuantity,
            Description = updatedProduct.Description,
            AiMetadata = updatedProduct.AiMetadata,
            CreatedAt = updatedProduct.CreatedAt,
            CategoryId = updatedProduct.CategoryId,
            CategoryName = updatedProduct.Category?.Name ?? "Unknown",
            CreatedById = updatedProduct.CreatedById,
            CreatedByEmail = updatedProduct.CreatedBy?.Email ?? "Unknown"
        };
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            throw new NotFoundException($"Product with ID {id} not found.");

        await _productRepository.DeleteAsync(product);
    }
}
