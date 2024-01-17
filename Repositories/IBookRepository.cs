using System;
using BiblioServer.Models;

namespace BiblioServer.Repositories
{
    public interface IBookRepository
    {
        Task<object> GetBooksAsync(BookQueryParameters queryParameters);
        Task<Book> GetBookByIdAsync(int id);
        Task AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(int id, Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<IEnumerable<Book>> GetTrendingBooksAsync();
        Task<IEnumerable<Book>> GetPopularBooksAsync();
    }
}

