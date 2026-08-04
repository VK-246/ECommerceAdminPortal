using ECommerce.Application.Interfaces;

namespace ECommerce.Infrastructure.Services.Auth;

/// <summary>
/// BCrypt implementation of IPasswordHasher.
/// BCrypt.Net is an Infrastructure dependency — Application layer never sees it directly.
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
