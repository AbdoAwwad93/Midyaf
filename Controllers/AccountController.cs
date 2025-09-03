using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Midyaf.DTOs;
using Midyaf.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Midyaf.Controllers;
[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    public AccountController(SignInManager<AppUser> signInManager
        ,UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("Signup")]
    public async Task<IActionResult> SignUp(RegisterDTO registerDto)
    {
        var response = new GeneralResponse();
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        AppUser appUser = new()
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            City = registerDto.City,
            Country = registerDto.Country,
            Address = registerDto.Address,
        };
        var appuser =  await _userManager.FindByEmailAsync(registerDto.Email);
        if (appuser != null)
        {
            response.SetResponse("Email already exists",false);
            return BadRequest(response);
        }
        var result = await _userManager.CreateAsync(appUser, registerDto.Password);
        if (!result.Succeeded)
        {
          // response.SetResponse(string.Join(";",result.Errors.Select(e=>e.Description)),false);
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("",error.Description);
            }

            return BadRequest(ModelState);
        }
        return Created();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDTO loginDto)
    {
        var response = new  GeneralResponse();
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        AppUser? appUser = await _userManager.FindByEmailAsync(loginDto.Email);
        if (appUser != null)
        {
            var isValid = await _userManager.CheckPasswordAsync(appUser, loginDto.Password);
            if (isValid)
            {
                var Claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, appUser.Id)
                };
                var securityKey = Environment.GetEnvironmentVariable("SecurityKey");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey!));
                var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: Environment.GetEnvironmentVariable("Issuer"),
                    audience: Environment.GetEnvironmentVariable("Audience"),
                    claims: Claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );
                
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                });
            }
        }
        response.SetResponse("Invalid Email or Password",false);
        return BadRequest(response);
    }
}