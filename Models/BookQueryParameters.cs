using System;
namespace BiblioServer.Models
{
	public class BookQueryParameters
	{
        public string? SearchQuery { get; set; }
        public List<int>? Genres { get; set; }
        public List<int>? Stars { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortOrder { get; set; }
    }
}

