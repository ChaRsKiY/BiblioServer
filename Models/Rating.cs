using System.ComponentModel.DataAnnotations;

namespace BiblioServer.Models;

public class Rating
{
    [Key]
    public int Id { get; set; }

    [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
    public int Stars { get; set; }

    public int IdUser { get; set; }

    public int IdBook { get; set; }
}