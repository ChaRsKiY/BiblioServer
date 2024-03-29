using System;
using BiblioServer.Interfaces;
using BiblioServer.Models;
using BiblioServer.Repositories;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BiblioServer.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IActivityService _activityService;

        public RegistrationService(IActivityService activityService, IUserRepository userRepository, ITokenService tokenService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _activityService = activityService;
        }

        public async Task<string> RegisterUserAsync(UserRegistrationModel user)
        {
            if (await _userRepository.IsEmailExistsAsync(user.Email))
            {
                return "emailExist";
            }

            if (await _userRepository.IsUsernameExistsAsync(user.UserName))
            {
                return "usernameExist";
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);

            string verificationCode = Guid.NewGuid().ToString().Substring(0, 8);

            var newUser = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                Salt = salt,
                RegistrationDate = DateTime.Now,
                HashedPassword = HashPassword(user.Password, salt),
                Avatar = "standart.jpg",
                VerificationCode = verificationCode
            };

            await _userRepository.AddUserAsync(newUser);

            string callbackUrl = $"http://localhost:3000/confirmation?email={user.Email}&verificationCode={verificationCode}";

            await _emailService.SendVerificationEmail(newUser, verificationCode, callbackUrl);

            var activityModel = new ActivityModel
            {
                Email = newUser.Email,
                Name = newUser.UserName,
                Time = DateTime.Now,
                Status = "Registered"
            };

            _activityService.AddActivity(activityModel);

            var token = _tokenService.GenerateJwtToken(newUser);

            return token;
        }

        public async Task ResentVerifyCode(User user)
        {
            string newVerificationCode = Guid.NewGuid().ToString().Substring(0, 8);
            user.VerificationCode = newVerificationCode;

            await _userRepository.UpdateUserAsync(user);

            string callbackUrl = $"http://localhost:3000/confirmation?email={user.Email}&verificationCode={newVerificationCode}";

            await _emailService.SendVerificationEmail(user, newVerificationCode, callbackUrl);
        }

        public async Task CompleteRegistration(User user)
        {
            user.VerificationCode = null;
            user.IsVerified = true;

            await _userRepository.UpdateUserAsync(user);
        }

        private string HashPassword(string password, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }
    }
}

