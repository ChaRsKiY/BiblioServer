using System;
using System.ComponentModel.DataAnnotations;

namespace BiblioServer.Models
{
	public class DownloadReadUser
	{
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }

        public bool isDownloaded { get; set; }

        public bool isRead { get; set; }
    }
}

