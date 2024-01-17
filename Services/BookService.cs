
using System;
using BiblioServer.Models;
using BiblioServer.Repositories;

namespace BiblioServer.Services
{
	public class BookService : IBookService
	{
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<object> GetBooksAsync(BookQueryParameters queryParameters)
        {
            return await _bookRepository.GetBooksAsync(queryParameters);
        }

        public async Task<IEnumerable<Book>> GetPopularBooksAsync()
        {
            return await _bookRepository.GetPopularBooksAsync();
        }

        public async Task<IEnumerable<Book>> GetTrendingBooksAsync()
        {
            return await _bookRepository.GetTrendingBooksAsync();
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetBookByIdAsync(id);
        }

        public async Task AddBookAsync(int userId, AddBookModel book)
        {
            var coverFileName = Guid.NewGuid().ToString() + Path.GetExtension(book.Image.FileName);
            var coverFilePath = Path.Combine("wwwroot/covers", coverFileName);

            using (var stream = new FileStream(coverFilePath, FileMode.Create))
            {
                await book.Image.CopyToAsync(stream);
            }

            var contentFileName = Guid.NewGuid().ToString() + Path.GetExtension(book.Content.FileName);
            var contentFilePath = Path.Combine("wwwroot/texts", contentFileName);

            using (var stream = new FileStream(contentFilePath, FileMode.Create))
            {
                await book.Content.CopyToAsync(stream);
            }

            var newBook = new Book
            {
                Author = book.Author,
                Title = book.Title,
                Year = book.Year,
                GenreId = book.GenreId,
                Description = book.Description,
                CoverImage = coverFileName,
                PublicationDate = DateTime.Now,
                Content = coverFilePath
            };

            await _bookRepository.AddBookAsync(newBook);
        }

        public async Task<Book> UpdateBookAsync(int id, Book book)
        {
            return await _bookRepository.UpdateBookAsync(id, book);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            return await _bookRepository.DeleteBookAsync(id);
        }
    }
}

