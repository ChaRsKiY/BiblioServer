using System;
using BiblioServer.Context;
using BiblioServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblioServer.Repositories
{
	public class UserRepository : IUserRepository
	{
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByPasswordResetTokenAsync(string token)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.PasswordResetToken == token);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<object> GetAllUsersAsync(int page, int pageSize)
        {
            var query = _context.Users
                .Select(u => new User { UserName = u.UserName, Name = u.Name, Email = u.Email, Id = u.Id, IsAdmin = u.IsAdmin });

            var totalUsers = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            if (page < 1 || page > totalPages)
            {
                page = 1;
            }

            var users = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new { users, totalPages };
        }

        public async Task<object> GetAllAdminsAsync(int page, int pageSize)
        {
            var query = _context.Users
                .Select(u => new User { UserName = u.UserName, Name = u.Name, Email = u.Email, Id = u.Id, IsAdmin = u.IsAdmin }).Where(u => u.IsAdmin == true);

            var totalUsers = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            if (page < 1 || page > totalPages)
            {
                page = 1;
            }

            var users = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new { users, totalPages };
        }

    }
}

