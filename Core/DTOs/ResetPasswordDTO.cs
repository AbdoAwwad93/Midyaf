using System.ComponentModel.DataAnnotations;

namespace Midyaf.Core.DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
