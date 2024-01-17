using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace BiblioServer.Models
{
	public class ChangeEmailModel
	{
		[EmailAddress]
		public string Email { get; set; }

		public string Code { get; set; }
	}
}

