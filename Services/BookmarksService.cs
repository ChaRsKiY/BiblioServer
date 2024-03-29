using System;
using BiblioServer.Context;
using BiblioServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblioServer.Services
{
    public interface IBookmarkService
    {
        Task<IEnumerable<Bookmark>> GetBookmarksAsync(string id, int bookId);
        Task<Bookmark> CreateBookmarkAsync(Bookmark bookmark);
        Task DeleteBookmarkAsync(int bookmarkId, int page);
    }

    public class BookmarkService : IBookmarkService
    {
        private readonly ApplicationDbContext _context;

        public BookmarkService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bookmark>> GetBookmarksAsync(string id, int bookId)
        {
            return await _context.Bookmarks
                .Where(b => b.UserId == id && b.BookId == bookId)
                .ToListAsync();
        }

        public async Task<Bookmark> CreateBookmarkAsync(Bookmark bookmark)
        {
            try
            {
                var existingBookmark = await _context.Bookmarks.FirstOrDefaultAsync(b => b.PageNumber == bookmark.PageNumber);

                if (existingBookmark != null)
                {
                    _context.Bookmarks.Remove(existingBookmark);
                }

                _context.Bookmarks.Add(bookmark);
                await _context.SaveChangesAsync();
                return bookmark;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create bookmark.", ex);
            }
        }


        public async Task DeleteBookmarkAsync(int bookId, int page)
        {
            var bookmark = await _context.Bookmarks.FirstOrDefaultAsync(b => b.BookId == bookId && b.PageNumber == page);
            if (bookmark != null)
            {
                _context.Bookmarks.Remove(bookmark);
                await _context.SaveChangesAsync();
            }
        }
    }

}

