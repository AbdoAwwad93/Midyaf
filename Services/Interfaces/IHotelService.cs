using Midyaf.Models;
using Midyaf.Models.DTOs;

namespace Midyaf.Services.Interfaces;

public interface IHotelService
{
    Task<GeneralResponse> GetAllHotelsAsync();
    Task<GeneralResponse> GetHotelByIdAsync(int id);
    Task<GeneralResponse> AddHotelAsync(HotelDTO hotelDto);
    Task<GeneralResponse> UpdateHotelAsync(int id, HotelDTO hotelDto);
    Task<GeneralResponse> DeleteHotelAsync(int id);
    Task<GeneralResponse> SearchHotelsAsync(HotelSearchDTO searchDto);
    Task<GeneralResponse> AddHotelImageAsync(int hotelId, string imageUrl);
    Task<GeneralResponse> RemoveHotelImageAsync(int hotelId, string imageUrl);
}
