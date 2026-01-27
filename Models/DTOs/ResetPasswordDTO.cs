using System.ComponentModel.DataAnnotations;

namespace Midyaf.Models.DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
