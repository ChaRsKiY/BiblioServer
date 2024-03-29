using System;
using BiblioServer.Models;

namespace BiblioServer.Interfaces
{
	public interface IActivityRepository
	{
        Task AddActivityAsync(ActivityModel model);
        void AddActivity(ActivityModel model);
        Task<object> GetActivitiesAsync(int page, int pageSize);
    }
}

