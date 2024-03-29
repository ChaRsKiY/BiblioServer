using System;
using BiblioServer.Context;
using BiblioServer.Interfaces;
using BiblioServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblioServer.Repositories
{
	public class DownloadReadUserRepository : IDownloadReadUserRepository
	{
        private readonly ApplicationDbContext _context;

        public DownloadReadUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SetBookAsDownloadedAsync(int userId, int bookId)
        {
            var userBook = await _context.DRUserBook.FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == bookId);

            if (userBook == null)
            {
                userBook = new DownloadReadUser
                {
                    UserId = userId,
                    BookId = bookId,
                    isDownloaded = true,
                    isRead = false
                };
                await _context.DRUserBook.AddAsync(userBook);
            }
            else if (!userBook.isDownloaded)
            {
                userBook.isDownloaded = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SetBookAsReadAsync(int userId, int bookId)
        {
            var userBook = await _context.DRUserBook.FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == bookId);

            if (userBook == null)
            {
                userBook = new DownloadReadUser
                {
                    UserId = userId,
                    BookId = bookId,
                    isDownloaded = false,
                    isRead = true
                };
                await _context.DRUserBook.AddAsync(userBook);
            }
            else if (!userBook.isDownloaded)
            {
                userBook.isRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetDownloadsByBookAsync(int bookId)
        {
            var downloadCount = await _context.DRUserBook
                .Where(ub => ub.BookId == bookId && ub.isDownloaded)
                .CountAsync();

            return downloadCount;
        }

        public async Task<int> GetReadsByBookAsync(int bookId)
        {
            var downloadCount = await _context.DRUserBook
                .Where(ub => ub.BookId == bookId && ub.isRead)
                .CountAsync();

            return downloadCount;
        }
    }
}

