using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Midyaf.Infrastructure.Data;

public class AppDbContextFactory:IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.Load();
        var connectionstring = Environment.GetEnvironmentVariable("CONNECTION_STRING"); 
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionstring);
        return new AppDbContext(optionsBuilder.Options);
    }
}