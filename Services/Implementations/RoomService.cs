using AutoMapper;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;
using Midyaf.UnitOfWork;

namespace Midyaf.Services.Implementations;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoomService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse> GetAllRoomsAsync()
    {
        var rooms = await _unitOfWork.Rooms.GetAllAsync();
        if (rooms != null)
        {
            return ApiResponse.SuccessResponse("All rooms retrieved successfully", rooms);
        }
        return ApiResponse.FailureResponse("Error occurred while retrieving rooms");
    }

    public async Task<ApiResponse> GetRoomByIdAsync(int id)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room != null)
        {
            return ApiResponse.SuccessResponse("Room retrieved successfully", room);
        }
        return ApiResponse.FailureResponse("Room not found");
    }

    public async Task<ApiResponse> GetRoomsByPropertyIdAsync(int PropertyId)
    {
        var Property = await _unitOfWork.Propertys.GetByIdAsync(PropertyId);
        if (Property == null)
        {
            return ApiResponse.FailureResponse("Property not found");
        }

        var propertyRooms = await _unitOfWork.Rooms.FindAsync(r => r.PropertyId == PropertyId);
        return ApiResponse.SuccessResponse($"Rooms for Property {PropertyId} retrieved successfully", propertyRooms);
    }

    public async Task<ApiResponse> AddRoomAsync(RoomDTO roomDto)
    {
        var Property = await _unitOfWork.Propertys.GetByIdAsync(roomDto.PropertyId);
        if (Property == null)
        {
            return ApiResponse.FailureResponse("Property not found");
        }

        if (roomDto.RoomTypeId.HasValue)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(roomDto.RoomTypeId.Value);
            if (roomType == null)
            {
                return ApiResponse.FailureResponse("Room Type not found");
            }
        }

        var room = _mapper.Map<Room>(roomDto);
        await _unitOfWork.Rooms.AddAsync(room);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Room added successfully", room);
    }

    public async Task<ApiResponse> UpdateRoomAsync(int id, RoomDTO roomDto)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null)
        {
            return ApiResponse.FailureResponse("Room not found");
        }
        if (room.PropertyId != roomDto.PropertyId)
        {
            var Property = await _unitOfWork.Propertys.GetByIdAsync(roomDto.PropertyId);
            if (Property == null)
            {
                return ApiResponse.FailureResponse("Property not found");
            }
        }

        if (roomDto.RoomTypeId.HasValue)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(roomDto.RoomTypeId.Value);
            if (roomType == null)
            {
                return ApiResponse.FailureResponse("Room Type not found");
            }
        }

        _mapper.Map(roomDto, room);
        await _unitOfWork.Rooms.UpdateAsync(room);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Room updated successfully", room);
    }

    public async Task<ApiResponse> DeleteRoomAsync(int id)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null)
        {
            return ApiResponse.FailureResponse("Room not found");
        }
        await _unitOfWork.Rooms.RemoveAsync(room);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Room deleted successfully");
    }

    public async Task<ApiResponse> AddRoomImageAsync(int roomId, string imageUrl)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
        if (room == null)
        {
            return ApiResponse.FailureResponse("Room not found");
        }

        if (room.ImagesUrls == null)
        {
            room.ImagesUrls = new List<string>();
        }

        room.ImagesUrls.Add(imageUrl);
        await _unitOfWork.Rooms.UpdateAsync(room);
        await _unitOfWork.SaveAsync();

        return ApiResponse.SuccessResponse("Image added successfully", room.ImagesUrls);
    }

    public async Task<ApiResponse> RemoveRoomImageAsync(int roomId, string imageUrl)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
        if (room == null)
        {
            return ApiResponse.FailureResponse("Room not found");
        }

        if (room.ImagesUrls == null || !room.ImagesUrls.Contains(imageUrl))
        {
            return ApiResponse.FailureResponse("Image not found in room");
        }

        room.ImagesUrls.Remove(imageUrl);
        await _unitOfWork.Rooms.UpdateAsync(room);
        await _unitOfWork.SaveAsync();

        return ApiResponse.SuccessResponse("Image removed successfully", room.ImagesUrls);
    }
}
