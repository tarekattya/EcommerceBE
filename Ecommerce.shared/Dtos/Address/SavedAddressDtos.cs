namespace Ecommerce.Shared;

public record SavedAddressDto(int Id, string Label, string FirstName, string LastName, string Street, string City, string Country, bool IsDefault);

public record SavedAddressRequest(string Label, string FirstName, string LastName, string Street, string City, string Country, bool IsDefault = false);
