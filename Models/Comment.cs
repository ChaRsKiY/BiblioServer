using System.ComponentModel.DataAnnotations;

namespace BiblioServer.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }

    //Comment text
    [Required(ErrorMessage = "Comment content is required.")]
    [MaxLength(500, ErrorMessage = "Comment content cannot exceed 500 characters.")]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int? IdUser { get; set; }

    public int IdBook { get; set; }
}