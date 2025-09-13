using Ecommerce.Core.Entites;
using Ecommerce.Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure
{
    public static class SpecificationEvaluator<T> where T : AuditLogging
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

            if (spec.Includes != null && spec.Includes.Any())
            {
                query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query;


        } 



    }
}
