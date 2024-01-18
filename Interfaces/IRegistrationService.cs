using System;
using BiblioServer.Models;

namespace BiblioServer.Services
{
	public interface IRegistrationService
	{
        Task<string> RegisterUserAsync(UserRegistrationModel user);
        Task ResentVerifyCode(User verificationModel);
        Task CompleteRegistration(User user);
    }
}

