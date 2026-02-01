using AutoMapper;
using Midyaf.Models;
using Midyaf.Models.DTOs;
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

    public async Task<GeneralResponse> GetAllRoomsAsync()
    {
        var response = new GeneralResponse();
        var rooms = await _unitOfWork.Rooms.GetAllAsync();
        if (rooms != null)
        {
            response.SetResponse("All rooms retrieved successfully", true, Data: rooms);
            return response;
        }
        response.SetResponse("Error occurred while retrieving rooms", false);
        return response;
    }

    public async Task<GeneralResponse> GetRoomByIdAsync(int id)
    {
        var response = new GeneralResponse();
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room != null)
        {
            response.SetResponse("Room retrieved successfully", true, Data: room);
            return response;
        }
        response.SetResponse("Room not found", false);
        return response;
    }

    public async Task<GeneralResponse> GetRoomsByHotelIdAsync(int hotelId)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            response.SetResponse("Hotel not found", false);
            return response;
        }

        var allRooms = await _unitOfWork.Rooms.GetAllAsync();
        var hotelRooms = allRooms.Where(r => r.HotelId == hotelId).ToList();
        response.SetResponse($"Rooms for hotel {hotelId} retrieved successfully", true, Data: hotelRooms);
        return response;
    }

    public async Task<GeneralResponse> AddRoomAsync(RoomDTO roomDto)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(roomDto.HotelId);
        if (hotel == null)
        {
            response.SetResponse("Hotel not found", false);
            return response;
        }

        if (roomDto.RoomTypeId.HasValue)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(roomDto.RoomTypeId.Value);
            if (roomType == null)
            {
                response.SetResponse("Room Type not found", false);
                return response;
            }
        }

        var room = _mapper.Map<Room>(roomDto);
        await _unitOfWork.Rooms.AddAsync(room);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Room added successfully", true, room);
        return response;
    }

    public async Task<GeneralResponse> UpdateRoomAsync(int id, RoomDTO roomDto)
    {
        var response = new GeneralResponse();
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null)
        {
            response.SetResponse("Room not found", false);
            return response;
        }
        if (room.HotelId != roomDto.HotelId)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(roomDto.HotelId);
            if (hotel == null)
            {
                response.SetResponse("Hotel not found", false);
                return response;
            }
        }

        if (roomDto.RoomTypeId.HasValue)
        {
            var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(roomDto.RoomTypeId.Value);
            if (roomType == null)
            {
                response.SetResponse("Room Type not found", false);
                return response;
            }
        }

        _mapper.Map(roomDto, room);
        await _unitOfWork.Rooms.UpdateAsync(room);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Room updated successfully", true, Data: room);
        return response;
    }

    public async Task<GeneralResponse> DeleteRoomAsync(int id)
    {
        var response = new GeneralResponse();
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null)
        {
            response.SetResponse("Room not found", false);
            return response;
        }
        await _unitOfWork.Rooms.RemoveAsync(room);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Room deleted successfully", true);
        return response;
    }

    public async Task<GeneralResponse> AddRoomImageAsync(int roomId, string imageUrl)
    {
        var response = new GeneralResponse();
        var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
        if (room == null)
        {
            response.SetResponse("Room not found", false);
            return response;
        }

        if (room.ImagesUrls == null)
        {
            room.ImagesUrls = new List<string>();
        }

        room.ImagesUrls.Add(imageUrl);
        await _unitOfWork.Rooms.UpdateAsync(room);
        await _unitOfWork.SaveAsync();

        response.SetResponse("Image added successfully", true, Data: room.ImagesUrls);
        return response;
    }

    public async Task<GeneralResponse> RemoveRoomImageAsync(int roomId, string imageUrl)
    {
        var response = new GeneralResponse();
        var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
        if (room == null)
        {
            response.SetResponse("Room not found", false);
            return response;
        }

        if (room.ImagesUrls == null || !room.ImagesUrls.Contains(imageUrl))
        {
            response.SetResponse("Image not found in room", false);
            return response;
        }

        room.ImagesUrls.Remove(imageUrl);
        await _unitOfWork.Rooms.UpdateAsync(room);
        await _unitOfWork.SaveAsync();

        response.SetResponse("Image removed successfully", true, Data: room.ImagesUrls);
        return response;
    }
}
