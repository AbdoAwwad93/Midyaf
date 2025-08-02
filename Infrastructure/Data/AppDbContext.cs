using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Midyaf.Models;

namespace Midyaf.Infrastructure.Data;

public class AppDbContext:IdentityDbContext
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Hotel>  Hotels { get; set; }
    public DbSet<Room>  Rooms { get; set; }
    public DbSet<Review>  Reviews { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<AppUser>().HasMany(user => user.Rooms)
            .WithOne(room => room.AppUser);
        builder.Entity<AppUser>().HasMany(user => user.Reviews)
            .WithOne(review => review.AppUser);
        builder.Entity<AppUser>().HasMany(user => user.Reservations)
            .WithOne(reservation => reservation.AppUser);

        builder.Entity<Hotel>().HasMany(hotel => hotel.Rooms)
            .WithOne(room => room.Hotel);
        builder.Entity<Hotel>().HasMany(hotel => hotel.Reviews)
            .WithOne(review => review.Hotel);
        builder.Entity<Reservation>().Property(reservation => reservation.Status)
            .HasConversion<string>();
    }
}