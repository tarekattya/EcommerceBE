namespace Ecommerce.Core;

public class SavedAddressByUserSpec : BaseSpecifications<SavedAddress>
{
    public SavedAddressByUserSpec(string userId) : base(s => s.UserId == userId)
    {
        // no includes
    }
}

public class SavedAddressByIdAndUserSpec : BaseSpecifications<SavedAddress>
{
    public SavedAddressByIdAndUserSpec(int id, string userId) : base(s => s.Id == id && s.UserId == userId) { }
}
