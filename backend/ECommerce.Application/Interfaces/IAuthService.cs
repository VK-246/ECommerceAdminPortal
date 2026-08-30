using ECommerce.Application.DTOs.Auth;

namespace ECommerce.Application.Interfaces;

/// <summary>
/// Contract for authentication operations.
/// Controllers depend on this interface, never on AuthService directly.
/// This keeps the controller decoupled from the implementation.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user. Throws BadRequestException if email is already taken
    /// or if the role is invalid.
    /// </summary>
    Task<UserDto> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Validates credentials and returns a JWT token.
    /// Throws UnauthorizedException if email/password are incorrect.
    /// </summary>
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
