using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioServer.Models;

//Book model
public class AddBookModel
{
    //Book name
    [MaxLength(60)]
    public string? Title { get; set; }

    [MaxLength(60)]
    public string? Author { get; set; }

    [ForeignKey("GenreId")]
    public int GenreId { get; set; }

    public IFormFile? Image { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    public int? Year { get; set; }

    public IFormFile? Content { get; set; }
}