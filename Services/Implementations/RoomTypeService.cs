using AutoMapper;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Response;
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

    public async Task<ApiResponse> GetAllRoomTypesAsync()
    {
        var roomTypes = await _unitOfWork.RoomTypes.GetAllAsync();
        var roomTypeDtos = _mapper.Map<IEnumerable<RoomTypeDTO>>(roomTypes);
        return ApiResponse.SuccessResponse("Room Types retrieved successfully", roomTypeDtos);
    }

    public async Task<ApiResponse> GetRoomTypesByHotelIdAsync(int hotelId)
    {
        var roomTypes = await _unitOfWork.RoomTypes.FindAsync(rt => rt.HotelId == hotelId);
        if (!roomTypes.Any())
        {
            return ApiResponse.SuccessResponse($"No room types found for hotel {hotelId}", new List<RoomTypeDTO>());
        }
        var roomTypeDtos = _mapper.Map<IEnumerable<RoomTypeDTO>>(roomTypes);
        return ApiResponse.SuccessResponse("Room Types retrieved successfully", roomTypeDtos);
    }

    public async Task<ApiResponse> GetRoomTypeByIdAsync(int id)
    {
        var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
        if (roomType == null)
        {
            return ApiResponse.FailureResponse("Room Type not found");
        }
        var roomTypeDto = _mapper.Map<RoomTypeDTO>(roomType);
        return ApiResponse.SuccessResponse("Room Type retrieved successfully", roomTypeDto);
    }

    public async Task<ApiResponse> AddRoomTypeAsync(RoomTypeDTO roomTypeDto)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(roomTypeDto.HotelId);
        if (hotel == null)
        {
            return ApiResponse.FailureResponse("Hotel not found");
        }

        var roomType = _mapper.Map<RoomType>(roomTypeDto);
        await _unitOfWork.RoomTypes.AddAsync(roomType);
        await _unitOfWork.SaveAsync();

        var createdDto = _mapper.Map<RoomTypeDTO>(roomType);
        return ApiResponse.SuccessResponse("Room Type created successfully", createdDto);
    }

    public async Task<ApiResponse> UpdateRoomTypeAsync(int id, RoomTypeDTO roomTypeDto)
    {
        var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
        if (roomType == null)
        {
            return ApiResponse.FailureResponse("Room Type not found");
        }
        if (roomType.HotelId != roomTypeDto.HotelId)
        {
             var hotel = await _unitOfWork.Hotels.GetByIdAsync(roomTypeDto.HotelId);
             if (hotel == null)
             {
                 return ApiResponse.FailureResponse("Target Hotel not found");
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
        return ApiResponse.SuccessResponse("Room Type updated successfully", updatedDto);
    }

    public async Task<ApiResponse> DeleteRoomTypeAsync(int id)
    {
        var roomType = await _unitOfWork.RoomTypes.GetByIdAsync(id);
        if (roomType == null)
        {
            return ApiResponse.FailureResponse("Room Type not found");
        }

        if (roomType.Rooms != null && roomType.Rooms.Any())
        {
             return ApiResponse.FailureResponse("Cannot delete Room Type because it has rooms assigned");
        }

        await _unitOfWork.RoomTypes.RemoveAsync(roomType);
        await _unitOfWork.SaveAsync();

        return ApiResponse.SuccessResponse("Room Type deleted successfully");
    }
}
