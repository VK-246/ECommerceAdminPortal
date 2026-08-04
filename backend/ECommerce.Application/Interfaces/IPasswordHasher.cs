namespace ECommerce.Application.Interfaces;

/// <summary>
/// Contract for password hashing and verification.
/// Placed in Application as an interface; implemented in Infrastructure with BCrypt.
/// This keeps BCrypt.Net as an Infrastructure-only dependency.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
