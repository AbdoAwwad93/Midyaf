using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;
using Midyaf.UnitOfWork;

namespace Midyaf.Services.Implementations;

public class ReservationService : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly UserManager<AppUser> _userManager;

    public ReservationService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        IEmailService emailService,
        UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _emailService = emailService;
        _userManager = userManager;
    }

    public async Task<ApiResponse> GetAllReservationsAsync()
    {
        var reservations = await _unitOfWork.Reservations.GetAllAsync();
        if (reservations != null)
        {
            return ApiResponse.SuccessResponse("All reservations retrieved successfully", reservations);
        }
        return ApiResponse.FailureResponse("Error occurred while retrieving reservations");
    }

    public async Task<ApiResponse> GetReservationByIdAsync(int id)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
        if (reservation != null)
        {
            return ApiResponse.SuccessResponse("Reservation retrieved successfully", reservation);
        }
        return ApiResponse.FailureResponse("Reservation not found");
    }

    public async Task<ApiResponse> GetUserReservationsAsync(string userId)
    {
        var userReservations = await _unitOfWork.Reservations.FindAsync(r => r.UserId == userId);
        return ApiResponse.SuccessResponse("User reservations retrieved successfully", userReservations);
    }

    public async Task<ApiResponse> CreateReservationAsync(ReservationDTO reservationDto, string userId)
    {
        // Validate dates
        if (reservationDto.CheckIn >= reservationDto.CheckOut)
        {
            return ApiResponse.FailureResponse("Check-in date must be before check-out date");
        }

        if (reservationDto.CheckIn < DateTime.UtcNow.Date)
        {
            return ApiResponse.FailureResponse("Check-in date cannot be in the past");
        }

        // Validate rooms exist
        var rooms = new List<Room>();
        foreach (var roomId in reservationDto.RoomIds)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
            if (room == null)
            {
                return ApiResponse.FailureResponse($"Room with ID {roomId} not found");
            }
            if (!room.IsAvailable)
            {
                return ApiResponse.FailureResponse($"Room {room.RoomNumber} is not available");
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
        var user = await _userManager.FindByIdAsync(userId);
        if (user?.Email != null)
        {
            await _emailService.SendBookingConfirmationAsync(user.Email, reservation);
        }

        return ApiResponse.SuccessResponse("Reservation created successfully", reservation);
    }

    public async Task<ApiResponse> UpdateReservationAsync(int id, ReservationDTO reservationDto)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
        if (reservation == null)
        {
            return ApiResponse.FailureResponse("Reservation not found");
        }

        // Validate dates
        if (reservationDto.CheckIn >= reservationDto.CheckOut)
        {
            return ApiResponse.FailureResponse("Check-in date must be before check-out date");
        }

        _mapper.Map(reservationDto, reservation);
        reservation.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Reservations.UpdateAsync(reservation);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Reservation updated successfully", reservation);
    }

    public async Task<ApiResponse> UpdateStatusAsync(int id, Status status)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
        if (reservation == null)
        {
            return ApiResponse.FailureResponse("Reservation not found");
        }

        var previousStatus = reservation.Status;
        reservation.Status = status;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Reservations.UpdateAsync(reservation);
        await _unitOfWork.SaveAsync();

        if (reservation.UserId != null)
        {
            var user = await _userManager.FindByIdAsync(reservation.UserId);
            if (user?.Email != null)
            {
                if (status == Status.Confirmed && previousStatus != Status.Confirmed)
                {
                    await _emailService.SendBookingConfirmationAsync(user.Email, reservation);
                }
                if (status == Status.Declined)
                {
                    await _emailService.SendBookingCancellationAsync(user.Email, reservation);
                }
            }
        }

        return ApiResponse.SuccessResponse($"Reservation status updated to {status}", reservation);
    }

    public async Task<ApiResponse> CancelReservationAsync(int id)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(id);
        if (reservation == null)
        {
            return ApiResponse.FailureResponse("Reservation not found");
        }

        reservation.Status = Status.Declined;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Reservations.UpdateAsync(reservation);
        await _unitOfWork.SaveAsync();

        if (reservation.UserId != null)
        {
            var user = await _userManager.FindByIdAsync(reservation.UserId);
            if (user?.Email != null)
            {
                await _emailService.SendBookingCancellationAsync(user.Email, reservation);
            }
        }

        return ApiResponse.SuccessResponse("Reservation cancelled successfully");
    }
}
