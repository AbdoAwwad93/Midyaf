using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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

    public AccountService(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IMapper mapper)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _mapper = mapper;
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
}
