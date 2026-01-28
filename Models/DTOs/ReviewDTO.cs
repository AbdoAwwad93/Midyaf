using System.ComponentModel.DataAnnotations;

namespace Midyaf.Models.DTOs;

public class ReviewDTO
{
    public string? Comment { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5")]
    public int Rate { get; set; }

    [Required]
    public int HotelId { get; set; }
}
