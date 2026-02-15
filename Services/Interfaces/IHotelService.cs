using Midyaf.Models.DTOs;
using Midyaf.Models.Response;

namespace Midyaf.Services.Interfaces;

public interface IPropertyService
{
    Task<ApiResponse> GetAllPropertysAsync();
    Task<ApiResponse> GetPropertyByIdAsync(int id);
    Task<ApiResponse> AddPropertyAsync(PropertyDTO PropertyDto);
    Task<ApiResponse> UpdatePropertyAsync(int id, PropertyDTO PropertyDto);
    Task<ApiResponse> DeletePropertyAsync(int id);
    Task<ApiResponse> SearchPropertysAsync(PropertySearchDTO searchDto);
    Task<ApiResponse> AddPropertyImageAsync(int PropertyId, string imageUrl);
    Task<ApiResponse> RemovePropertyImageAsync(int PropertyId, string imageUrl);
}
