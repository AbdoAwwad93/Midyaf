using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> SignUpUser(RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _accountService.RegisterAsync(registerDto, UserRole.User);
        return response.IsSuccess ? Created("", response) : BadRequest(response);
    }

    [HttpPost("signup/manager")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SignUpManager(RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _accountService.RegisterAsync(registerDto, UserRole.Manager);
        return response.IsSuccess ? Created("", response) : BadRequest(response);
    }

    [HttpPost("signup/admin")]
    public async Task<IActionResult> SignUpAdmin(RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _accountService.RegisterAsync(registerDto, UserRole.Admin);
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
