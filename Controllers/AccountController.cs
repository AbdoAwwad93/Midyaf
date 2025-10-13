using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Midyaf.Core.DTOs;
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
    private readonly IMapper mapper;

    public AccountController(SignInManager<AppUser> signInManager
        ,UserManager<AppUser> userManager,IMapper mapper)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        this.mapper = mapper;
    }

    [HttpPost("Signup")]
    public async Task<IActionResult> SignUp(RegisterDTO registerDto)
    {
        var response = new GeneralResponse();
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        //AppUser appUser = new()
        //{
        //    FirstName = registerDto.FirstName,
        //    LastName = registerDto.LastName,
        //    UserName = registerDto.UserName,
        //    Email = registerDto.Email,
        //    PhoneNumber = registerDto.PhoneNumber,
        //    City = registerDto.City,
        //    Country = registerDto.Country,
        //    Address = registerDto.Address,
        //};
        var appUser = mapper.Map<AppUser>(registerDto);
        var appuser =  await _userManager.FindByEmailAsync(registerDto.Email);
        if (appuser != null)
        {
            response.SetResponse("Email already exists",false);
            return BadRequest(response);
        }
        var result = await _userManager.CreateAsync(appUser, registerDto.Password);
        if (!result.Succeeded)
        {
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
                var loginToken  = new JwtSecurityTokenHandler().WriteToken(token);
                response.SetResponse("Login done successfully",true,Data:loginToken);
                return Ok(response);
            }
        }
        response.SetResponse("Invalid Email or Password",false);
        return BadRequest(response);
    }
}