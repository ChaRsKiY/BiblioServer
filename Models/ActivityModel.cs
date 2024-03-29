using System;
using System.ComponentModel.DataAnnotations;

namespace BiblioServer.Models
{
	public class ActivityModel
	{
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

		public string Email { get; set; }

		public DateTime Time { get; set; }

		public string Status { get; set; }
	}
}

