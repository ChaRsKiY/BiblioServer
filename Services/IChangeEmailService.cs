using System;
using BiblioServer.Models;

namespace BiblioServer.Services
{
	public interface IChangeEmailService
	{
		Task<string> GenerateChangeEmailToken(int userId);
		Task<string> CheckTokenAsync(User user, ChangeEmailModel model);
		Task<string> ConfirmChangingEmailAsync(ChangeEmailModel model);
    }
}

