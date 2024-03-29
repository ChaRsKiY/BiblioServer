using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Models;

namespace BiblioServer.Interfaces
{
    public interface ICommentRepository
    {
        Task<CommentsWithPagination> GetCommentsByBookId(int bookId, int page, int perPage);
        Task<object> GetCommentsAsync(int page, int perPage);
        Task CreateCommentAsync(Comment model);
        Task UpdateCommentAsync(Comment model);
        Task DeleteCommentAsync(Comment model);
        Task<Comment> GetCommentAsync(int commentId);
        Task<int> GetCommentsCountAsync();
        Task DeleteAllCommentsInBook(int bookId);
    }
}