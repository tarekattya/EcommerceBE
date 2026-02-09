namespace Ecommerce.Core;

/// <summary>User's saved shipping address (multiple per user).</summary>
public class SavedAddress : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty; // e.g. "Home", "Office"
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
