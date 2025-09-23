
namespace Ecommerce.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecification<T> where T : AuditLogging
    {
        public Expression<Func<T, bool>>? Criteria { get; set; } = null;
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>>? OrderBy { get; set; }
        public Expression<Func<T, object>>? OrderByDesc { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsEnablerPagination { get; set; }

        public BaseSpecifications()
        {
        }
        public BaseSpecifications(Expression<Func<T, bool>> cirteria)
        {
            Criteria = cirteria;
        }


        public void AddOrderBy(Expression<Func<T , object>> OrderByExp)
        {
            OrderBy = OrderByExp;
        }
        public void AddOrderByDesc(Expression<Func<T, object>> OrderByDescExp)
        {
            OrderByDesc = OrderByDescExp;
        }

        public void ApplyPagination(int skip, int take)
        {
            IsEnablerPagination = true;
            Skip = skip;
            Take = take;
        }

    }
}
