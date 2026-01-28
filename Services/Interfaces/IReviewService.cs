using Midyaf.Models;
using Midyaf.Models.DTOs;

namespace Midyaf.Services.Interfaces;

public interface IReviewService
{
    Task<GeneralResponse> GetAllReviewsAsync();
    Task<GeneralResponse> GetReviewByIdAsync(int id);
    Task<GeneralResponse> GetReviewsByHotelIdAsync(int hotelId);
    Task<GeneralResponse> GetUserReviewsAsync(string userId);
    Task<GeneralResponse> CreateReviewAsync(ReviewDTO reviewDto, string userId);
    Task<GeneralResponse> UpdateReviewAsync(int id, ReviewDTO reviewDto, string userId);
    Task<GeneralResponse> DeleteReviewAsync(int id, string userId);
}
