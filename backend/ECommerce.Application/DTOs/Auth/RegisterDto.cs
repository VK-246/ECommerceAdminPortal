using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Auth;

/// <summary>
/// Input model for the POST /api/auth/register endpoint.
/// Contains only the fields a new user is allowed to provide.
/// </summary>
public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Must be "Admin" or "Editor". Validated in the service layer.
    /// </summary>
    [Required]
    public string Role { get; set; } = string.Empty;
}
