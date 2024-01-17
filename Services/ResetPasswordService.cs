using System;
using BiblioServer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BiblioServer.Services
{
	public class ResetPasswordService : IResetPasswordService
	{
        private readonly IUserRepository _userRepository;

        public ResetPasswordService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> GeneratePasswordResetToken(string userEmail)
        {
            var user = await _userRepository.GetUserByEmailAsync(userEmail);

            if (user != null)
            {
                var token = Guid.NewGuid().ToString();

                user.PasswordResetToken = token;
                user.PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(1);

                await _userRepository.UpdateUserAsync(user);

                return token;
            }
            else
            {
                return "userExist";
            }
        }
    }
}

