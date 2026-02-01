using AutoMapper;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Services.Interfaces;
using Midyaf.UnitOfWork;

namespace Midyaf.Services.Implementations;

public class RoomTypeService : IRoomTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RoomTypeService> _logger;

    public RoomTypeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomTypeService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GeneralResponse> GetAllRoomTypesAsync()
    {
        var response = new GeneralResponse();
        var roomTypes = await _unitOfWork.RoomTypes.GetAllAsync();
        var roomTypeDtos = _mapper.Map<IEnumerable<RoomTypeDTO>>(roomTypes);
        response.SetResponse("Room Types retrieved successfully", true, roomTypeDtos);
        return response;
    }

    public async Task<GeneralResponse> GetRoomTypesByHotelIdAsync(int hotelId)
    {
        var response = new GeneralResponse();
        var roomTypes = await _unitOfWork.RoomTypes.FindAsync(rt => rt.HotelId == hotelId);
        if (!roomTypes.Any())
        {
            response.SetResponse($"No room types found for hotel {hotelId}", true, new List<RoomTypeDTO>());
            return response;
        }
        var roomTypeDtos = _mapper.Map<IEnumerable<RoomTypeDTO>>(roomTypes);
        response.SetResponse("Room Types retrieved successfully", true, roomTypeDtos);
        return response;
    }

    public async Task<GeneralResponse> GetRoomTypeByIdAsync(int id)
    {
        var response = new GeneralResponse();
        var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
        if (roomType == null)
        {
            response.SetResponse("Room Type not found", false);
            return response;
        }
        var roomTypeDto = _mapper.Map<RoomTypeDTO>(roomType);
        response.SetResponse("Room Type retrieved successfully", true, roomTypeDto);
        return response;
    }

    public async Task<GeneralResponse> AddRoomTypeAsync(RoomTypeDTO roomTypeDto)
    {
        var response = new GeneralResponse();
        
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(roomTypeDto.HotelId);
        if (hotel == null)
        {
            response.SetResponse("Hotel not found", false);
            return response;
        }

        var roomType = _mapper.Map<RoomType>(roomTypeDto);
        await _unitOfWork.RoomTypes.AddAsync(roomType);
        await _unitOfWork.SaveAsync();

        var createdDto = _mapper.Map<RoomTypeDTO>(roomType);
        response.SetResponse("Room Type created successfully", true, createdDto);
        return response;
    }

    public async Task<GeneralResponse> UpdateRoomTypeAsync(int id, RoomTypeDTO roomTypeDto)
    {
        var response = new GeneralResponse();
        var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
        if (roomType == null)
        {
            response.SetResponse("Room Type not found", false);
            return response;
        }
        if (roomType.HotelId != roomTypeDto.HotelId)
        {
             var hotel = await _unitOfWork.Hotels.GetByIdAsync(roomTypeDto.HotelId);
             if (hotel == null)
             {
                 response.SetResponse("Target Hotel not found", false);
                 return response;
             }
        }

        roomType.Name = roomTypeDto.Name;
        roomType.Description = roomTypeDto.Description;
        roomType.Price = roomTypeDto.Price;
        roomType.Capacity = roomTypeDto.Capacity;
        roomType.HotelId = roomTypeDto.HotelId;

        await _unitOfWork.RoomTypes.UpdateAsync(roomType);
        await _unitOfWork.SaveAsync();

        var updatedDto = _mapper.Map<RoomTypeDTO>(roomType);
        response.SetResponse("Room Type updated successfully", true, updatedDto);
        return response;
    }

    public async Task<GeneralResponse> DeleteRoomTypeAsync(int id)
    {
        var response = new GeneralResponse();
        var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
        if (roomType == null)
        {
            response.SetResponse("Room Type not found", false);
            return response;
        }

        if (roomType.Rooms != null && roomType.Rooms.Any())
        {
             response.SetResponse("Cannot delete Room Type because it has rooms assigned", false);
             return response;
        }

        await _unitOfWork.RoomTypes.RemoveAsync(roomType);
        await _unitOfWork.SaveAsync();

        response.SetResponse("Room Type deleted successfully", true);
        return response;
    }
}
