using Midyaf.Models.DTOs;
using Midyaf.Models.Response;

namespace Midyaf.Services.Interfaces;

public interface IRoomTypeService
{
    Task<ApiResponse> GetAllRoomTypesAsync();
    Task<ApiResponse> GetRoomTypesByPropertyIdAsync(int PropertyId);
    Task<ApiResponse> GetRoomTypeByIdAsync(int id);
    Task<ApiResponse> AddRoomTypeAsync(RoomTypeDTO roomTypeDto);
    Task<ApiResponse> UpdateRoomTypeAsync(int id, RoomTypeDTO roomTypeDto);
    Task<ApiResponse> DeleteRoomTypeAsync(int id);
}
