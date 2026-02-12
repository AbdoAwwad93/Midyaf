using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
using Midyaf.Models.Response;

namespace Midyaf.Services.Interfaces;

public interface IAccountService
{
    Task<ApiResponse> RegisterAsync(RegisterDTO registerDto);
    Task<ApiResponse> LoginAsync(LoginDTO loginDto);
    Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDto);
    Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDto);
    Task<string> GenerateJwtToken(AppUser user);
}
