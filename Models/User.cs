namespace BiblioServer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

public class User
{
    [Key]
    public int Id { get; set; }
    //���������� ��� ������������.
    [Required]
    [StringLength(15, MinimumLength = 3)]
    [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
    public string UserName { get; set; }

    [MaxLength(20)]
    public string? Name { get; set; }

    [MaxLength(20)]
    public string? Surname { get; set; }

    //����������� ����� ������������.
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    public DateTime? RegistrationDate { get; set; }

    //������� ��������� ��� ��������.
    [MaxLength(500)]
    public string? Bio { get; set; }

    //���� �� ����������� ������� ������������.
    public string? Avatar { get; set; }

    //����������� ������ ������������.
    public string? HashedPassword { get; set; }

    //���� ��� ����������� ������.
    public string? Salt { get; set; }

    //����, �����������, �������� �� ������������ ���������������.
    public bool? IsAdmin { get; set; }

    //��������� ������� ���� ������������.
    //public ICollection<Book> FavoriteBooks { get; set; } = new List<Book>();
}