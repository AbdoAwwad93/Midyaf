using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midyaf.Models.DTOs;
using Midyaf.Services.Interfaces;

namespace Midyaf.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpGet("/hotel")]
    public async Task<IActionResult> GetAllHotels()
    {
        var response = await _hotelService.GetAllHotelsAsync();
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("/hotel/{id:int}")]
    public async Task<IActionResult> GetHotelById(int id)
    {
        var response = await _hotelService.GetHotelByIdAsync(id);
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }

    [HttpPost("/hotel/add")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add(HotelDTO hotelDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _hotelService.AddHotelAsync(hotelDto);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpPatch("/hotel/edit/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, HotelDTO hotelDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _hotelService.UpdateHotelAsync(id, hotelDto);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("hotel/delete/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _hotelService.DeleteHotelAsync(id);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}
