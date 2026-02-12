using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Midyaf.Data;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
using Midyaf.Models.Response;
using Midyaf.Services.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Midyaf.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly AppDbContext _context;

    public AccountService(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IMapper mapper,
        IEmailService emailService,
        AppDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _mapper = mapper;
        _emailService = emailService;
        _context = context;
    }

    public async Task<ApiResponse> RegisterAsync(RegisterDTO registerDto)
    {
        var appUser = _mapper.Map<AppUser>(registerDto);
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return ApiResponse.FailureResponse("Email already exists");
        }
        var result = await _userManager.CreateAsync(appUser, registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse("Registration failed", errors);
        }

        await _userManager.AddToRoleAsync(appUser, registerDto.Role.ToString());
        return ApiResponse.SuccessResponse($"User with role {registerDto.Role} created successfully");
    }

    public async Task<ApiResponse> LoginAsync(LoginDTO loginDto)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(loginDto.Email);
        if (appUser == null)
        {
            return ApiResponse.FailureResponse("Invalid Email or Password");
        }

        var isValid = await _userManager.CheckPasswordAsync(appUser, loginDto.Password);
        if (!isValid)
        {
            return ApiResponse.FailureResponse("Invalid Email or Password");
        }

        var loginToken = await GenerateJwtToken(appUser);
        var userResponse = _mapper.Map<UserResponseDTO>(appUser);
        userResponse.loginToken = loginToken;
        return ApiResponse.SuccessResponse("Authentication successful", userResponse);
    }

    public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null)
        {
            return ApiResponse.SuccessResponse("If the email exists, an OTP has been sent");
        }

        // Invalidate any existing OTPs for this user
        var existingOtps = await _context.PasswordResetOtps
            .Where(o => o.UserId == user.Id && !o.IsUsed)
            .ToListAsync();
        foreach (var existingOtp in existingOtps)
        {
            existingOtp.IsUsed = true;
        }

        // Generate 6-digit OTP
        var otp = new Random().Next(100000, 999999).ToString();

        // Save OTP to database
        var passwordResetOtp = new PasswordResetOtp
        {
            UserId = user.Id,
            Otp = otp,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        _context.PasswordResetOtps.Add(passwordResetOtp);
        await _context.SaveChangesAsync();

        // Send OTP via email
        await _emailService.SendOtpAsync(user.Email!, otp);

        return ApiResponse.SuccessResponse("If the email exists, an OTP has been sent");
    }

    public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return ApiResponse.FailureResponse("Invalid reset request");
        }

        // Find valid OTP
        var otpRecord = await _context.PasswordResetOtps
            .Where(o => o.UserId == user.Id && 
                        o.Otp == resetPasswordDto.Otp && 
                        !o.IsUsed && 
                        o.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (otpRecord == null)
        {
            return ApiResponse.FailureResponse("Invalid or expired OTP");
        }

        // Mark OTP as used
        otpRecord.IsUsed = true;
        await _context.SaveChangesAsync();

        // Reset password using Identity
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse("Password reset failed", errors);
        }

        return ApiResponse.SuccessResponse("Password has been reset successfully");
    }
    public async Task<string> GenerateJwtToken(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var securityKey = Environment.GetEnvironmentVariable("SecurityKey");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("Issuer"),
            audience: Environment.GetEnvironmentVariable("Audience"),
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        var loginToken = new JwtSecurityTokenHandler().WriteToken(token);
        return loginToken;
    }
}
