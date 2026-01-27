using AutoMapper;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Services.Interfaces;
using Midyaf.UnitOfWork;

namespace Midyaf.Services.Implementations;

public class HotelService : IHotelService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HotelService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GeneralResponse> GetAllHotelsAsync()
    {
        var response = new GeneralResponse();
        var hotels = await _unitOfWork.Hotels.GetAllAsync();
        if (hotels != null)
        {
            response.SetResponse("All hotels retrieved successfully", true, Data: hotels);
            return response;
        }
        response.SetResponse("Error occurred while retrieving hotels", false);
        return response;
    }

    public async Task<GeneralResponse> GetHotelByIdAsync(int id)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel != null)
        {
            response.SetResponse("Hotel retrieved successfully", true, Data: hotel);
            return response;
        }
        response.SetResponse("Hotel not found", false);
        return response;
    }

    public async Task<GeneralResponse> AddHotelAsync(HotelDTO hotelDto)
    {
        var response = new GeneralResponse();
        var hotel = _mapper.Map<Hotel>(hotelDto);
        await _unitOfWork.Hotels.AddAsync(hotel);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Hotel added successfully", true, hotel);
        return response;
    }

    public async Task<GeneralResponse> UpdateHotelAsync(int id, HotelDTO hotelDto)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null)
        {
            response.SetResponse("No hotel existed with this data", false);
            return response;
        }
        _mapper.Map(hotelDto, hotel);
        await _unitOfWork.Hotels.UpdateAsync(hotel);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Hotel edited successfully", true, Data: hotel);
        return response;
    }

    public async Task<GeneralResponse> DeleteHotelAsync(int id)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null)
        {
            response.SetResponse("There is no hotel with this data", false);
            return response;
        }
        await _unitOfWork.Hotels.RemoveAsync(hotel);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Hotel removed successfully", true);
        return response;
    }
}
