using System;
using BiblioServer.Models;
using BiblioServer.Repositories;

namespace BiblioServer.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public LoginService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
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

            return token;
        }

        //Returns true if passwords matched and false if not
        private bool VerifyPassword(string enteredPassword, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, passwordHash);
        }
    }
}

