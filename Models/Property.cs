namespace Midyaf.Models;

public class Property
{
    public int  Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string ManagerId { get; set; }
    public virtual AppUser Manager { get; set; }
    public virtual List<Review>? Reviews { get; set; }
    public virtual List<Room> Rooms { get; set; }
    public virtual List<RoomType> RoomTypes { get; set; }
    public List<string> Images { get; set; } = new List<string>();
    public PropertyType PropertyType {get;set;}
}