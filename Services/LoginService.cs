using System;
using BiblioServer.Models;
using BiblioServer.Repositories;
using BiblioServer.Interfaces;

namespace BiblioServer.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IActivityService _activityService;

        public LoginService(IUserRepository userRepository, ITokenService tokenService, IActivityService activityService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _activityService = activityService;
        }

        public async Task<string> LoginUserAsync(UserLoginModel loginModel)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginModel.Email);

            if (user == null || !VerifyPassword(loginModel.Password, user.HashedPassword))
            {
                return "invalidCredentials";
            }

            if (user.IsVerified == false)
            {
                return "emailNotVerified";
            }

            var token = _tokenService.GenerateJwtToken(user);

            var activityModel = new ActivityModel
            {
                Email = user.Email,
                Name = user.UserName,
                Time = DateTime.Now,
                Status = "Logined"
            };

            _activityService.AddActivity(activityModel);

            return token;
        }

        //Returns true if passwords matched and false if not
        private bool VerifyPassword(string enteredPassword, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, passwordHash);
        }
    }
}

