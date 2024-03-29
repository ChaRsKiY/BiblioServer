using System;
using BiblioServer.Repositories;
using BiblioServer.Models;

namespace BiblioServer.Services
{
	public class ChangeEmailService : IChangeEmailService
	{
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public ChangeEmailService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<string> GenerateChangeEmailToken(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user != null)
            {
                var token = Guid.NewGuid().ToString().Substring(0, 7);

                user.EmailChangeToken = token;
                user.EmailChangeTokenExpiration = DateTime.UtcNow.AddHours(1);

                await _userRepository.UpdateUserAsync(user);

                return token;
            }
            else
            {
                return "userExist";
            }
        }

        public async Task<string> CheckTokenAsync(User user, ChangeEmailModel model)
        {
            if(user.EmailChangeToken == null || user.EmailChangeToken == "" || model.Code != user.EmailChangeToken)
            {
                return "invalidCode";
            }

            if(user.EmailChangeTokenExpiration > DateTime.Now)
            {
                return "expiredCode";
            }

            var checkEmailUser = await _userRepository.GetUserByEmailAsync(model.Email);
             
            if(checkEmailUser != null)
            {
                return "emailAlreadyBinded";
            }

            string verificationCode = Guid.NewGuid().ToString().Substring(0, 8);

            user.EmailChangeToken = verificationCode;
            user.EmailChangeTokenExpiration = DateTime.UtcNow.AddHours(1);
            user.NewChangingEmail = model.Email;
            await _userRepository.UpdateUserAsync(user);

            string callbackUrl = $"http://localhost:3000/emailchangeconfirm?email={user.Email}&verificationCode={verificationCode}";

            _emailService.SendChangeEmailVerificationEmail(model.Email, user.UserName, verificationCode, callbackUrl);

            return "";
        }

        public async Task<string> ConfirmChangingEmailAsync(ChangeEmailModel model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Email);

            if (model.Code != user.EmailChangeToken)
            {
                return "invalidCode";
            }

            if (user.EmailChangeTokenExpiration > DateTime.Now)
            {
                return "expiredCode";
            }

            user.Email = user.NewChangingEmail;
            user.NewChangingEmail = null;
            user.EmailChangeToken = null;
            user.EmailChangeTokenExpiration = null;
            await _userRepository.UpdateUserAsync(user);

            return "";
        }
    }
}

