namespace Ecommerce.Application;

public class BrandService(IUnitOfWork unitOfWork, ICacheService cacheService) : IBrandService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<Pagination<BrandResponse>>> GetBrands(BrandSpecParams specParams)
    {
        var spec = new BrandWithSearchSpec(specParams);
        var countSpec = new BrandCountSpec(specParams);
        
        var brands = await _unitOfWork.Repository<ProductBrand>().GetAllWithSpecAsync(spec);
        var count = await _unitOfWork.Repository<ProductBrand>().GetCountAsync(countSpec);

        if (brands is null)
            return Result<Pagination<BrandResponse>>.Failure(BrandErrors.NotFoundBrand);

        var data = brands.Adapt<IReadOnlyList<BrandResponse>>();
        return Result<Pagination<BrandResponse>>.Success(new Pagination<BrandResponse>(specParams.PageIndex, specParams.PageSize, count, data));
    }


    public async Task<Result<BrandResponse>> GetBrandById(int id)
    {
        var barnd = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(id);
        if(barnd is null)
            return Result<BrandResponse>.Failure(BrandErrors.NotFoundBrand);
        return Result<BrandResponse>.Success(barnd.Adapt<BrandResponse>());
    }

    public async Task<Result<BrandResponse>> CreateBrand(BrandRequest brand)
    {
        int exists = await _unitOfWork.Repository<ProductBrand>().GetCountAsync(new BrandsByNameSpec(brand.Name));
        if (exists > 0)
            return Result<BrandResponse>.Failure(BrandErrors.BrandNameAlreadyExists);

        var newBrand = brand.Adapt<ProductBrand>();
        var createdBrand = await _unitOfWork.Repository<ProductBrand>().AddAsync(newBrand);
        if (createdBrand is null)
            return Result<BrandResponse>.Failure(BrandErrors.CantCreateBrand);
        await _unitOfWork.CompleteAsync();

        await _cacheService.RemoveCacheByPrefixAsync("/api/brands");

        return Result<BrandResponse>.Success(createdBrand.Adapt<BrandResponse>());

    }
    public async Task<Result> UpdateBrand(int id , BrandRequest brand)
    {

        var exists = await _unitOfWork.Repository<ProductBrand>().GetCountAsync(new BrandsByNameSpec(brand.Name));
        if (exists > 0)
            return Result<BrandResponse>.Failure(BrandErrors.BrandNameAlreadyExists);
        var existingBrand = await  _unitOfWork.Repository<ProductBrand>().GetByIdAsync(id);
        if (existingBrand is null)
            return Result.Failure(BrandErrors.NotFoundBrand);
        existingBrand.Name = brand.Name;
         _unitOfWork.Repository<ProductBrand>().Update(existingBrand);
        await _unitOfWork.CompleteAsync(); 
        
        await _cacheService.RemoveCacheByPrefixAsync("/api/brands");

        return Result.Success();

    }
    public async Task<Result> DeleteBrand(int id)
    {
        var existingBrand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(id);
        if (existingBrand is null)
            return Result.Failure(BrandErrors.NotFoundBrand);
        var Forcount = new ProductsByBrandSpec(id);
        var IsHasProduct = await _unitOfWork.Repository<Product>().GetCountAsync(Forcount);

        if(IsHasProduct > 0)
            return Result.Failure(BrandErrors.BrandHasProducts);

        _unitOfWork.Repository<ProductBrand>().Delete(existingBrand);
        await _unitOfWork.CompleteAsync();

        await _cacheService.RemoveCacheByPrefixAsync("/api/brands");

        return Result.Success();

    }
}
