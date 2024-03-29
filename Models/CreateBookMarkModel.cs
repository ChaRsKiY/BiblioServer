using System;
namespace BiblioServer.Models
{
    public class CreateBookmark
    {
        public int BookId { get; set; }

        public int PageNumber { get; set; }

        public int? Color { get; set; }
    }

}

