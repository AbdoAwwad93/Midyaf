using AutoMapper;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;
using Midyaf.UnitOfWork;

namespace Midyaf.Services.Implementations;

public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PropertyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse> GetAllPropertiesAsync()
    {
        var properties = await _unitOfWork.Properties.GetAllAsync();
        if (properties != null)
        {
            return ApiResponse.SuccessResponse("All properties retrieved successfully", properties);
        }
        return ApiResponse.FailureResponse("Error occurred while retrieving properties");
    }

    public async Task<ApiResponse> GetPropertyByIdAsync(int id)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property != null)
        {
            return ApiResponse.SuccessResponse("Property retrieved successfully", property);
        }
        return ApiResponse.FailureResponse("Property not found");
    }

    public async Task<ApiResponse> AddPropertyAsync(PropertyDTO PropertyDto)
    {
        var property = _mapper.Map<Property>(PropertyDto);
        await _unitOfWork.Properties.AddAsync(property);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Property added successfully", property);
    }

    public async Task<ApiResponse> UpdatePropertyAsync(int id, PropertyDTO PropertyDto)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null)
        {
            return ApiResponse.FailureResponse("No Property existed with this data");
        }
        _mapper.Map(PropertyDto, property);
        await _unitOfWork.Properties.UpdateAsync(property);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Property edited successfully", property);
    }

    public async Task<ApiResponse> DeletePropertyAsync(int id)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null)
        {
            return ApiResponse.FailureResponse("There is no Property with this data");
        }
        await _unitOfWork.Properties.RemoveAsync(property);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Property removed successfully");
    }

    public async Task<ApiResponse> SearchPropertiesAsync(PropertySearchDTO searchDto)
    {
        var result = await _unitOfWork.Properties.SearchAsync(searchDto);
        return ApiResponse.SuccessResponse($"Found {result.TotalCount} properties", result);
    }

    public async Task<ApiResponse> AddPropertyImageAsync(int PropertyId, string imageUrl)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(PropertyId);
        if (property == null)
        {
            return ApiResponse.FailureResponse("Property not found");
        }

        if (property.Images == null)
        {
            property.Images = new List<string>();
        }

        property.Images.Add(imageUrl);
        await _unitOfWork.Properties.UpdateAsync(property);
        await _unitOfWork.SaveAsync();

        return ApiResponse.SuccessResponse("Image added successfully", property.Images);
    }

    public async Task<ApiResponse> RemovePropertyImageAsync(int PropertyId, string imageUrl)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(PropertyId);
        if (property == null)
        {
            return ApiResponse.FailureResponse("Property not found");
        }

        if (property.Images == null || !property.Images.Contains(imageUrl))
        {
            return ApiResponse.FailureResponse("Image not found in Property");
        }

        property.Images.Remove(imageUrl);
        await _unitOfWork.Properties.UpdateAsync(property);
        await _unitOfWork.SaveAsync();

        return ApiResponse.SuccessResponse("Image removed successfully", property.Images);
    }
}
