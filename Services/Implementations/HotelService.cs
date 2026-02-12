using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;
using Midyaf.UnitOfWork;

namespace Midyaf.Services.Implementations;

public class HotelService : IHotelService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HotelService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse> GetAllHotelsAsync()
    {
        var hotels = await _unitOfWork.Hotels.GetAllAsync();
        if (hotels != null)
        {
            return ApiResponse.SuccessResponse("All hotels retrieved successfully", hotels);
        }
        return ApiResponse.FailureResponse("Error occurred while retrieving hotels");
    }

    public async Task<ApiResponse> GetHotelByIdAsync(int id)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel != null)
        {
            return ApiResponse.SuccessResponse("Hotel retrieved successfully", hotel);
        }
        return ApiResponse.FailureResponse("Hotel not found");
    }

    public async Task<ApiResponse> AddHotelAsync(HotelDTO hotelDto)
    {
        var hotel = _mapper.Map<Hotel>(hotelDto);
        await _unitOfWork.Hotels.AddAsync(hotel);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Hotel added successfully", hotel);
    }

    public async Task<ApiResponse> UpdateHotelAsync(int id, HotelDTO hotelDto)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null)
        {
            return ApiResponse.FailureResponse("No hotel existed with this data");
        }
        _mapper.Map(hotelDto, hotel);
        await _unitOfWork.Hotels.UpdateAsync(hotel);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Hotel edited successfully", hotel);
    }

    public async Task<ApiResponse> DeleteHotelAsync(int id)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null)
        {
            return ApiResponse.FailureResponse("There is no hotel with this data");
        }
        await _unitOfWork.Hotels.RemoveAsync(hotel);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Hotel removed successfully");
    }

    public async Task<ApiResponse> SearchHotelsAsync(HotelSearchDTO searchDto)
    {
        var query = _unitOfWork.Hotels.GetQueryable();
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
        var hotels = await query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        var result = new PaginatedResult<Hotel>
        {
            Items = hotels,
            TotalCount = totalCount,
            Page = searchDto.Page,
            PageSize = searchDto.PageSize
        };

        return ApiResponse.SuccessResponse($"Found {totalCount} hotels", result);
    }

    public async Task<ApiResponse> AddHotelImageAsync(int hotelId, string imageUrl)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            return ApiResponse.FailureResponse("Hotel not found");
        }

        if (hotel.Images == null)
        {
            hotel.Images = new List<string>();
        }

        hotel.Images.Add(imageUrl);
        await _unitOfWork.Hotels.UpdateAsync(hotel);
        await _unitOfWork.SaveAsync();

        return ApiResponse.SuccessResponse("Image added successfully", hotel.Images);
    }

    public async Task<ApiResponse> RemoveHotelImageAsync(int hotelId, string imageUrl)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            return ApiResponse.FailureResponse("Hotel not found");
        }

        if (hotel.Images == null || !hotel.Images.Contains(imageUrl))
        {
            return ApiResponse.FailureResponse("Image not found in hotel");
        }

        hotel.Images.Remove(imageUrl);
        await _unitOfWork.Hotels.UpdateAsync(hotel);
        await _unitOfWork.SaveAsync();

        return ApiResponse.SuccessResponse("Image removed successfully", hotel.Images);
    }
}
