using System.ComponentModel.DataAnnotations;

namespace Midyaf.DTOs;

public class RegisterDTO
{
    [Required]
    [RegularExpression("[a-zA-Z]*$")]
    public string FirstName { get; set; }
    [Required]
    [RegularExpression("[a-zA-Z]*$")]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
    [Required]
    [RegularExpression("[a-zA-Z]*$]")]
    public string Country { get; set; }
    [Required]
    [RegularExpression("[a-zA-Z]*$]")]
    public string City { get; set; }
    [Required]
    [RegularExpression("[a-zA-Z]*$]")]
    public string Address { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}