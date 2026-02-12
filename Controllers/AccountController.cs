using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
using Midyaf.Services.Interfaces;

namespace Midyaf.Controllers;

[ApiController]
[Route("[controller]")]
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
            return BadRequest(ModelState);
        }
        var response = await _accountService.RegisterAsync(registerDto);
        return response.IsSuccess ? Created("", response) : BadRequest(response);
    }
    [HttpPost("signup/admin")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> SignUpAdmin(RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _accountService.RegisterAsync(registerDto);
        return response.IsSuccess ? Created("", response) : BadRequest(response);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDTO loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _accountService.LoginAsync(loginDto);
        return response.IsSuccess ? Ok(response) : Unauthorized(response);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("PasswordReset")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
            return BadRequest(ModelState);
        }
        var response = await _accountService.ResetPasswordAsync(resetPasswordDto);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}

