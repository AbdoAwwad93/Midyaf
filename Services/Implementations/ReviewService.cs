using AutoMapper;
using Midyaf.Models;
using Midyaf.Models.DTOs;
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

    public async Task<GeneralResponse> GetAllReviewsAsync()
    {
        var response = new GeneralResponse();
        var reviews = await _unitOfWork.Reviews.GetAllAsync();
        if (reviews != null)
        {
            response.SetResponse("All reviews retrieved successfully", true, Data: reviews);
            return response;
        }
        response.SetResponse("Error occurred while retrieving reviews", false);
        return response;
    }

    public async Task<GeneralResponse> GetReviewByIdAsync(int id)
    {
        var response = new GeneralResponse();
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review != null)
        {
            response.SetResponse("Review retrieved successfully", true, Data: review);
            return response;
        }
        response.SetResponse("Review not found", false);
        return response;
    }

    public async Task<GeneralResponse> GetReviewsByHotelIdAsync(int hotelId)
    {
        var response = new GeneralResponse();
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
        if (hotel == null)
        {
            response.SetResponse("Hotel not found", false);
            return response;
        }

        var allReviews = await _unitOfWork.Reviews.GetAllAsync();
        var hotelReviews = allReviews.Where(r => r.HotelId == hotelId).ToList();
        response.SetResponse($"Reviews for hotel {hotelId} retrieved successfully", true, Data: hotelReviews);
        return response;
    }

    public async Task<GeneralResponse> GetUserReviewsAsync(string userId)
    {
        var response = new GeneralResponse();
        var allReviews = await _unitOfWork.Reviews.GetAllAsync();
        var userReviews = allReviews.Where(r => r.UserId == userId).ToList();
        response.SetResponse("User reviews retrieved successfully", true, Data: userReviews);
        return response;
    }

    public async Task<GeneralResponse> CreateReviewAsync(ReviewDTO reviewDto, string userId)
    {
        var response = new GeneralResponse();

        var hotel = await _unitOfWork.Hotels.GetByIdAsync(reviewDto.HotelId);
        if (hotel == null)
        {
            response.SetResponse("Hotel not found", false);
            return response;
        }

        var review = _mapper.Map<Review>(reviewDto);
        review.UserId = userId;
        review.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Review created successfully", true, review);
        return response;
    }

    public async Task<GeneralResponse> UpdateReviewAsync(int id, ReviewDTO reviewDto, string userId)
    {
        var response = new GeneralResponse();
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
        {
            response.SetResponse("Review not found", false);
            return response;
        }

        // Only the owner can update their review
        if (review.UserId != userId)
        {
            response.SetResponse("You can only update your own reviews", false);
            return response;
        }

        _mapper.Map(reviewDto, review);
        await _unitOfWork.Reviews.UpdateAsync(review);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Review updated successfully", true, Data: review);
        return response;
    }

    public async Task<GeneralResponse> DeleteReviewAsync(int id, string userId)
    {
        var response = new GeneralResponse();
        var review = await _unitOfWork.Reviews.GetByIdAsync(id);
        if (review == null)
        {
            response.SetResponse("Review not found", false);
            return response;
        }

        // Only the owner can delete their review
        if (review.UserId != userId)
        {
            response.SetResponse("You can only delete your own reviews", false);
            return response;
        }

        await _unitOfWork.Reviews.RemoveAsync(review);
        await _unitOfWork.SaveAsync();
        response.SetResponse("Review deleted successfully", true);
        return response;
    }
}
