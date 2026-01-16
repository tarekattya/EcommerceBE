namespace Ecommerce.Core;

    public interface IProductService
    {
        public Task<Result<Pagination<productResponse>>> GetAllAsync(ProductSpecParams specParams);

        public Task<Result<productResponse>> GetProductById(int id);


        public Task<Result<productResponse>> CreateProduct(ProductRequest product);

        public Task<Result<productResponse>> UpdateProduct(int id, ProductRequest request);

        public Task<Result> DeleteProduct(int id);

        public Task<Result<ProductFiltersResponse>> GetProductFiltersAsync();

    }

