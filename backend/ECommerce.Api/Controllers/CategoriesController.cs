using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces;
using ECommerce.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return Ok(ApiResponse<CategoryDto>.Ok(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var result = await _categoryService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<CategoryDto>.Ok(result, "Category created successfully."));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        var result = await _categoryService.UpdateAsync(id, dto);
        return Ok(ApiResponse<CategoryDto>.Ok(result, "Category updated successfully."));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "Category deleted successfully."));
    }
}
