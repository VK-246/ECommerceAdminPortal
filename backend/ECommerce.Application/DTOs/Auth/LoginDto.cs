using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Auth;

/// <summary>
/// Input model for the POST /api/auth/login endpoint.
/// </summary>
public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
