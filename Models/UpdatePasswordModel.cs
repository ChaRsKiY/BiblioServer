using System;
using System.ComponentModel.DataAnnotations;

namespace BiblioServer.Models
{
	public class UpdatePasswordModel
	{
        [Required]
        public string OldPassword { get; set; }

        [Required]
        //[RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W)", ErrorMessage = "passNotValid")]
        [MinLength(8, ErrorMessage = "passLessThan8Symbols")]
        public string NewPassword { get; set; }
    }
}

