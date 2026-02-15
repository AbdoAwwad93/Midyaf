using System.ComponentModel.DataAnnotations;

namespace Midyaf.Models.DTOs;

public class RoomTypeDTO
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public int Capacity { get; set; }
    
    [Required]
    public int PropertyId { get; set; }
}
