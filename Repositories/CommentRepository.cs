using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Context;
using BiblioServer.Interfaces;
using BiblioServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblioServer.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommentsWithPagination> GetCommentsByBookId(int bookId, int page, int perPage)
        {
            var totalComments = await _context.Comments
                .Where(c => c.IdBook == bookId)
                .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalComments / perPage);

            var commentsWithUserInfo = await _context.Comments
                .Where(c => c.IdBook == bookId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Join(_context.Users,
                      comment => comment.IdUser,
                      user => user.Id,
                      (comment, user) => new CommentWithUsername
                      {
                          CommentId = comment.Id,
                          Content = comment.Content,
                          IdUser = (int)comment.IdUser,
                          Username = user.UserName,
                          Avatar = user.Avatar
                      })
                .ToListAsync();

            var result = new CommentsWithPagination
            {
                Comments = commentsWithUserInfo,
                TotalPages = totalPages
            };

            return result;
        }

        public async Task<object> GetCommentsAsync(int page, int perPage)
        {
            try
            {
                var totalComments = await _context.Comments.CountAsync();
                if(totalComments == 0)
                {
                    return new { Comments = new List<Comment>() { }, TotalPages = 1 };
                }


                var totalPages = (int)Math.Ceiling((double)totalComments / perPage);


                if (page < 1)
                    page = 1;
                if (page > totalPages)
                    page = totalPages;

                var comments = await _context.Comments
                                              .OrderByDescending(c => c.CreatedAt)
                                              .Skip((page - 1) * perPage)
                                              .Take(perPage)
                                              .ToListAsync();

                return new { Comments = comments, TotalPages = totalPages };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public async Task<int> GetCommentsCountAsync()
        {
            return await _context.Comments.CountAsync();
        }

        public async Task CreateCommentAsync(Comment model)
        {
            await _context.Comments.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(Comment model)
        {
            _context.Comments.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(Comment model)
        {
            _context.Comments.Remove(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllCommentsInBook(int bookId)
        {
            var commentsToDelete = _context.Comments.Where(c => c.IdBook == bookId);
            _context.Comments.RemoveRange(commentsToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment> GetCommentAsync(int commentId)
        {
            return await _context.Comments.FindAsync(commentId);
        }
    }
}