using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Midyaf.Models;

public class AppUser:IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public virtual List<Review>?  Reviews { get; set; }
    public virtual List<Room> Rooms { get; set; }
    public virtual List<Reservation> Reservations { get; set; }
    
}