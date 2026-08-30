using ECommerce.Application.DTOs.Product;

namespace ECommerce.Application.Interfaces;

public interface IProductService
{
    Task<PagedResultDto<ProductDto>> GetPagedAsync(int page, int pageSize, int? categoryId);
    Task<ProductDto> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto dto, Guid createdById);

    /// <summary>
    /// Updates only the product's metadata (name, description, category).
    /// Variant data is managed separately via UpdateVariantAsync.
    /// </summary>
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);

    /// <summary>
    /// Updates a single variant's SKU, price, and stock quantity.
    /// </summary>
    Task<ProductVariantDto> UpdateVariantAsync(int productId, int variantId, UpdateVariantDto dto);

    Task DeleteAsync(int id);
}
