using Midyaf.Models.Enums;

namespace Midyaf;

public class UserResponseDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public UserRole Role {get; set;}
    public string loginToken {get; set;}
}

