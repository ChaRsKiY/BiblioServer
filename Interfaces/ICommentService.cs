using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Models;

namespace BiblioServer.Interfaces
{
        public interface ICommentService
        {
            Task<CommentsWithPagination> GetCommentsByBookId(int bookId, int page);
            Task<object> GetCommentsAsync(int page);
            Task CreateCommentAsync(Comment model);
            Task UpdateCommentAsync(Comment model);
            Task DeleteCommentAsync(int commentId, string userId);
            Task<int> GetCommentsCountAsync();
            Task<object> GetAllComments(int page);
            Task DeleteCommentByIdAsync(int commentId);
            Task DeleteAllCommentsInBook(int bookId);
        }
}

