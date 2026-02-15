using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midyaf.Models.DTOs;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;

namespace Midyaf.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReviews()
    {
        var response = await _reviewService.GetAllReviewsAsync();
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReviewById(int id)
    {
        var response = await _reviewService.GetReviewByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("Property/{PropertyId:int}")]
    public async Task<IActionResult> GetReviewsByPropertyId(int PropertyId)
    {
        var response = await _reviewService.GetReviewsByPropertyIdAsync(PropertyId);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyReviews()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(ApiResponse.FailureResponse("User not authenticated"));
        }
        var response = await _reviewService.GetUserReviewsAsync(userId);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> Create(ReviewDTO reviewDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(ApiResponse.FailureResponse("User not authenticated"));
        }
        var response = await _reviewService.CreateReviewAsync(reviewDto, userId);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPatch("edit/{id:int}")]
    [Authorize]
    public async Task<IActionResult> Edit(int id, ReviewDTO reviewDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(ApiResponse.FailureResponse("User not authenticated"));
        }
        var response = await _reviewService.UpdateReviewAsync(id, reviewDto, userId);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("delete/{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(ApiResponse.FailureResponse("User not authenticated"));
        }
        var response = await _reviewService.DeleteReviewAsync(id, userId);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
