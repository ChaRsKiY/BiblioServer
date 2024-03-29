using System;
using BiblioServer.Models;
using BiblioServer.Repositories;

namespace BiblioServer.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public UserProfileService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        //Gets user info
        public async Task<UserProfileModel> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            //Mapping user to profileusermodel
            var userProfile = new UserProfileModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Bio = user.Bio,
                Avatar = user.Avatar,
                RegistrationDate = user.RegistrationDate,
                IsAdmin = user.IsAdmin
            };

            return userProfile;
        }

        //Updates user data
        public async Task<string> UpdateUserProfileAsync(int userId, UserUpdateModel updateModel)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user != null)
            {
                if (!string.IsNullOrEmpty(updateModel.UserName))
                {
                    if(user.UserName.Trim() != updateModel.UserName)
                    {
                        if (await _userRepository.IsUsernameExistsAsync(updateModel.UserName))
                        {
                            return "usernameExist";
                        }

                        user.UserName = updateModel.UserName;
                    }
                }

                if (!string.IsNullOrEmpty(updateModel.Name))
                {
                    user.Name = updateModel.Name;
                }

                if (!string.IsNullOrEmpty(updateModel.Surname))
                {
                    user.Surname = updateModel.Surname;
                }

                if (!string.IsNullOrEmpty(updateModel.Bio))
                {
                    user.Bio = updateModel.Bio;
                }

                if (updateModel.AvatarFile != null)
                {
                    // Delete the previous avatar if it exists
                    if (user.Avatar != "standart.jpg")
                    {
                        var previousAvatarPath = Path.Combine("wwwroot/avatars", user.Avatar);
                        if (System.IO.File.Exists(previousAvatarPath))
                        {
                            System.IO.File.Delete(previousAvatarPath);
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateModel.AvatarFile.FileName);
                    var filePath = Path.Combine("wwwroot/avatars", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await updateModel.AvatarFile.CopyToAsync(stream);
                    }

                    user.Avatar = fileName;
                }

                await _userRepository.UpdateUserAsync(user);

                return "";

            } else
            {
                return "userExist";
            }
        }

        //Deletes user
        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user.Avatar != "standart.jpg")
            {
                var previousAvatarPath = Path.Combine("wwwroot/avatars", user.Avatar);
                if (System.IO.File.Exists(previousAvatarPath))
                {
                    System.IO.File.Delete(previousAvatarPath);
                }
            }

            if (user != null)
            {
                await _userRepository.DeleteUserAsync(user);
            }
        }

        public async Task<string> UpdatePasswordAsync(int userId, UpdatePasswordModel model)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (!VerifyPassword(model.OldPassword, user.HashedPassword))
            {
                return "invalidOldPassword";
            }

            user.HashedPassword = HashPassword(model.NewPassword, user.Salt);

            await _userRepository.UpdateUserAsync(user);

            return "";
        }

        private string HashPassword(string password, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        private bool VerifyPassword(string enteredPassword, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, passwordHash);
        }
    }
}

