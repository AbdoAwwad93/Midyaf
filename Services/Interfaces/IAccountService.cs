using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;

namespace Midyaf.Services.Interfaces;

public interface IAccountService
{
    Task<GeneralResponse> RegisterAsync(RegisterDTO registerDto);
    Task<GeneralResponse> LoginAsync(LoginDTO loginDto);
    Task<GeneralResponse> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto);
    Task<GeneralResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDto);
    Task<string> GenerateJwtToken(AppUser user);
}

