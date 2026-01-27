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

namespace Midyaf;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        DotNetEnv.Env.Load();
        builder.Services.AddControllers().ConfigureApiBehaviorOptions(option =>
        {
            option.SuppressModelStateInvalidFilter = false;
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
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        
        // Register Services
        builder.Services.AddScoped<IHotelService, HotelService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        
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

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}