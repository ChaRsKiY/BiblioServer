using System;
using System.Drawing.Printing;
using BiblioServer.Context;
using BiblioServer.Models;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace BiblioServer.Repositories
{
	public class BookRepository : IBookRepository
	{
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetBooksAsync(BookQueryParameters queryParameters)
        {
            var query = _context.Books.AsQueryable();

            // Применение фильтров
            if (!string.IsNullOrEmpty(queryParameters.SearchQuery))
            {
                query = query.Where(b => b.Title.Contains(queryParameters.SearchQuery) || b.Author.Contains(queryParameters.SearchQuery));
            }

            if (queryParameters.Genres != null && queryParameters.Genres.Any())
            {
                query = query.Where(b => queryParameters.Genres.Contains(b.GenreId));
            }

            if (queryParameters.Stars != null && queryParameters.Stars.Any())
            {
                query = query.Where(b => queryParameters.Stars.Contains((int)b.Rating));
            }

            switch (queryParameters.SortOrder)
            {
                case "popularity":
                    query = query.OrderByDescending(b => b.ReadCounter);
                    break;
                case "rating":
                    query = query.OrderByDescending(b => b.Rating);
                    break;
                case "title":
                    query = query.OrderBy(b => b.Title);
                    break;
                case "author":
                    query = query.OrderBy(b => b.Author);
                    break;
                default:
                    query = query.OrderByDescending(b => b.ReadCounter);
                    break;
            }

            var pagedQuery = query.Skip((queryParameters.Page - 1) * queryParameters.PageSize).Take(queryParameters.PageSize);

            int totalItems = query.Count();

            int totalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize);

            return new
            {
                Books = await pagedQuery.ToListAsync(),
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<IEnumerable<Book>> GetPopularBooksAsync()
        {
            return await _context.Books.OrderByDescending(q => q.ReadCounter).Take(5).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetTrendingBooksAsync()
        {
            return await _context.Books.OrderByDescending(q => q.Rating).Take(5).ToListAsync();
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task AddBookAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task<Book> UpdateBookAsync(int id, Book book)
        {
            var existingBook = await _context.Books.FindAsync(id);

            if (existingBook == null)
            {
                return null;
            }

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.GenreId = book.GenreId;
            existingBook.CoverImage = book.CoverImage;
            existingBook.Description = book.Description;
            existingBook.Rating = book.Rating;
            existingBook.ReadCounter = book.ReadCounter;
            existingBook.DownloadCount = book.DownloadCount;
            existingBook.PublicationDate = book.PublicationDate;
            existingBook.Year = book.Year;
            existingBook.Content = book.Content;

            await _context.SaveChangesAsync();

            return existingBook;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return false;
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

