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
    public virtual Property Property { get; set; }
    [ForeignKey("Property")]
    public int PropertyId { get; set; }
    public virtual AppUser  AppUser { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
    [ForeignKey("RoomType")]
    public int? RoomTypeId { get; set; }
    public virtual RoomType? RoomType { get; set; }
}