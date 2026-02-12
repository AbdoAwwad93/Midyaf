using Midyaf.Models.DTOs;
using Midyaf.Models.Response;

namespace Midyaf.Services.Interfaces;

public interface IHotelService
{
    Task<ApiResponse> GetAllHotelsAsync();
    Task<ApiResponse> GetHotelByIdAsync(int id);
    Task<ApiResponse> AddHotelAsync(HotelDTO hotelDto);
    Task<ApiResponse> UpdateHotelAsync(int id, HotelDTO hotelDto);
    Task<ApiResponse> DeleteHotelAsync(int id);
    Task<ApiResponse> SearchHotelsAsync(HotelSearchDTO searchDto);
    Task<ApiResponse> AddHotelImageAsync(int hotelId, string imageUrl);
    Task<ApiResponse> RemoveHotelImageAsync(int hotelId, string imageUrl);
}
