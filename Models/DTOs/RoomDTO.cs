using System.ComponentModel.DataAnnotations;

namespace Midyaf.Models.DTOs;

public class RoomDTO
{
    [Required]
    public string RoomNumber { get; set; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    [Required]
    [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; } = true;
    public List<string>? ImagesUrls { get; set; }
    [Required]
    public int HotelId { get; set; }
    public int? RoomTypeId { get; set; }
}
