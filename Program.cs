using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Midyaf.Core.Interfaces;
using Midyaf.Infrastructure.Data;
using Midyaf.Infrastructure.repository;
using Midyaf.Models;

namespace Midyaf;

public class Program
{
    public static void Main(string[] args)
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
        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        }).AddEntityFrameworkStores<AppDbContext>();
        var app = builder.Build();

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