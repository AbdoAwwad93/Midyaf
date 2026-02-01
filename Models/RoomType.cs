using System.ComponentModel.DataAnnotations.Schema;

namespace Midyaf.Models;

public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    
    [ForeignKey("Hotel")]
    public int HotelId { get; set; }
    public virtual Hotel Hotel { get; set; }
    
    public virtual ICollection<Room> Rooms { get; set; }
}
