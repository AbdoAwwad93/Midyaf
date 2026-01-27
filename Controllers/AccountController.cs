using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using DotNetEnv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Midyaf.Models;
using Midyaf.Models.DTOs;
using Midyaf.Models.Enums;
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

    [HttpPost("signup")]
    public async Task<IActionResult> SignUpUser(RegisterDTO registerDto)
    {
        return await SignUpHelper(registerDto, UserRole.User);
    }

    [HttpPost("signup/manager")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SignUpManager(RegisterDTO registerDto)
    {
        return await SignUpHelper(registerDto, UserRole.Manager);
    }
    [HttpPost("signup/admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SignUpAdmin(RegisterDTO registerDto)
    {
        return await SignUpHelper(registerDto, UserRole.Admin);
    }
    private async Task<IActionResult> SignUpHelper(RegisterDTO registerDto,UserRole role)
    {
        var response = new GeneralResponse();
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var appUser = mapper.Map<AppUser>(registerDto);
        var appuser =  await _userManager.FindByEmailAsync(registerDto.Email);
        if (appuser != null)
        {
            response.SetResponse("Email already exists",false);
            return BadRequest(response);
        }
        appUser.Role = role;
        var result = await _userManager.CreateAsync(appUser, registerDto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("",error.Description);
            }
            
            return BadRequest(ModelState);
        }
        //appuser.Role = role;
        await _userManager.AddToRoleAsync(appUser,role.ToString());
        response.SetResponse($"User with role {role} created successfully", true, new {appUser.Email,appUser.Role});
        return Created("",response);
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
            var roles = await _userManager.GetRolesAsync(appUser);
            if (isValid)
            {
                var Claims = new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, appUser.Id)
                };
                foreach(var role in roles)
                {
                    Claims.Add(new Claim(ClaimTypes.Role, role));
                }
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
                response.SetResponse("Authentication successfu", true,Data:loginToken);
                return Ok(response);
            }
        }
        response.SetResponse("Invalid Email or Password",false);
        return Unauthorized(response);
    }
}