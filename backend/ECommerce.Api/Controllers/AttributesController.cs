using ECommerce.Application.Interfaces;
using ECommerce.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

/// <summary>
/// Manages global product attributes (Color, Size, Storage, etc.).
/// These are the "templates" used when adding options to a product.
/// The Angular product form calls GET /api/attributes to populate the
/// "Add Option" dropdown when building a new product.
/// </summary>
[Authorize]
[ApiController]
[Route("api/attributes")]
public class AttributesController : ControllerBase
{
    private readonly IAppDbContext _context;

    public AttributesController(IAppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns all global attributes for use in the product options builder.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var attributes = _context.Attributes
            .OrderBy(a => a.Name)
            .Select(a => new { a.Id, a.Name, a.Description })
            .ToList();

        return Ok(ApiResponse<object>.Ok(attributes));
    }

    /// <summary>
    /// Creates a new global attribute (e.g., "Material").
    /// Restricted to Admin role — only admins can extend the global attribute catalog.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateAttributeRequest request)
    {
        var exists = _context.Attributes.Any(a => a.Name == request.Name);
        if (exists)
            return Conflict(ApiResponse<object>.Fail($"Attribute '{request.Name}' already exists."));

        var attribute = new Domain.Entities.Attribute
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Attributes.Add(attribute);
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<object>.Ok(new { attribute.Id, attribute.Name, attribute.Description },
                "Attribute created successfully."));
    }
}

public record CreateAttributeRequest(
    string Name,
    string? Description);
