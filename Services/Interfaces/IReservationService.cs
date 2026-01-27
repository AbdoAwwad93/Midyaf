using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;

namespace Midyaf.Services.Interfaces;

public interface IReservationService
{
    Task<GeneralResponse> GetAllReservationsAsync();
    Task<GeneralResponse> GetReservationByIdAsync(int id);
    Task<GeneralResponse> GetUserReservationsAsync(string userId);
    Task<GeneralResponse> CreateReservationAsync(ReservationDTO reservationDto, string userId);
    Task<GeneralResponse> UpdateReservationAsync(int id, ReservationDTO reservationDto);
    Task<GeneralResponse> UpdateStatusAsync(int id, Status status);
    Task<GeneralResponse> CancelReservationAsync(int id);
}
