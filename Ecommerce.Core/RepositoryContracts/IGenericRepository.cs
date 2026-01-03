namespace Ecommerce.Core;

    public interface IGenericRepository<T> where T : BaseEntity
    {

        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec);

        Task<int> GetCountAsync(ISpecification<T> spec);


        Task<T> AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

    }

