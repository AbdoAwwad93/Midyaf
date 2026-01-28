using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Midyaf.Models;

public class PasswordResetOtp
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; } = string.Empty;
    [ForeignKey("UserId")]
    public virtual AppUser? User { get; set; }
    [Required]
    [StringLength(6)]
    public string Otp { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
}
