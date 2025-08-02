using System.ComponentModel.DataAnnotations.Schema;

namespace Midyaf.Models;

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }
    public List<string> ImagesUrls { get; set; }
    public virtual Hotel Hotel { get; set; }
    [ForeignKey("Hotel")]
    public int HotelId { get; set; }
    public virtual AppUser  AppUser { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public virtual Reservation Reservation { get; set; }
    [ForeignKey("Reservation")]
    public int ReservationId { get; set; }
}