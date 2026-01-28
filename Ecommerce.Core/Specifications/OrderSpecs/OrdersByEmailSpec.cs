namespace Ecommerce.Core;

public class OrdersByEmailSpec : BaseSpecifications<Order>
{
    public OrdersByEmailSpec(string email, OrderSpecParams specParams) 
        : base(o => o.BuyerEmail == email && 
                   (!specParams.Status.HasValue || o.Status == specParams.Status) &&
                   (!specParams.MinAmount.HasValue || o.SubTotal >= specParams.MinAmount) &&
                   (!specParams.MaxAmount.HasValue || o.SubTotal <= specParams.MaxAmount) &&
                   (!specParams.FromDate.HasValue || o.OrderDate >= specParams.FromDate) &&
                   (!specParams.ToDate.HasValue || o.OrderDate <= specParams.ToDate) &&
                   (string.IsNullOrEmpty(specParams.Search) || o.BuyerEmail.Contains(specParams.Search)))
    {
        Includes.Add(o => o.DeliveryMethod);
        Includes.Add(o => o.Items);

        if (!string.IsNullOrEmpty(specParams.Sort))
        {
            switch (specParams.Sort)
            {
                case "DateAsc":
                    AddOrderBy(o => o.OrderDate);
                    break;
                case "DateDesc":
                    AddOrderByDesc(o => o.OrderDate);
                    break;
                case "TotalAsc":
                    AddOrderBy(o => o.SubTotal); // Simplified, ideally total with delivery
                    break;
                case "TotalDesc":
                    AddOrderByDesc(o => o.SubTotal);
                    break;
                default:
                    AddOrderByDesc(o => o.OrderDate);
                    break;
            }
        }
        else
        {
            AddOrderByDesc(o => o.OrderDate);
        }

        ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
    }
}
