using System.ComponentModel.DataAnnotations;
namespace BiblioServer.Models;

//Model by signin up
public class UserRegistrationModel
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    //[RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W)", ErrorMessage = "passNotValid")]
    [MinLength(8, ErrorMessage = "passLessThan8Symbols")]
    public string Password { get; set; }
}

