namespace Ecommerce.Core;

public class OrdersWithFilterForCountSpec : BaseSpecifications<Order>
{
    public OrdersWithFilterForCountSpec(string email, OrderSpecParams specParams) 
        : base(o => o.BuyerEmail == email && 
                   (!specParams.Status.HasValue || o.Status == specParams.Status) &&
                   (!specParams.MinAmount.HasValue || o.SubTotal >= specParams.MinAmount) &&
                   (!specParams.MaxAmount.HasValue || o.SubTotal <= specParams.MaxAmount) &&
                   (!specParams.FromDate.HasValue || o.OrderDate >= specParams.FromDate) &&
                   (!specParams.ToDate.HasValue || o.OrderDate <= specParams.ToDate) &&
                   (string.IsNullOrEmpty(specParams.Search) || o.BuyerEmail.Contains(specParams.Search)))
    {
    }
}
