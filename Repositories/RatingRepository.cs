using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Context;
using BiblioServer.Interfaces;
using BiblioServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblioServer.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRatingAsync(Rating model)
        {
            await _context.Rating.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetAllRatingForBookAsync(int bookId)
        {
            return await _context.Rating
            .Where(r => r.IdBook == bookId)
            .SumAsync(r => r.Stars);
        }

        public async Task<Rating> GetRatingByModel(Rating model)
        {
            return await _context.Rating
                .FirstOrDefaultAsync(r =>
                    r.IdBook == model.IdBook &&
                    r.IdUser == model.IdUser);
        }

        public async Task UpdateRatingAsync(Rating model)
        {
            var existingRating = await _context.Rating.FirstOrDefaultAsync(r => r.IdUser == model.IdUser && r.IdBook == model.IdBook);

            if (existingRating != null)
            {
                existingRating.Stars = model.Stars;
                existingRating.IdBook = model.IdBook;
                existingRating.IdUser = model.IdUser;

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Rating not found.");
            }
        }
    }
}