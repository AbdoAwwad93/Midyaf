using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task<ApiResponse> GetAllPropertysAsync()
    {
        var Propertys = await _unitOfWork.Propertys.GetAllAsync();
        if (Propertys != null)
        {
            return ApiResponse.SuccessResponse("All Propertys retrieved successfully", Propertys);
        }
        return ApiResponse.FailureResponse("Error occurred while retrieving Propertys");
    }

    public async Task<ApiResponse> GetPropertyByIdAsync(int id)
    {
        var Property = await _unitOfWork.Propertys.GetByIdAsync(id);
        if (Property != null)
        {
            return ApiResponse.SuccessResponse("Property retrieved successfully", Property);
        }
        return ApiResponse.FailureResponse("Property not found");
    }

    public async Task<ApiResponse> AddPropertyAsync(PropertyDTO PropertyDto)
    {
        var property = _mapper.Map<Property>(PropertyDto);
        await _unitOfWork.Propertys.AddAsync(property);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Property added successfully", property);
    }

    public async Task<ApiResponse> UpdatePropertyAsync(int id, PropertyDTO PropertyDto)
    {
        var property = await _unitOfWork.Propertys.GetByIdAsync(id);
        if (property == null)
        {
            return ApiResponse.FailureResponse("No Property existed with this data");
        }
        _mapper.Map(PropertyDto, property);
        await _unitOfWork.Propertys.UpdateAsync(property);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Property edited successfully", property);
    }

    public async Task<ApiResponse> DeletePropertyAsync(int id)
    {
        var Property = await _unitOfWork.Propertys.GetByIdAsync(id);
        if (Property == null)
        {
            return ApiResponse.FailureResponse("There is no Property with this data");
        }
        await _unitOfWork.Propertys.RemoveAsync(Property);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Property removed successfully");
    }

    public async Task<ApiResponse> SearchPropertysAsync(PropertySearchDTO searchDto)
    {
        var query = _unitOfWork.Propertys.GetQueryable();
        if (!string.IsNullOrWhiteSpace(searchDto.Name))
        {
            query = query.Where(h => h.Name.ToLower().Contains(searchDto.Name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(searchDto.City))
        {
            query = query.Where(h => h.City.ToLower() == searchDto.City.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(searchDto.Country))
        {
            query = query.Where(h => h.Country.ToLower() == searchDto.Country.ToLower());
        }

        if (searchDto.MinRating.HasValue)
        {
            query = query.Where(h => h.Reviews != null && h.Reviews.Count > 0 && 
                h.Reviews.Average(r => r.Rate) >= searchDto.MinRating.Value);
        }

        if (searchDto.MinPrice.HasValue)
        {
            query = query.Where(h => h.Rooms.Any(r => r.Price >= searchDto.MinPrice.Value));
        }
        if (searchDto.MaxPrice.HasValue)
        {
            query = query.Where(h => h.Rooms.Any(r => r.Price <= searchDto.MaxPrice.Value));
        }

        if (searchDto.Guests.HasValue)
        {
            query = query.Where(h => h.Rooms.Any(r => r.Capacity >= searchDto.Guests.Value));
        }

        if (searchDto.CheckIn.HasValue && searchDto.CheckOut.HasValue)
        {
            var checkIn = searchDto.CheckIn.Value;
            var checkOut = searchDto.CheckOut.Value;
            
            query = query.Where(h => h.Rooms.Any(room => 
                room.IsAvailable &&
                (room.Reservations.All(res => res.CheckIn.Date == default || 
                    res.CheckOut <= checkIn || res.CheckIn >= checkOut))
            ));
        }

        query = searchDto.SortBy?.ToLower() switch
        {
            "name" => searchDto.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(h => h.Name) 
                : query.OrderBy(h => h.Name),
            "price" => searchDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(h => h.Rooms.Min(r => r.Price))
                : query.OrderBy(h => h.Rooms.Min(r => r.Price)),
            "rating" => searchDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(h => h.Reviews != null && h.Reviews.Count > 0 ? h.Reviews.Average(r => r.Rate) : 0)
                : query.OrderBy(h => h.Reviews != null && h.Reviews.Count > 0 ? h.Reviews.Average(r => r.Rate) : 0),
            _ => query.OrderBy(h => h.Name)
        };

        var totalCount = await query.CountAsync();
        var Propertys = await query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        var result = new PaginatedResult<Property>
        {
            Items = Propertys,
            TotalCount = totalCount,
            Page = searchDto.Page,
            PageSize = searchDto.PageSize
        };

        return ApiResponse.SuccessResponse($"Found {totalCount} Propertys", result);
    }

    public async Task<ApiResponse> AddPropertyImageAsync(int PropertyId, string imageUrl)
    {
        var Property = await _unitOfWork.Propertys.GetByIdAsync(PropertyId);
        if (Property == null)
        {
            return ApiResponse.FailureResponse("Property not found");
        }

        if (Property.Images == null)
        {
            Property.Images = new List<string>();
        }

        Property.Images.Add(imageUrl);
        await _unitOfWork.Propertys.UpdateAsync(Property);
        await _unitOfWork.SaveAsync();

        return ApiResponse.SuccessResponse("Image added successfully", Property.Images);
    }

    public async Task<ApiResponse> RemovePropertyImageAsync(int PropertyId, string imageUrl)
    {
        var Property = await _unitOfWork.Propertys.GetByIdAsync(PropertyId);
        if (Property == null)
        {
            return ApiResponse.FailureResponse("Property not found");
        }

        if (Property.Images == null || !Property.Images.Contains(imageUrl))
        {
            return ApiResponse.FailureResponse("Image not found in Property");
        }

        Property.Images.Remove(imageUrl);
        await _unitOfWork.Propertys.UpdateAsync(Property);
        await _unitOfWork.SaveAsync();

        return ApiResponse.SuccessResponse("Image removed successfully", Property.Images);
    }
}
