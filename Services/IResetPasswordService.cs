using System;
namespace BiblioServer.Services
{
	public interface IResetPasswordService
	{
        Task<string> GeneratePasswordResetToken(string userEmail);
    }
}

