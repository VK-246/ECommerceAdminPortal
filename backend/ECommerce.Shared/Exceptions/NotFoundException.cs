namespace ECommerce.Shared.Exceptions;

/// <summary>
/// Custom exception thrown when a requested resource is not found.
/// The global exception middleware (Epic 2) will catch this and return HTTP 404.
/// 
/// Usage: throw new NotFoundException("Category", id);
/// Result: "Category with ID 5 was not found."
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with ID {key} was not found.")
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }
}
