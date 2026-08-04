using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

/// <summary>
/// Contract for JWT token generation.
/// Placed in Application layer as an interface so the Application layer can
/// call token generation without depending on the Infrastructure JWT libraries.
/// The actual implementation (TokenService) lives in Infrastructure.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a signed JWT token for the given user.
    /// Returns the token string and its expiry timestamp.
    /// </summary>
    (string token, DateTime expiresAt) GenerateToken(User user);
}
