namespace ECommerce.Shared.Exceptions;

/// <summary>
/// Custom exception thrown for validation failures or invalid requests.
/// The global exception middleware (Epic 2) will catch this and return HTTP 400.
/// 
/// Usage: throw new BadRequestException("Cannot delete category with existing products.");
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message)
        : base(message)
    {
    }
}
