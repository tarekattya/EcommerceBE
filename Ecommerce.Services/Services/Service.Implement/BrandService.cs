namespace Ecommerce.Application;

public class BrandService(IGenericRepository<ProductBrand> Pbrepository , IGenericRepository<Product> prepository) : IBrandService
{
    private readonly IGenericRepository<ProductBrand> _pbrepository = Pbrepository;
    private readonly IGenericRepository<Product> _prepository = prepository;


    public async Task<Result<IReadOnlyList<BrandResponse>>> GetBrands()
    {
        var result = await _pbrepository.GetAllAsync();
        if (result is null)
            return Result<IReadOnlyList<BrandResponse>>.Failure(BrandErrors.NotFoundBrand);

        return Result<IReadOnlyList<BrandResponse>>.Success(result.Adapt<IReadOnlyList<BrandResponse>>());
    }


    public async Task<Result<BrandResponse>> GetBrandById(int id)
    {
        var barnd = await _pbrepository.GetByIdAsync(id);
        if(barnd is null)
            return Result<BrandResponse>.Failure(BrandErrors.NotFoundBrand);
        return Result<BrandResponse>.Success(barnd.Adapt<BrandResponse>());
    }
    public async Task<Result<BrandResponse>> CreateBrand(BrandRequest brand)
    {
        var exists = await _pbrepository.GetCountAsync(new BrandsByNameSpec(brand.Name));
        if (exists > 0)
            return Result<BrandResponse>.Failure(BrandErrors.BrandNameAlreadyExists);

        var newBrand = brand.Adapt<ProductBrand>();
        var createdBrand = await _pbrepository.AddAsync(newBrand);
        return Result<BrandResponse>.Success(createdBrand.Adapt<BrandResponse>());

    }
    public async Task<Result> UpdateBrand(int id , BrandRequest brand)
    {

        var exists = await _pbrepository.GetCountAsync(new BrandsByNameSpec(brand.Name));
        if (exists > 0)
            return Result<BrandResponse>.Failure(BrandErrors.BrandNameAlreadyExists);
        var existingBrand = await  _pbrepository.GetByIdAsync(id);
        if (existingBrand is null)
            return Result.Failure(BrandErrors.NotFoundBrand);
        existingBrand.Name = brand.Name;
        await _pbrepository.UpdateAsync(existingBrand);
        return Result.Success();

    }
    public async Task<Result> DeleteBrand(int id)
    {
        var existingBrand = await _pbrepository.GetByIdAsync(id);
        if (existingBrand is null)
            return Result.Failure(BrandErrors.NotFoundBrand);
        var Forcount = new ProductsByBrandSpec(id);
        var IsHasProduct = await _prepository.GetCountAsync(Forcount);

        if(IsHasProduct > 0)
            return Result.Failure(BrandErrors.BrandHasProducts);

        await _pbrepository.DeleteAsync(existingBrand);
        return Result.Success();



    }
}
