using System;
using BiblioServer.Interfaces;
using BiblioServer.Models;

namespace BiblioServer.Services
{
	public class ActivityService : IActivityService
	{
        IActivityRepository _activityRepository;

        public ActivityService(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public void AddActivity(ActivityModel model)
        {
            _activityRepository.AddActivity(model);
        }

        public async Task AddActivityAsync(ActivityModel model)
		{
            await _activityRepository.AddActivityAsync(model);
        }

        public async Task<object> GetActivitiesAsync(int page)
        {
            int totalPages = 8;
            return await _activityRepository.GetActivitiesAsync(page, totalPages);
        }
    }
}

