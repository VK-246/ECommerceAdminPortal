using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces;
using ECommerce.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

/// <summary>
/// Handles user registration and login.
/// Both endpoints are public (no [Authorize] attribute).
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // -------------------------------------------------------
    // POST /api/auth/register
    // -------------------------------------------------------
    /// <summary>Register a new user account.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        // Service throws BadRequestException if email taken or role invalid.
        // GlobalExceptionMiddleware will catch that and return 400 automatically.
        var result = await _authService.RegisterAsync(dto);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<UserDto>.Ok(result, "User registered successfully."));
    }

    // -------------------------------------------------------
    // POST /api/auth/login
    // -------------------------------------------------------
    /// <summary>Login and receive a JWT token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // Service throws UnauthorizedException on bad credentials.
        // GlobalExceptionMiddleware catches it and returns 401.
        var result = await _authService.LoginAsync(dto);

        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
    }
}
