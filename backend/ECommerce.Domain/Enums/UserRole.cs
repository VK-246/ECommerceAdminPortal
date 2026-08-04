namespace ECommerce.Domain.Enums;

/// <summary>
/// Defines the roles available in the system.
/// Admin: Full access to all features (categories, products, users).
/// Editor: Can manage products only.
/// </summary>
public enum UserRole
{
    Admin,
    Editor
}
