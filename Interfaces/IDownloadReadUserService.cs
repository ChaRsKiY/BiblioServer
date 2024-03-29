using System;
namespace BiblioServer.Interfaces
{
	public interface IDownloadReadUserService
	{
        Task SetBookAsDownloaded(int userId, int bookId);
        Task SetBookAsRead(int userId, int bookId);
    }
}

