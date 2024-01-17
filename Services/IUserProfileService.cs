using System;
using BiblioServer.Models;

namespace BiblioServer.Services
{
    public interface IUserProfileService
    {
        Task<UserProfileModel> GetUserProfileAsync(int userId);
        Task<string> UpdateUserProfileAsync(int userId, UserUpdateModel updateModel);
        Task DeleteUserAsync(int userId);
        Task<string> UpdatePasswordAsync(int userId, UpdatePasswordModel model);
    }
}

