using System;
using System.Threading.Tasks;
using BiblioServer.Models;
using BiblioServer.Repositories;

namespace BiblioServer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return user;
        }

        public async Task<User> GetUserByPasswordResetToken(string token)
        {
            var user = await _userRepository.GetUserByPasswordResetTokenAsync(token);
            return user;
        }

        public async Task<bool> IsEmailExists(string email)
        {
            var user = await _userRepository.IsEmailExistsAsync(email);
            return user;
        }

        public async Task<bool> IsUsernameExists(string username)
        {
            var user = await _userRepository.IsUsernameExistsAsync(username);
            return user;
        }
        public async Task<User> GetUserById(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user;
        }
        public async Task UpdateUser(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }
    }
}

