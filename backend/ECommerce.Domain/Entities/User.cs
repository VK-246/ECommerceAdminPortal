namespace ECommerce.Domain.Entities;

/// <summary>
/// Represents an authenticated user in the system.
/// Users are identified by a Guid (prevents ID enumeration attacks).
/// Each user has a role (Admin or Editor) that determines their access level.
/// </summary>
public class User
{
    /// <summary>
    /// Primary key — non-sequential Guid to prevent enumeration attacks.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User's email address — used as the login identifier.
    /// Must be unique across all users.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// BCrypt-hashed password — never store plaintext passwords.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// User role stored as string ("Admin" or "Editor") for human-readable DB values.
    /// Mapped to/from the UserRole enum in the application layer.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp of when the account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // --- Navigation Properties ---

    /// <summary>
    /// Products created by this user (one-to-many relationship for audit trail).
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
