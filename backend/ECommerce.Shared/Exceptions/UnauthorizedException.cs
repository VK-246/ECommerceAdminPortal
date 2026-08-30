namespace ECommerce.Shared.Exceptions;

/// <summary>
/// Custom exception thrown when authentication fails (invalid credentials).
/// The global exception middleware will catch this and return HTTP 401.
///
/// Usage: throw new UnauthorizedException("Invalid email or password.");
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}
