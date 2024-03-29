using System;
using BiblioServer.Models;

namespace BiblioServer.Services
{
    public interface IBookService
    {
        Task<object> GetBooksAsync(BookQueryParameters queryParameters);
        Task<Book> GetBookByIdAsync(int id);
        Task<string> AddBookAsync(int userId, AddBookModel book);
        Task<Book> UpdateBookAsync(int id, Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<IEnumerable<Book>> GetTrendingBooksAsync();
        Task<IEnumerable<Book>> GetPopularBooksAsync();
        Task UpdateRatingAsync(int newRating, int bookId);
        Task<int> GetBooksCountAsync();
        Task<int> GetDownloadsCountAsync();
        Task UpdateDownloadsAsync(int newDownloads, int bookId);
        Task UpdateReadsAsync(int newReads, int bookId);
        Task<object> GetBooksByUserId(int userId, int page);
    }
}

