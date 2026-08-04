namespace ECommerce.Application.DTOs.Auth;

/// <summary>
/// Output model returned after a successful login.
/// Contains the JWT token and the user's identity details.
/// The Angular frontend will read this and store the token in localStorage.
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
