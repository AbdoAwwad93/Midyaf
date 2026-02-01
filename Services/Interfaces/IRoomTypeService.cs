using Midyaf.Models;
using Midyaf.Models.DTOs;

namespace Midyaf.Services.Interfaces;

public interface IRoomTypeService
{
    Task<GeneralResponse> GetAllRoomTypesAsync();
    Task<GeneralResponse> GetRoomTypesByHotelIdAsync(int hotelId);
    Task<GeneralResponse> GetRoomTypeByIdAsync(int id);
    Task<GeneralResponse> AddRoomTypeAsync(RoomTypeDTO roomTypeDto);
    Task<GeneralResponse> UpdateRoomTypeAsync(int id, RoomTypeDTO roomTypeDto);
    Task<GeneralResponse> DeleteRoomTypeAsync(int id);
}
