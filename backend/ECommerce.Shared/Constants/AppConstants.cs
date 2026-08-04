namespace ECommerce.Shared.Constants;

/// <summary>
/// Application-wide constants. Centralized here so role names and default values
/// are consistent across all layers without magic strings.
/// </summary>
public static class AppConstants
{
    // --- Role Names ---
    // Used in JWT claims, [Authorize] attributes, and seed data.

    public const string RoleAdmin = "Admin";
    public const string RoleEditor = "Editor";

    // --- Default Values ---

    /// <summary>
    /// Default page number for paginated queries (1-based).
    /// </summary>
    public const int DefaultPage = 1;

    /// <summary>
    /// Default number of items per page.
    /// </summary>
    public const int DefaultPageSize = 10;

    /// <summary>
    /// Maximum allowed page size to prevent excessively large queries.
    /// </summary>
    public const int MaxPageSize = 50;
}
