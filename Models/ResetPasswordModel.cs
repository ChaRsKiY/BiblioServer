using System;
namespace BiblioServer.Models
{
	public class ResetPasswordModel
	{
        public string Token { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}

