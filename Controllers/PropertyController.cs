using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midyaf.Models.DTOs;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;

namespace Midyaf.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _PropertyService;
    private readonly IFileService _fileService;

    public PropertyController(IPropertyService PropertyService, IFileService fileService)
    {
        _PropertyService = PropertyService;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProperties()
    {
        var response = await _PropertyService.GetAllPropertiesAsync();
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProperties([FromQuery] PropertySearchDTO searchDto)
    {
        var response = await _PropertyService.SearchPropertiesAsync(searchDto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPropertyById(int id)
    {
        var response = await _PropertyService.GetPropertyByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost("add")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Add(PropertyDTO PropertyDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        var response = await _PropertyService.AddPropertyAsync(PropertyDto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPatch("edit/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, PropertyDTO PropertyDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        var response = await _PropertyService.UpdatePropertyAsync(id, PropertyDto);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("delete/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _PropertyService.DeletePropertyAsync(id);
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
        var imageUrl = await _fileService.SaveFileAsync(file, "Properties");
        var response = await _PropertyService.AddPropertyImageAsync(id, imageUrl);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:int}/images")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteImage(int id, [FromQuery] string imageUrl)
    {
        _fileService.DeleteFile(imageUrl, "Properties");
        var response = await _PropertyService.RemovePropertyImageAsync(id, imageUrl);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
