using System;
using BiblioServer.Interfaces;
using BiblioServer.Models;

namespace BiblioServer.Services
{
	public class AdminService : IAdminService
	{
		private readonly IUserService _userService;
        private readonly IBookService _bookService;
        private readonly ICommentService _commentService;

        public AdminService(IUserService userService, IBookService bookService, ICommentService commentService)
		{
			_userService = userService;
            _bookService = bookService;
            _commentService = commentService;
		}

		public async Task<string> AddAdminAsync(AdminAddDeleteModel model)
		{
			var user = await _userService.GetUserById(model.Id);

			if(user == null)
			{
				return "userExist";
			}

            if (user.IsVerified == false)
            {
                return "notVerified";
            }

            if (user.IsAdmin == true)
            {
                return "alreadyAdmin";
            }

			user.IsAdmin = true;

			await _userService.UpdateUser(user);

            return "";
		}

        public async Task<string> DeleteAdminAsync(AdminAddDeleteModel model)
        {
            var user = await _userService.GetUserById(model.Id);

            if (user == null)
            {
                return "userExist";
            }

            if (user.IsVerified == false)
            {
                return "notVerified";
            }

            if (user.IsAdmin == false)
            {
                return "isnotAdmin";
            }

            user.IsAdmin = false;

            await _userService.UpdateUser(user);

            return "";
        }

        public async Task<GeneralStatisticModel> GetGeneralStatisticAsync()
        {
            var count = await _bookService.GetBooksCountAsync();
            var comments = await _commentService.GetCommentsCountAsync();
            var downloads = await _bookService.GetDownloadsCountAsync();

            var model = new GeneralStatisticModel {
                TotalBooks = count,
                TotalComments = comments,
                TotalDownloads = downloads
            };

            return model;
        }
    }
}

