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
    private readonly IFileService _fileService;

    public HotelController(IHotelService hotelService, IFileService fileService)
    {
        _hotelService = hotelService;
        _fileService = fileService;
    }

    [HttpGet("/hotel")]
    public async Task<IActionResult> GetAllHotels()
    {
        var response = await _hotelService.GetAllHotelsAsync();
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpGet("/hotel/search")]
    public async Task<IActionResult> SearchHotels([FromQuery] HotelSearchDTO searchDto)
    {
        var response = await _hotelService.SearchHotelsAsync(searchDto);
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

    [HttpPost("/hotel/{id:int}/images")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest("Invalid file type. Allowed types: jpg, jpeg, png, webp");
        }
        var imageUrl = await _fileService.SaveFileAsync(file, "hotels");
        var response = await _hotelService.AddHotelImageAsync(id, imageUrl);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("/hotel/{id:int}/images")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteImage(int id, [FromQuery] string imageUrl)
    {
        _fileService.DeleteFile(imageUrl, "hotels");
        var response = await _hotelService.RemoveHotelImageAsync(id, imageUrl);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}
