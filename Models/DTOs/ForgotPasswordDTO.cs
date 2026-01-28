using System.ComponentModel.DataAnnotations;

namespace Midyaf.Models.DTOs;

public class ForgotPasswordDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
