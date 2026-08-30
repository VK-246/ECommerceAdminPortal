namespace ECommerce.Domain.Entities;

/// <summary>
/// Join table that maps which ProductOptionValues make up each ProductVariant.
/// This implements the Many-to-Many relationship between variants and option values.
///
/// Uses a Composite Primary Key: (VariantId, OptionValueId).
/// This naturally enforces that the same option value cannot be applied
/// to the same variant twice.
///
/// Example: For the variant "TSHIRT-RED-M", there are two rows:
///   (VariantId=5, OptionValueId=1)  → Color=Red
///   (VariantId=5, OptionValueId=4)  → Size=Medium
/// </summary>
public class VariantOptionValue
{
    /// <summary>
    /// FK to the ProductVariant — part of the Composite PK.
    /// </summary>
    public int VariantId { get; set; }

    /// <summary>
    /// FK to the ProductOptionValue — part of the Composite PK.
    /// </summary>
    public int OptionValueId { get; set; }

    // --- Navigation Properties ---

    /// <summary>
    /// The variant this row belongs to.
    /// </summary>
    public ProductVariant ProductVariant { get; set; } = null!;

    /// <summary>
    /// The option value applied to this variant.
    /// </summary>
    public ProductOptionValue ProductOptionValue { get; set; } = null!;
}
