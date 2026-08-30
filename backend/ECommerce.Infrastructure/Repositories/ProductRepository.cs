using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// List view: includes Category, CreatedBy, and Variants (for price range + total stock).
    /// Intentionally does NOT include the full option tree to keep list queries lean.
    /// </summary>
    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, int? categoryId)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.CreatedBy)
            .Include(p => p.Variants)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// Detail view: loads the full nested tree for the product edit form.
    /// The Include chain follows: Options → Attribute, Options → Values → VariantOptionValues
    ///                            Variants → VariantOptionValues → ProductOptionValue
    /// </summary>
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.CreatedBy)
            .Include(p => p.Options)
                .ThenInclude(o => o.Attribute)
            .Include(p => p.Options)
                .ThenInclude(o => o.Values)
            .Include(p => p.Variants)
                .ThenInclude(v => v.VariantOptionValues)
                    .ThenInclude(vov => vov.ProductOptionValue)
                        .ThenInclude(pov => pov.ProductOption)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<ProductVariant?> GetVariantAsync(int productId, int variantId)
    {
        return await _context.ProductVariants
            .Include(v => v.VariantOptionValues)
                .ThenInclude(vov => vov.ProductOptionValue)
                    .ThenInclude(pov => pov.ProductOption)
            .FirstOrDefaultAsync(v => v.Id == variantId && v.ProductId == productId);
    }

    public async Task<ProductVariant> UpdateVariantAsync(ProductVariant variant)
    {
        _context.ProductVariants.Update(variant);
        await _context.SaveChangesAsync();
        return variant;
    }
}
