using System;
namespace BiblioServer.Models
{
    public class Bookmark
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string UserId { get; set; }
       
        public int PageNumber { get; set; }

        public int? Color { get; set; }
    }

}

