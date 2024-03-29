using System;
namespace BiblioServer.Interfaces
{
	public interface IDownloadReadUserRepository
	{
        Task SetBookAsDownloadedAsync(int userId, int bookId);
        Task SetBookAsReadAsync(int userId, int bookId);
        Task<int> GetDownloadsByBookAsync(int bookId);
        Task<int> GetReadsByBookAsync(int bookId);

    }
}

