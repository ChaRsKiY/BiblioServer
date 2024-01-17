namespace BiblioServer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

public class UserUpdateModel
{
    public string? UserName { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Email { get; set; }

    public string? Bio { get; set; }

    public string? Avatar { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public IFormFile? AvatarFile { get; set; }

    //public ICollection<Book> FavoriteBooks { get; set; } = new List<Book>();
}