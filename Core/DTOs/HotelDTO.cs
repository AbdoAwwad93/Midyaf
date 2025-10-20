using System.ComponentModel.DataAnnotations;

namespace Midyaf.Core.DTOs
{
    public class HotelDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [RegularExpression("[a-zA-Z0-9 ]+$")]
        public string Address { get; set; }
        [Required]
        [RegularExpression("[a-zA-Z]+$")]
        public string City { get; set; }
        [RegularExpression("[a-zA-Z]+$")]
        public string Country { get; set; }
    }
}
