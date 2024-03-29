using System;
using BiblioServer.Interfaces;

namespace BiblioServer.Services
{
	public class DownloadReadUserService : IDownloadReadUserService
	{
		private readonly IDownloadReadUserRepository _downloadReadUserRepository;
		private readonly IBookService _bookService;

		public DownloadReadUserService(IDownloadReadUserRepository downloadReadUserRepository, IBookService bookService)
		{
			_downloadReadUserRepository = downloadReadUserRepository;
			_bookService = bookService;
		}

		public async Task SetBookAsDownloaded(int userId, int bookId)
		{
			var book = await _bookService.GetBookByIdAsync(bookId);

			if (book == null)
				return;

			await _downloadReadUserRepository.SetBookAsDownloadedAsync(userId, bookId);

			var downloads = await _downloadReadUserRepository.GetDownloadsByBookAsync(bookId);
			await _bookService.UpdateDownloadsAsync(downloads, bookId);
		}

        public async Task SetBookAsRead(int userId, int bookId)
        {
            var book = await _bookService.GetBookByIdAsync(bookId);

            if (book == null)
                return;

            await _downloadReadUserRepository.SetBookAsReadAsync(userId, bookId);

            var reads = await _downloadReadUserRepository.GetReadsByBookAsync(bookId);
            await _bookService.UpdateReadsAsync(reads, bookId);
        }
    }
}

