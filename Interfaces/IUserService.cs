using System;
using BiblioServer.Models;

namespace BiblioServer.Services
{
	public interface IUserService
	{
		Task<User> GetUserByEmail(string email);
		Task UpdateUser(User user);
	}
}

