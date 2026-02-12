using System.ComponentModel.DataAnnotations;
using Midyaf.Models.Enums;

namespace Midyaf.Models.DTOs;

public class RegisterDTO
{
    [Required]
    [RegularExpression("[a-zA-Z]*$")]
    public string FirstName { get; set; }
    [Required]
    [RegularExpression("[a-zA-Z]*$",ErrorMessage = "Invalid Format")]
    public string LastName { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
    [Required]
    [RegularExpression("[a-zA-Z]*$",ErrorMessage = "Invalid Format")]
    public string Country { get; set; }
    [Required]
    [RegularExpression("[a-zA-Z]*$",ErrorMessage = "Invalid Format")]
    public string City { get; set; }
    [Required]
    [RegularExpression("[a-zA-Z]*$",ErrorMessage = "Invalid Format")]
    public string Address { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
    [Required]
    public UserRole Role {get; set;}
}