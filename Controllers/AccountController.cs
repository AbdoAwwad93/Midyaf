using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;

namespace Midyaf.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        if(registerDto.Role == UserRole.Admin)
        {
            return BadRequest(ApiResponse.FailureResponse("Validation failed",new List<string> {"Invalid Role it must be User Or Manager"}));
        }
        var response = await _accountService.RegisterAsync(registerDto);
        return response.Success ? Created("", response) : BadRequest(response);
    }
    [HttpPost("signup/admin")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> SignUpAdmin(RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        if(registerDto.Role != UserRole.Admin)
        {
            return BadRequest(ApiResponse.FailureResponse("Validation failed",new List<string> {"Invalid Role it must be Admin"}));
        }
        var response = await _accountService.RegisterAsync(registerDto);
        return response.Success ? Created("", response) : BadRequest(response);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDTO loginDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        var response = await _accountService.LoginAsync(loginDto);
        return response.Success ? Ok(response) : Unauthorized(response);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("PasswordReset")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        var response = await _accountService.ForgotPasswordAsync(forgotPasswordDto);
        return Ok(response);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("PasswordReset")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
        }
        var response = await _accountService.ResetPasswordAsync(resetPasswordDto);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
