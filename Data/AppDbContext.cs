using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Midyaf.Models;

namespace Midyaf.Data;

public class AppDbContext:IdentityDbContext
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Room>  Rooms { get; set; }
    public DbSet<Review>  Reviews { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        :base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<AppUser>().HasMany(user => user.ManagedProperties)
            .WithOne(Property => Property.Manager)
            .HasForeignKey(Property=>Property.ManagerId);
        builder.Entity<AppUser>().HasMany(user => user.Rooms)
            .WithOne(room => room.AppUser);
        builder.Entity<AppUser>().HasMany(user => user.Reviews)
            .WithOne(review => review.AppUser);
        builder.Entity<AppUser>().HasMany(user => user.Reservations)
            .WithOne(reservation => reservation.AppUser);

        builder.Entity<Property>().HasMany(Property => Property.Rooms)
            .WithOne(room => room.Property);
        builder.Entity<Property>().HasMany(Property => Property.Reviews)
            .WithOne(review => review.Property);
        builder.Entity<Reservation>().Property(reservation => reservation.Status)
            .HasConversion<string>();
            
        builder.Entity<Reservation>()
            .HasMany(r => r.Rooms)
            .WithMany(r => r.Reservations);
        builder.Entity<AppUser>().Property(user => user.Role)
            .HasConversion<string>();
            builder.Entity<Property>().Property(p=>p.PropertyType)
            .HasConversion<string>();
        
        builder.Entity<Property>().ToTable("Properties");
    }
}