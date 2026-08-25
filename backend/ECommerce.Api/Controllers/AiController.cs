using ECommerce.Application.Interfaces;
using ECommerce.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;

    public AiController(IAiService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("generate-description")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateDescription([FromBody] GenerateDescriptionRequest request)
    {
        var result = await _aiService.GenerateProductDescriptionAsync(request.ProductName, request.CategoryName, request.AdditionalSpecs);
        return Ok(ApiResponse<string>.Ok(result, "Description generated successfully."));
    }

    [HttpPost("chat")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        var result = await _aiService.GetMarketingAdviceAsync(request.Prompt);
        return Ok(ApiResponse<string>.Ok(result, "Chat response generated."));
    }
}

public class GenerateDescriptionRequest
{
    public required string ProductName { get; set; }
    public string? CategoryName { get; set; }
    public string? AdditionalSpecs { get; set; }
}

public class ChatRequest
{
    public required string Prompt { get; set; }
}
