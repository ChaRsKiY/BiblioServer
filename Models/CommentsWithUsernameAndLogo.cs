using System.ComponentModel.DataAnnotations;

namespace BiblioServer.Models;

public class CommentWithUsername
{
    public int CommentId { get; set; }

    public string Content { get; set; }

    public int IdUser { get; set; }

    public string Username { get; set; }

    public string Avatar { get; set; }

    public int IdBook { get; set; }
}