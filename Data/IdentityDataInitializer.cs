using Microsoft.AspNetCore.Identity;
using Midyaf.Models;
using Midyaf.Models.Enums;

namespace Midyaf.Data;

public  class IdentityDataInitializer
{
    public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("Manager"))
        {
            await roleManager.CreateAsync(new IdentityRole("Manager"));
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
    }
    public static async Task SeedAdmin(UserManager<AppUser> userManager)
    {
        var user = await userManager.FindByEmailAsync("admin@midyaf.com");
        if(user == null)
        {
            var admin = new AppUser
            {
                FirstName = "Admin1",
                LastName = "User",
                UserName = "admin123",
                Email = "admin@midyaf.com",
                Role = UserRole.Admin
            };
            await userManager.CreateAsync(admin, "admin123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        
    }
}