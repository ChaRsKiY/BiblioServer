using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Interfaces;
using BiblioServer.Models;
using BiblioServer.Repositories;

namespace BiblioServer.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<CommentsWithPagination> GetCommentsByBookId(int bookId, int page)
        {
            const int perPage = 10;
            return await _commentRepository.GetCommentsByBookId(bookId, page, perPage);
        }

        public async Task<object> GetCommentsAsync(int page)
        {
            return await _commentRepository.GetCommentsAsync(page, 8);
        }

        public async Task<int> GetCommentsCountAsync()
        {
            return await _commentRepository.GetCommentsCountAsync();
        }
        
        public async Task CreateCommentAsync(Comment model)
        {
            await _commentRepository.CreateCommentAsync(model);
        }

        public async Task UpdateCommentAsync(Comment model)
        {
            await _commentRepository.UpdateCommentAsync(model);
        }

        public async Task<object> GetAllComments(int page)
        {
            const int perPage = 8;
            return await _commentRepository.GetCommentsAsync(page, perPage);
        }

        public async Task DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _commentRepository.GetCommentAsync(commentId);

            if (comment != null && comment.IdUser == Int32.Parse(userId))
            {
                await _commentRepository.DeleteCommentAsync(comment);
            }
            else
            {
                throw new Exception("Comment not found or user is not authorized to delete the comment.");
            }
        }

        public async Task DeleteCommentByIdAsync(int commentId)
        {
            var comment = await _commentRepository.GetCommentAsync(commentId);

            if (comment != null)
            {
                await _commentRepository.DeleteCommentAsync(comment);
            }
            else
            {
                throw new Exception("Comment not found.");
            }
        }

        public async Task DeleteAllCommentsInBook(int bookId)
        {
            await _commentRepository.DeleteAllCommentsInBook(bookId);
        }

    }
}

