using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midyaf.Models.DTOs;
using Midyaf.Services.Interfaces;

namespace Midyaf.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRooms()
    {
        var response = await _roomService.GetAllRoomsAsync();
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRoomById(int id)
    {
        var response = await _roomService.GetRoomByIdAsync(id);
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }

    [HttpGet("hotel/{hotelId:int}")]
    public async Task<IActionResult> GetRoomsByHotelId(int hotelId)
    {
        var response = await _roomService.GetRoomsByHotelIdAsync(hotelId);
        return response.IsSuccess ? Ok(response) : NotFound(response);
    }

    [HttpPost("add")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Add(RoomDTO roomDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _roomService.AddRoomAsync(roomDto);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpPatch("edit/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, RoomDTO roomDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _roomService.UpdateRoomAsync(id, roomDto);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("delete/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _roomService.DeleteRoomAsync(id);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}
