using System.ComponentModel.DataAnnotations;
using Midyaf.Models.Enums;

namespace Midyaf.Models.DTOs;

public class ReservationDTO
{
    [Required]
    public DateTime CheckIn { get; set; }

    [Required]
    public DateTime CheckOut { get; set; }

    [Required]
    [Range(1, 50, ErrorMessage = "Number of guests must be between 1 and 50")]
    public int NumberOfGuests { get; set; }

    public Status Status { get; set; } = Status.Pending;

    [Required]
    public List<int> RoomIds { get; set; } = new();
}
