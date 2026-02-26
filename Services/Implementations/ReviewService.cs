using AutoMapper;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;
using Midyaf.UnitOfWork;

namespace Midyaf.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse> GetAllReviewsAsync()
    {
        var reviews = await _unitOfWork.Reviews.GetAllAsync();
        if (reviews != null)
        {
            return ApiResponse.SuccessResponse("All reviews retrieved successfully", reviews);
        }
        return ApiResponse.FailureResponse("Error occurred while retrieving reviews");
    }

    public async Task<ApiResponse> GetReviewByIdAsync(int id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review != null)
        {
            return ApiResponse.SuccessResponse("Review retrieved successfully", review);
        }
        return ApiResponse.FailureResponse("Review not found");
    }

    public async Task<ApiResponse> GetReviewsByPropertyIdAsync(int PropertyId)
    {
        var Property = await _unitOfWork.Propertys.GetByIdAsync(PropertyId);
        if (Property == null)
        {
            return ApiResponse.FailureResponse("Property not found");
        }

        var propertyReviews = await _unitOfWork.Reviews.FindAsync(r => r.PropertyId == PropertyId);
        return ApiResponse.SuccessResponse($"Reviews for Property {PropertyId} retrieved successfully", propertyReviews);
    }

    public async Task<ApiResponse> GetUserReviewsAsync(string userId)
    {
        var userReviews = await _unitOfWork.Reviews.FindAsync(r => r.UserId == userId);
        return ApiResponse.SuccessResponse("User reviews retrieved successfully", userReviews);
    }

    public async Task<ApiResponse> CreateReviewAsync(ReviewDTO reviewDto, string userId)
    {
        var Property = await _unitOfWork.Propertys.GetByIdAsync(reviewDto.PropertyId);
        if (Property == null)
        {
            return ApiResponse.FailureResponse("Property not found");
        }

        var review = _mapper.Map<Review>(reviewDto);
        review.UserId = userId;
        review.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Review created successfully", review);
    }

    public async Task<ApiResponse> UpdateReviewAsync(int id, ReviewDTO reviewDto, string userId)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
        {
            return ApiResponse.FailureResponse("Review not found");
        }

        // Only the owner can update their review
        if (review.UserId != userId)
        {
            return ApiResponse.FailureResponse("You can only update your own reviews");
        }

        _mapper.Map(reviewDto, review);
        await _unitOfWork.Reviews.UpdateAsync(review);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Review updated successfully", review);
    }

    public async Task<ApiResponse> DeleteReviewAsync(int id, string userId)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
        {
            return ApiResponse.FailureResponse("Review not found");
        }

        // Only the owner can delete their review
        if (review.UserId != userId)
        {
            return ApiResponse.FailureResponse("You can only delete your own reviews");
        }

        await _unitOfWork.Reviews.RemoveAsync(review);
        await _unitOfWork.SaveAsync();
        return ApiResponse.SuccessResponse("Review deleted successfully");
    }
}
