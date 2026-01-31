using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
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

    public async Task<GeneralResponse> GetAllHotelsAsync()
    {
        var response = new GeneralResponse();
        var hotels = await _unitOfWork.Hotels.GetAllAsync();
        if (hotels != null)
        {
            response.SetResponse("All hotels retrieved successfully", true, Data: hotels);
            return response;
        }
        response.SetResponse("Error occurred while retrieving hotels", false);
        return response;
    }

    public async Task<GeneralResponse> GetHotelByIdAsync(int id)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel != null)
        {
            response.SetResponse("Hotel retrieved successfully", true, Data: hotel);
            return response;
        }
        response.SetResponse("Hotel not found", false);
        return response;
    }

    public async Task<GeneralResponse> AddHotelAsync(HotelDTO hotelDto)
    {
        var response = new GeneralResponse();
        var hotel = _mapper.Map<Hotel>(hotelDto);
        await _unitOfWork.Hotels.AddAsync(hotel);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Hotel added successfully", true, hotel);
        return response;
    }

    public async Task<GeneralResponse> UpdateHotelAsync(int id, HotelDTO hotelDto)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null)
        {
            response.SetResponse("No hotel existed with this data", false);
            return response;
        }
        _mapper.Map(hotelDto, hotel);
        await _unitOfWork.Hotels.UpdateAsync(hotel);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Hotel edited successfully", true, Data: hotel);
        return response;
    }

    public async Task<GeneralResponse> DeleteHotelAsync(int id)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null)
        {
            response.SetResponse("There is no hotel with this data", false);
            return response;
        }
        await _unitOfWork.Hotels.RemoveAsync(hotel);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Hotel removed successfully", true);
        return response;
    }

    public async Task<GeneralResponse> SearchHotelsAsync(HotelSearchDTO searchDto)
    {
        var response = new GeneralResponse();
        
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
                !room.Reservation.CheckIn.Date.Equals(default) == false || 
                (room.Reservation.CheckOut <= checkIn || room.Reservation.CheckIn >= checkOut) 
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

        response.SetResponse($"Found {totalCount} hotels", true, result);
        return response;
    }

    public async Task<GeneralResponse> AddHotelImageAsync(int hotelId, string imageUrl)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            response.SetResponse("Hotel not found", false);
            return response;
        }

        if (hotel.Images == null)
        {
            hotel.Images = new List<string>();
        }

        hotel.Images.Add(imageUrl);
        await _unitOfWork.Hotels.UpdateAsync(hotel);
        await _unitOfWork.SaveAsync();

        response.SetResponse("Image added successfully", true, Data: hotel.Images);
        return response;
    }

    public async Task<GeneralResponse> RemoveHotelImageAsync(int hotelId, string imageUrl)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            response.SetResponse("Hotel not found", false);
            return response;
        }

        if (hotel.Images == null || !hotel.Images.Contains(imageUrl))
        {
            response.SetResponse("Image not found in hotel", false);
            return response;
        }

        hotel.Images.Remove(imageUrl);
        await _unitOfWork.Hotels.UpdateAsync(hotel);
        await _unitOfWork.SaveAsync();

        response.SetResponse("Image removed successfully", true, Data: hotel.Images);
        return response;
    }
}
