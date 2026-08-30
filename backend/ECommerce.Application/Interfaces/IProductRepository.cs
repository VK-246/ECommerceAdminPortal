using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface IProductRepository
{
    /// <summary>
    /// Gets a paged list of products. For the list view, includes
    /// Category, CreatedBy, and Variants (for PriceRange + TotalStock calculations).
    /// Does NOT include the full option/variant-option-value tree to keep list queries lean.
    /// </summary>
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? categoryId);

    /// <summary>
    /// Gets a single product with the full nested tree:
    /// Product → Options → Values and Product → Variants → VariantOptionValues → ProductOptionValue.
    /// Used for the product detail/edit view.
    /// </summary>
    Task<Product?> GetByIdAsync(int id);

    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Product product);

    /// <summary>
    /// Gets a single variant by its Id, verified to belong to the given productId.
    /// </summary>
    Task<ProductVariant?> GetVariantAsync(int productId, int variantId);

    /// <summary>
    /// Saves changes to a variant (price, stock, SKU).
    /// </summary>
    Task<ProductVariant> UpdateVariantAsync(ProductVariant variant);
}
