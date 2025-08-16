using Ecommerce.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecification<T> where T : AuditLogging
    {
        public Expression<Func<T, bool>>? Criteria { get; set; } = null;
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();


        public BaseSpecifications()
        {
        }
        public BaseSpecifications(Expression<Func<T, bool>> cirteria)
        {
            Criteria = cirteria;
        }
    }
}
