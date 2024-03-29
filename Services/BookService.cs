
using System;
using BiblioServer.Interfaces;
using BiblioServer.Models;
using BiblioServer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BiblioServer.Services
{
	public class BookService : IBookService
	{
        private readonly IBookRepository _bookRepository;
        private readonly IUserService _userService;
        private readonly IActivityService _activityService;
        private readonly ICommentService _commentService;

        public BookService(IBookRepository bookRepository, IUserService userService, IActivityService activityService, ICommentService commentService)
        {
            _bookRepository = bookRepository;
            _userService = userService;
            _activityService = activityService;
            _commentService = commentService;
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

        public async Task<object> GetBooksByUserId(int userId, int page)
        {
            int pageSize = 5;
            return await _bookRepository.GetBooksByUserId(userId, page, pageSize);
        }

        public async Task<int> GetBooksCountAsync()
        {
            return await _bookRepository.GetBooksCountAsync();
        }

        public async Task<string> AddBookAsync(int userId, AddBookModel book)
        {
            var user = await _userService.GetUserById(userId);

            if(user == null)
            {
                return "userExist";
            }

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
                UserId = user.Id,
                UserName = user.UserName,
                Author = book.Author,
                Title = book.Title,
                Year = new DateTime((int)book.Year, 1, 1),
                GenreId = book.GenreId,
                Description = book.Description,
                CoverImage = coverFileName,
                PublicationDate = DateTime.Now,
                Content = contentFileName
            };

            await _bookRepository.AddBookAsync(newBook);

            var activityModel = new ActivityModel
            {
                Email = user.Email,
                Name = user.UserName,
                Time = DateTime.Now,
                Status = "Created a book"
            };

            _activityService.AddActivity(activityModel);

            return "";
        }

        public async Task<Book> UpdateBookAsync(int id, Book book)
        {
            return await _bookRepository.UpdateBookAsync(id, book);
        }

        public async Task<int> GetDownloadsCountAsync()
        {
            return await _bookRepository.GetDownloadsCountAsync();
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            await _commentService.DeleteAllCommentsInBook(id);
            return await _bookRepository.DeleteBookAsync(id);
        }

        public async Task UpdateRatingAsync(int newRating, int bookId)
        {
            var book = await _bookRepository.GetBookByIdAsync(bookId);
          
            book.Rating = newRating;

            await _bookRepository.UpdateBookAsync(bookId, book);
        }

        public async Task UpdateDownloadsAsync(int newDownloads, int bookId)
        {
            var book = await _bookRepository.GetBookByIdAsync(bookId);
           
            book.DownloadCount = newDownloads;

            await _bookRepository.UpdateBookAsync(bookId, book);
        }

        public async Task UpdateReadsAsync(int newReads, int bookId)
        {
            var book = await _bookRepository.GetBookByIdAsync(bookId);

            book.ReadCounter = newReads;

            await _bookRepository.UpdateBookAsync(bookId, book);
        }
    }
}

