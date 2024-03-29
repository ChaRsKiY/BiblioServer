using System;
using BiblioServer.Models;

namespace BiblioServer.Interfaces
{
	public interface IAdminService
	{
        Task<string> AddAdminAsync(AdminAddDeleteModel model);
        Task<string> DeleteAdminAsync(AdminAddDeleteModel model);
        Task<GeneralStatisticModel> GetGeneralStatisticAsync();
    }
}

