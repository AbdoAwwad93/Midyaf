using System.ComponentModel.DataAnnotations.Schema;

namespace Midyaf.Models;

public class Reservation
{
    public int Id { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int NumberOfGuests { get; set; }
    public Status Status { get; set; }
    public virtual AppUser AppUser { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public virtual List<Room> Rooms { get; set; }
}