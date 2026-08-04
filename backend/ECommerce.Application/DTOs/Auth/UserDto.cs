namespace ECommerce.Application.DTOs.Auth;

/// <summary>
/// Output model returned after a successful registration.
/// Does NOT include a token — user must log in explicitly after registering.
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
