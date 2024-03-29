using System;
using System.Threading.Tasks;
using BiblioServer.Models;
using BiblioServer.Repositories;

namespace BiblioServer.Services
{
	public interface IUserService
	{
		Task<User> GetUserByEmail(string email);
		Task UpdateUser(User user);
        Task<User> GetUserByPasswordResetToken(string token);
        Task<bool> IsEmailExists(string email);
        Task<bool> IsUsernameExists(string username);
        Task<User> GetUserById(int userId);
        Task<object> GetAllUsersAsync(int page);
        Task<object> GetAllAdminsAsync(int page);
    }
}

