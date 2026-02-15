using Midyaf.Models.DTOs;
using Midyaf.Models.Response;

namespace Midyaf.Services.Interfaces;

public interface IRoomService
{
    Task<ApiResponse> GetAllRoomsAsync();
    Task<ApiResponse> GetRoomByIdAsync(int id);
    Task<ApiResponse> GetRoomsByPropertyIdAsync(int PropertyId);
    Task<ApiResponse> AddRoomAsync(RoomDTO roomDto);
    Task<ApiResponse> UpdateRoomAsync(int id, RoomDTO roomDto);
    Task<ApiResponse> DeleteRoomAsync(int id);
    Task<ApiResponse> AddRoomImageAsync(int roomId, string imageUrl);
    Task<ApiResponse> RemoveRoomImageAsync(int roomId, string imageUrl);
}
