using Ecommerce.Shared;

namespace Ecommerce.Application;

public class SavedAddressService(IUnitOfWork unitOfWork) : ISavedAddressService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IReadOnlyList<SavedAddressDto>>> GetUserAddressesAsync(string userId)
    {
        var list = await _unitOfWork.Repository<SavedAddress>().GetAllWithSpecAsync(new SavedAddressByUserSpec(userId));
        var dtos = list.Select(s => new SavedAddressDto(s.Id, s.Label, s.FirstName, s.LastName, s.Street, s.City, s.Country, s.IsDefault)).ToList();
        return Result<IReadOnlyList<SavedAddressDto>>.Success(dtos);
    }

    public async Task<Result<SavedAddressDto>> AddAsync(string userId, SavedAddressRequest request)
    {
        if (request.IsDefault)
            await ClearDefaultForUserAsync(userId);

        var entity = new SavedAddress
        {
            UserId = userId,
            Label = request.Label,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Street = request.Street,
            City = request.City,
            Country = request.Country,
            IsDefault = request.IsDefault
        };
        await _unitOfWork.Repository<SavedAddress>().AddAsync(entity);
        await _unitOfWork.CompleteAsync();
        return Result<SavedAddressDto>.Success(new SavedAddressDto(entity.Id, entity.Label, entity.FirstName, entity.LastName, entity.Street, entity.City, entity.Country, entity.IsDefault));
    }

    public async Task<Result<SavedAddressDto>> UpdateAsync(int id, string userId, SavedAddressRequest request)
    {
        var entity = await _unitOfWork.Repository<SavedAddress>().GetByIdWithSpecAsync(new SavedAddressByIdAndUserSpec(id, userId));
        if (entity is null)
            return Result<SavedAddressDto>.Failure(new Error("NotFound", "Address not found.", 404));

        if (request.IsDefault && !entity.IsDefault)
            await ClearDefaultForUserAsync(userId);

        entity.Label = request.Label;
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Street = request.Street;
        entity.City = request.City;
        entity.Country = request.Country;
        entity.IsDefault = request.IsDefault;

        _unitOfWork.Repository<SavedAddress>().Update(entity);
        await _unitOfWork.CompleteAsync();
        return Result<SavedAddressDto>.Success(new SavedAddressDto(entity.Id, entity.Label, entity.FirstName, entity.LastName, entity.Street, entity.City, entity.Country, entity.IsDefault));
    }

    public async Task<Result> DeleteAsync(int id, string userId)
    {
        var entity = await _unitOfWork.Repository<SavedAddress>().GetByIdWithSpecAsync(new SavedAddressByIdAndUserSpec(id, userId));
        if (entity is null)
            return Result.Failure(new Error("NotFound", "Address not found.", 404));
        _unitOfWork.Repository<SavedAddress>().Delete(entity);
        await _unitOfWork.CompleteAsync();
        return Result.Success();
    }

    public async Task<Result> SetDefaultAsync(int id, string userId)
    {
        var entity = await _unitOfWork.Repository<SavedAddress>().GetByIdWithSpecAsync(new SavedAddressByIdAndUserSpec(id, userId));
        if (entity is null)
            return Result.Failure(new Error("NotFound", "Address not found.", 404));
        await ClearDefaultForUserAsync(userId);
        entity.IsDefault = true;
        _unitOfWork.Repository<SavedAddress>().Update(entity);
        await _unitOfWork.CompleteAsync();
        return Result.Success();
    }

    private async Task ClearDefaultForUserAsync(string userId)
    {
        var list = await _unitOfWork.Repository<SavedAddress>().GetAllWithSpecAsync(new SavedAddressByUserSpec(userId));
        foreach (var a in list.Where(a => a.IsDefault))
        {
            a.IsDefault = false;
            _unitOfWork.Repository<SavedAddress>().Update(a);
        }
    }
}
