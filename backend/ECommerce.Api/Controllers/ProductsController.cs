using System.Security.Claims;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces;
using ECommerce.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // ──────────────────────────────────────────────────────────────
    // PRODUCT ENDPOINTS
    // ──────────────────────────────────────────────────────────────

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResultDto<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null)
    {
        var result = await _productService.GetPagedAsync(page, pageSize, categoryId);
        return Ok(ApiResponse<PagedResultDto<ProductDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        // Extract the user's ID securely from their JWT token claims.
        // It's impossible for the client to spoof this value.
        var createdById = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("nameid")!);

        var result = await _productService.CreateAsync(dto, createdById);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<ProductDto>.Ok(result, "Product created successfully."));
    }

    /// <summary>
    /// Updates product metadata only (name, description, category).
    /// To update variant price/stock/SKU, use PATCH /api/products/{id}/variants/{variantId}.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto);
        return Ok(ApiResponse<ProductDto>.Ok(result, "Product updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "Product deleted successfully."));
    }

    // ──────────────────────────────────────────────────────────────
    // VARIANT ENDPOINTS — /api/products/{id}/variants
    // ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Updates a single variant's price, stock, and SKU.
    /// This is the focused endpoint for inventory management:
    ///   "Update the stock of the Red-Medium T-Shirt to 50 units."
    /// </summary>
    [HttpPatch("{id:int}/variants/{variantId:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductVariantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVariant(int id, int variantId, [FromBody] UpdateVariantDto dto)
    {
        var result = await _productService.UpdateVariantAsync(id, variantId, dto);
        return Ok(ApiResponse<ProductVariantDto>.Ok(result, "Variant updated successfully."));
    }
}
