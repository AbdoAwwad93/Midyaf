using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Midyaf.Models;
using AutoMapper;
using System.Threading.Tasks;
using Midyaf.Data;
using Midyaf.UnitOfWork;
using Midyaf.Models.Mapping;
using Midyaf;
using Midyaf.Repository;
using Midyaf.Services.Implementations;
using Midyaf.Services.Interfaces;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json.Serialization;

namespace Midyaf;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        DotNetEnv.Env.Load();
        
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));
            
            options.AddPolicy("PasswordReset", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 5,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(15)
                    }));
            
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
        
        builder.Services.AddControllers().ConfigureApiBehaviorOptions(option =>
        {
            option.SuppressModelStateInvalidFilter = false;
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();
        var connnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        builder.Services.AddDbContext<AppDbContext>(option =>
        {
            option.UseNpgsql(connnectionString);
            option.UseLazyLoadingProxies();
        });
        builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
        builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfwork));
        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        }).AddEntityFrameworkStores<AppDbContext>();
        
        var securityKey = Environment.GetEnvironmentVariable("SecurityKey");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        { 
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(securityKey!))
            };
        });
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        
        // Register Services
        builder.Services.AddScoped<IPropertyService, PropertyService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IRoomService, RoomService>();
        builder.Services.AddScoped<IReservationService, ReservationService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
        
        var app = builder.Build();
        using (var scop = app.Services.CreateScope())
        {
            var roleManager = scop.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scop.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            await IdentityDataInitializer.SeedRoleAsync(roleManager);
            await IdentityDataInitializer.SeedAdmin(userManager);
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}