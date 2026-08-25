namespace ECommerce.Application.Interfaces;

public interface IAiService
{
    Task<string> GenerateProductDescriptionAsync(string productName, string? categoryName, string? additionalSpecs);
    Task<string> GetMarketingAdviceAsync(string prompt);
}
