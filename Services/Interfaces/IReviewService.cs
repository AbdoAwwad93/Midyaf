using Midyaf.Models.DTOs;
using Midyaf.Models.Response;

namespace Midyaf.Services.Interfaces;

public interface IReviewService
{
    Task<ApiResponse> GetAllReviewsAsync();
    Task<ApiResponse> GetReviewByIdAsync(int id);
    Task<ApiResponse> GetReviewsByPropertyIdAsync(int PropertyId);
    Task<ApiResponse> GetUserReviewsAsync(string userId);
    Task<ApiResponse> CreateReviewAsync(ReviewDTO reviewDto, string userId);
    Task<ApiResponse> UpdateReviewAsync(int id, ReviewDTO reviewDto, string userId);
    Task<ApiResponse> DeleteReviewAsync(int id, string userId);
}
