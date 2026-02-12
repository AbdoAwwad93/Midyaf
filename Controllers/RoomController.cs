using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Midyaf.Models.DTOs;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;

namespace Midyaf.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IFileService _fileService;
    private readonly ILogger<RoomController> _logger;

    public RoomController(IRoomService roomService, IFileService fileService, ILogger<RoomController> logger)
    {
        _roomService = roomService;
        _fileService = fileService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRooms()
    {
        var response = await _roomService.GetAllRoomsAsync();
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRoomById(int id)
    {
        var response = await _roomService.GetRoomByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("hotel/{hotelId:int}")]
    public async Task<IActionResult> GetRoomsByHotelId(int hotelId)
    {
        var response = await _roomService.GetRoomsByHotelIdAsync(hotelId);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost("add")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Add(RoomDTO roomDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        var response = await _roomService.AddRoomAsync(roomDto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPatch("edit/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, RoomDTO roomDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        var response = await _roomService.UpdateRoomAsync(id, roomDto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("delete/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _roomService.DeleteRoomAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }
    [HttpPost("{id:int}/images")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse.FailureResponse("No file uploaded"));
        }
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(ApiResponse.FailureResponse("Invalid file type. Allowed types: jpg, jpeg, png, webp"));
        }

        var imageUrl = await _fileService.SaveFileAsync(file, "rooms");
        var response = await _roomService.AddRoomImageAsync(id, imageUrl);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:int}/images")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteImage(int id, [FromQuery] string imageUrl)
    {
        _fileService.DeleteFile(imageUrl, "rooms");
        var response = await _roomService.RemoveRoomImageAsync(id, imageUrl);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
