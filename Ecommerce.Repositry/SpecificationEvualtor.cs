namespace Ecommerce.Infrastructure;

public static class SpecificationEvaluator<T> where T : BaseEntity
{

   public static IQueryable<T> GetQuery(IQueryable<T> InputQuery , ISpecification<T> spec)
   {
        var query = InputQuery;
        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }



        if(spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);
        else if(spec.OrderByDesc != null)
            query = query.OrderByDescending(spec.OrderByDesc);

        if(spec.IsEnablerPagination)
            query = query.Skip(spec.Skip).Take(spec.Take);

        if (spec.Includes != null && spec.Includes.Any())
        {
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        }

        return query;


    } 



}
