using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;

namespace Midyaf.Services.Interfaces;

public interface IAccountService
{
    Task<GeneralResponse> RegisterAsync(RegisterDTO registerDto, UserRole role);
    Task<GeneralResponse> LoginAsync(LoginDTO loginDto);
}
