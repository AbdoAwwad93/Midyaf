using AutoMapper;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
using Midyaf.Services.Interfaces;
using Midyaf.UnitOfWork;

namespace Midyaf.Services.Implementations;

public class ReservationService : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReservationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GeneralResponse> GetAllReservationsAsync()
    {
        var response = new GeneralResponse();
        var reservations = await _unitOfWork.Reservations.GetAllAsync();
        if (reservations != null)
        {
            response.SetResponse("All reservations retrieved successfully", true, Data: reservations);
            return response;
        }
        response.SetResponse("Error occurred while retrieving reservations", false);
        return response;
    }

    public async Task<GeneralResponse> GetReservationByIdAsync(int id)
    {
        var response = new GeneralResponse();
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
        if (reservation != null)
        {
            response.SetResponse("Reservation retrieved successfully", true, Data: reservation);
            return response;
        }
        response.SetResponse("Reservation not found", false);
        return response;
    }

    public async Task<GeneralResponse> GetUserReservationsAsync(string userId)
    {
        var response = new GeneralResponse();
        var allReservations = await _unitOfWork.Reservations.GetAllAsync();
        var userReservations = allReservations.Where(r => r.UserId == userId).ToList();
        response.SetResponse("User reservations retrieved successfully", true, Data: userReservations);
        return response;
    }

    public async Task<GeneralResponse> CreateReservationAsync(ReservationDTO reservationDto, string userId)
    {
        var response = new GeneralResponse();

        // Validate dates
        if (reservationDto.CheckIn >= reservationDto.CheckOut)
        {
            response.SetResponse("Check-in date must be before check-out date", false);
            return response;
        }

        if (reservationDto.CheckIn < DateTime.UtcNow.Date)
        {
            response.SetResponse("Check-in date cannot be in the past", false);
            return response;
        }

        // Validate rooms exist
        var rooms = new List<Room>();
        foreach (var roomId in reservationDto.RoomIds)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            if (room == null)
            {
                response.SetResponse($"Room with ID {roomId} not found", false);
                return response;
            }
            if (!room.IsAvailable)
            {
                response.SetResponse($"Room {room.RoomNumber} is not available", false);
                return response;
            }
            rooms.Add(room);
        }

        var reservation = _mapper.Map<Reservation>(reservationDto);
        reservation.UserId = userId;
        reservation.Status = Status.Pending;
        reservation.CreatedAt = DateTime.UtcNow;
        reservation.UpdatedAt = DateTime.UtcNow;
        reservation.Rooms = rooms;

        await _unitOfWork.Reservations.AddAsync(reservation);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Reservation created successfully", true, reservation);
        return response;
    }

    public async Task<GeneralResponse> UpdateReservationAsync(int id, ReservationDTO reservationDto)
    {
        var response = new GeneralResponse();
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
        if (reservation == null)
        {
            response.SetResponse("Reservation not found", false);
            return response;
        }

        // Validate dates
        if (reservationDto.CheckIn >= reservationDto.CheckOut)
        {
            response.SetResponse("Check-in date must be before check-out date", false);
            return response;
        }

        _mapper.Map(reservationDto, reservation);
        reservation.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Reservations.UpdateAsync(reservation);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Reservation updated successfully", true, Data: reservation);
        return response;
    }

    public async Task<GeneralResponse> UpdateStatusAsync(int id, Status status)
    {
        var response = new GeneralResponse();
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
        if (reservation == null)
        {
            response.SetResponse("Reservation not found", false);
            return response;
        }

        reservation.Status = status;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Reservations.UpdateAsync(reservation);
        await _unitOfWork.SaveAsync();
        response.SetResponse($"Reservation status updated to {status}", true, Data: reservation);
        return response;
    }

    public async Task<GeneralResponse> CancelReservationAsync(int id)
    {
        var response = new GeneralResponse();
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
        if (reservation == null)
        {
            response.SetResponse("Reservation not found", false);
            return response;
        }

        reservation.Status = Status.Declined;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Reservations.UpdateAsync(reservation);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Reservation cancelled successfully", true);
        return response;
    }
}
