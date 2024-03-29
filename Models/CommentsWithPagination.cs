using System;
namespace BiblioServer.Models
{
    public class CommentsWithPagination
    {
        public IEnumerable<CommentWithUsername> Comments { get; set; }
        public int TotalPages { get; set; }
    }

}

