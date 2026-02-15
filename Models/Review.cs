using System.ComponentModel.DataAnnotations.Schema;

namespace Midyaf.Models;

public class Review
{
    public int Id { get; set; }
    public string? Comment { get; set;}
    public DateTime CreatedAt { get; set; }
    public int Rate { get; set; }
    public virtual Property  Property { get; set; }
    [ForeignKey("Property")]
    public int PropertyId { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public virtual AppUser AppUser { get; set; }
}