using Midyaf.Models;
using Midyaf.Models.DTOs;

namespace Midyaf.Services.Interfaces;

public interface IRoomService
{
    Task<GeneralResponse> GetAllRoomsAsync();
    Task<GeneralResponse> GetRoomByIdAsync(int id);
    Task<GeneralResponse> GetRoomsByHotelIdAsync(int hotelId);
    Task<GeneralResponse> AddRoomAsync(RoomDTO roomDto);
    Task<GeneralResponse> UpdateRoomAsync(int id, RoomDTO roomDto);
    Task<GeneralResponse> DeleteRoomAsync(int id);
    Task<GeneralResponse> AddRoomImageAsync(int roomId, string imageUrl);
    Task<GeneralResponse> RemoveRoomImageAsync(int roomId, string imageUrl);
}
