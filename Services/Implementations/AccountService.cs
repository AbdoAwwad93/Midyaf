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

    public async Task<GeneralResponse> RegisterAsync(RegisterDTO registerDto, UserRole role)
    {
        var response = new GeneralResponse();

        var appUser = _mapper.Map<AppUser>(registerDto);
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            response.SetResponse("Email already exists", false);
            return response;
        }

        appUser.Role = role;
        var result = await _userManager.CreateAsync(appUser, registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            response.SetResponse(errors, false);
            return response;
        }

        await _userManager.AddToRoleAsync(appUser, role.ToString());
        response.SetResponse($"User with role {role} created successfully", true, new { appUser.Email, appUser.Role });
        return response;
    }

    public async Task<GeneralResponse> LoginAsync(LoginDTO loginDto)
    {
        var response = new GeneralResponse();

        AppUser? appUser = await _userManager.FindByEmailAsync(loginDto.Email);
        if (appUser == null)
        {
            response.SetResponse("Invalid Email or Password", false);
            return response;
        }

        var isValid = await _userManager.CheckPasswordAsync(appUser, loginDto.Password);
        if (!isValid)
        {
            response.SetResponse("Invalid Email or Password", false);
            return response;
        }

        var roles = await _userManager.GetRolesAsync(appUser);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, appUser.Id)
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
        response.SetResponse("Authentication successful", true, Data: loginToken);
        return response;
    }

    public async Task<GeneralResponse> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto)
    {
        var response = new GeneralResponse();

        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null)
        {
            response.SetResponse("If the email exists, an OTP has been sent", true);
            return response;
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

        response.SetResponse("If the email exists, an OTP has been sent", true);
        return response;
    }

    public async Task<GeneralResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDto)
    {
        var response = new GeneralResponse();

        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            response.SetResponse("Invalid reset request", false);
            return response;
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
            response.SetResponse("Invalid or expired OTP", false);
            return response;
        }

        // Mark OTP as used
        otpRecord.IsUsed = true;
        await _context.SaveChangesAsync();

        // Reset password using Identity
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            response.SetResponse(errors, false);
            return response;
        }

        response.SetResponse("Password has been reset successfully", true);
        return response;
    }
}
