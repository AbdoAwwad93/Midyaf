using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;

namespace Midyaf.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllReservations()
    {
        var response = await _reservationService.GetAllReservationsAsync();
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetReservationById(int id)
    {
        var response = await _reservationService.GetReservationByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyReservations()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(ApiResponse.FailureResponse("User not authenticated"));
        }
        var response = await _reservationService.GetUserReservationsAsync(userId);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create(ReservationDTO reservationDto)
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
        var response = await _reservationService.CreateReservationAsync(reservationDto, userId);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPatch("edit/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, ReservationDTO reservationDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        var response = await _reservationService.UpdateReservationAsync(id, reservationDto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPatch("status/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] Status status)
    {
        var response = await _reservationService.UpdateStatusAsync(id, status);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("cancel/{id:int}")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id)
    {
        var response = await _reservationService.CancelReservationAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
