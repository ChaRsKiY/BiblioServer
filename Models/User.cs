namespace BiblioServer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

//Main user model
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 3)]
    [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
    public string UserName { get; set; }

    [MaxLength(20)]
    public string? Name { get; set; }

    [MaxLength(20)]
    public string? Surname { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    //Registration date
    public DateTime? RegistrationDate { get; set; }

    //User biography
    [MaxLength(500)]
    public string? Bio { get; set; }

    //Random encoded path for image
    public string? Avatar { get; set; }

    public string? HashedPassword { get; set; }

    //Salt for password encoding
    public string? Salt { get; set; }

    public bool? IsAdmin { get; set; }

    public string? VerificationCode { get; set; }

    public bool IsVerified { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetTokenExpiration { get; set; }

    public string? EmailChangeToken { get; set; }

    public DateTime? EmailChangeTokenExpiration { get; set; }

    public string? NewChangingEmail { get; set; }

    //public ICollection<Book> FavoriteBooks { get; set; } = new List<Book>();
}