using Microsoft.AspNetCore.Identity;
using Midyaf.Models.Enums;

namespace Midyaf.Models;

public class AppUser:IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public UserRole Role { get; set; }
    public virtual List<Review>?  Reviews { get; set; }
    public virtual List<Room> Rooms { get; set; }
    public virtual List<Reservation> Reservations { get; set; }
    public virtual List<Property> ManagedPropertys { get; set; }
}