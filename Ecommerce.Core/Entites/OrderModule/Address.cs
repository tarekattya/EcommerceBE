
namespace Ecommerce.Core;

public class OrderAddress
{
    public OrderAddress()
    {
    }

    public OrderAddress(string city, string street, string country, string firstName, string lastName)
    {
        City = city;
        Street = street;
        Country = country;
        FirstName = firstName;
        LastName = lastName;
    }

    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

