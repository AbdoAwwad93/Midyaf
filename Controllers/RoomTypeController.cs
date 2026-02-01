using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midyaf.Models.DTOs;
using Midyaf.Services.Interfaces;

namespace Midyaf.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomTypeController : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService;

    public RoomTypeController(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _roomTypeService.GetAllRoomTypesAsync();
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _roomTypeService.GetRoomTypeByIdAsync(id);
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }

    [HttpGet("hotel/{hotelId:int}")]
    public async Task<IActionResult> GetByHotelId(int hotelId)
    {
        var response = await _roomTypeService.GetRoomTypesByHotelIdAsync(hotelId);
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }

    [HttpPost("add")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Add(RoomTypeDTO roomTypeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _roomTypeService.AddRoomTypeAsync(roomTypeDto);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpPatch("edit/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, RoomTypeDTO roomTypeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _roomTypeService.UpdateRoomTypeAsync(id, roomTypeDto);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("delete/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _roomTypeService.DeleteRoomTypeAsync(id);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}
