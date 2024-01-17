using System;
using BiblioServer.Models;

namespace BiblioServer.Services
{
    public interface IBookService
    {
        Task<object> GetBooksAsync(BookQueryParameters queryParameters);
        Task<Book> GetBookByIdAsync(int id);
        Task AddBookAsync(int userId, AddBookModel book);
        Task<Book> UpdateBookAsync(int id, Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<IEnumerable<Book>> GetTrendingBooksAsync();
        Task<IEnumerable<Book>> GetPopularBooksAsync();
    }
}

