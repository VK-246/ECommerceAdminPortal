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

    // ──────────────────────────────────────────────────────────────
    // READ
    // ──────────────────────────────────────────────────────────────

    public async Task<PagedResultDto<ProductDto>> GetPagedAsync(int page, int pageSize, int? categoryId)
    {
        var (items, totalCount) = await _productRepository.GetPagedAsync(page, pageSize, categoryId);

        var productDtos = items.Select(MapToListDto).ToList();

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
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product with ID {id} not found.");

        return MapToDetailDto(product);
    }

    // ──────────────────────────────────────────────────────────────
    // CREATE — Transactional: builds the full entity tree in one SaveChanges
    // ──────────────────────────────────────────────────────────────

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, Guid createdById)
    {
        // 1. Validate category exists
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId)
            ?? throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");

        // 2. Build Options with their Values — collecting the created OptionValues so we
        //    can resolve indices later when building Variants.
        var optionEntities = new List<ProductOption>();
        var createdOptionValues = new List<List<ProductOptionValue>>(); // [option index][value index]

        foreach (var optionDto in dto.Options)
        {
            var optionValues = optionDto.Values.Select(v => new ProductOptionValue
            {
                Value = v
            }).ToList();

            optionEntities.Add(new ProductOption
            {
                AttributeId = optionDto.AttributeId,
                Name = optionDto.AttributeId.ToString(), // Placeholder — resolved below from the DB attribute name
                Values = optionValues
            });

            createdOptionValues.Add(optionValues);
        }

        // 3. Build Variants — resolve OptionValueIndices into actual VariantOptionValue join rows
        var variantEntities = new List<ProductVariant>();
        foreach (var variantDto in dto.Variants)
        {
            var variant = new ProductVariant
            {
                SKU = variantDto.SKU,
                Price = variantDto.Price,
                StockQuantity = variantDto.StockQuantity,
                CreatedAt = DateTime.UtcNow,
                VariantOptionValues = new List<VariantOptionValue>()
            };

            // Map each index in OptionValueIndices to the corresponding ProductOptionValue entity
            for (int i = 0; i < variantDto.OptionValueIndices.Count; i++)
            {
                if (i >= createdOptionValues.Count)
                    continue; // Defensive: skip if more indices than options

                var valueIndex = variantDto.OptionValueIndices[i];
                if (valueIndex < 0 || valueIndex >= createdOptionValues[i].Count)
                    throw new InvalidOperationException(
                        $"Variant '{variantDto.SKU}': OptionValueIndex[{i}]={valueIndex} is out of range.");

                var targetOptionValue = createdOptionValues[i][valueIndex];
                variant.VariantOptionValues.Add(new VariantOptionValue
                {
                    ProductOptionValue = targetOptionValue // EF will resolve the FK after insert
                });
            }

            variantEntities.Add(variant);
        }

        // 4. Build the root Product entity — EF Core will cascade-insert the whole tree
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            CreatedById = createdById,
            CreatedAt = DateTime.UtcNow,
            Options = optionEntities,
            Variants = variantEntities
        };

        // 5. Single SaveChanges — entire tree is inserted atomically
        var createdProduct = await _productRepository.CreateAsync(product);

        // 6. Reload with full Include chain so the response DTO is fully populated
        var fetchedProduct = await _productRepository.GetByIdAsync(createdProduct.Id)
            ?? throw new InvalidOperationException("Failed to reload product after creation.");

        return MapToDetailDto(fetchedProduct);
    }

    // ──────────────────────────────────────────────────────────────
    // UPDATE — Product metadata only (name, description, category)
    // ──────────────────────────────────────────────────────────────

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product with ID {id} not found.");

        if (product.CategoryId != dto.CategoryId)
        {
            _ = await _categoryRepository.GetByIdAsync(dto.CategoryId)
                ?? throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;

        await _productRepository.UpdateAsync(product);

        // Reload to return fully populated DTO
        var updated = await _productRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Failed to reload product after update.");

        return MapToDetailDto(updated);
    }

    // ──────────────────────────────────────────────────────────────
    // UPDATE VARIANT — Price, Stock, SKU for a single variant
    // ──────────────────────────────────────────────────────────────

    public async Task<ProductVariantDto> UpdateVariantAsync(int productId, int variantId, UpdateVariantDto dto)
    {
        var variant = await _productRepository.GetVariantAsync(productId, variantId)
            ?? throw new NotFoundException($"Variant with ID {variantId} not found on product {productId}.");

        variant.SKU = dto.SKU;
        variant.Price = dto.Price;
        variant.StockQuantity = dto.StockQuantity;

        var updated = await _productRepository.UpdateVariantAsync(variant);
        return MapVariantToDto(updated);
    }

    // ──────────────────────────────────────────────────────────────
    // DELETE
    // ──────────────────────────────────────────────────────────────

    public async Task DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product with ID {id} not found.");

        await _productRepository.DeleteAsync(product);
    }

    // ──────────────────────────────────────────────────────────────
    // PRIVATE MAPPING HELPERS
    // ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Maps a product to a list DTO — lean, only includes variants for price range calculation.
    /// Options tree is NOT included for performance (list view doesn't need it).
    /// </summary>
    private static ProductDto MapToListDto(Product p)
    {
        var variants = p.Variants.Select(MapVariantToDto).ToList();
        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            AiMetadata = p.AiMetadata,
            CreatedAt = p.CreatedAt,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? "Unknown",
            CreatedById = p.CreatedById,
            CreatedByEmail = p.CreatedBy?.Email ?? "Unknown",
            Variants = variants,
            PriceRange = ComputePriceRange(variants)
        };
    }

    /// <summary>
    /// Maps a product to a detail DTO — full tree including options and their values.
    /// Used for the product detail/edit view.
    /// </summary>
    private static ProductDto MapToDetailDto(Product p)
    {
        var options = p.Options.Select(o => new ProductOptionDto
        {
            Id = o.Id,
            Name = o.Name,
            AttributeId = o.AttributeId,
            Values = o.Values.Select(v => new ProductOptionValueDto
            {
                Id = v.Id,
                Value = v.Value
            }).ToList()
        }).ToList();

        var variants = p.Variants.Select(MapVariantToDto).ToList();

        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            AiMetadata = p.AiMetadata,
            CreatedAt = p.CreatedAt,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? "Unknown",
            CreatedById = p.CreatedById,
            CreatedByEmail = p.CreatedBy?.Email ?? "Unknown",
            Options = options,
            Variants = variants,
            PriceRange = ComputePriceRange(variants)
        };
    }

    private static ProductVariantDto MapVariantToDto(ProductVariant v)
    {
        return new ProductVariantDto
        {
            Id = v.Id,
            SKU = v.SKU,
            Price = v.Price,
            StockQuantity = v.StockQuantity,
            CreatedAt = v.CreatedAt,
            OptionValues = v.VariantOptionValues.Select(vov => new VariantOptionValueDto
            {
                OptionName = vov.ProductOptionValue?.ProductOption?.Name ?? string.Empty,
                Value = vov.ProductOptionValue?.Value ?? string.Empty
            }).ToList()
        };
    }

    private static PriceRangeDto? ComputePriceRange(List<ProductVariantDto> variants)
    {
        if (!variants.Any()) return null;
        return new PriceRangeDto
        {
            Min = variants.Min(v => v.Price),
            Max = variants.Max(v => v.Price)
        };
    }
}
