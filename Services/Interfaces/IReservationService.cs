using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
using Midyaf.Models.Response;

namespace Midyaf.Services.Interfaces;

public interface IReservationService
{
    Task<ApiResponse> GetAllReservationsAsync();
    Task<ApiResponse> GetReservationByIdAsync(int id);
    Task<ApiResponse> GetUserReservationsAsync(string userId);
    Task<ApiResponse> CreateReservationAsync(ReservationDTO reservationDto, string userId);
    Task<ApiResponse> UpdateReservationAsync(int id, ReservationDTO reservationDto);
    Task<ApiResponse> UpdateStatusAsync(int id, Status status);
    Task<ApiResponse> CancelReservationAsync(int id);
}
