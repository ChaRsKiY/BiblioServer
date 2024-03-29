using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Models;

namespace BiblioServer.Interfaces
{
    public interface IRatingRepository
    {
        Task AddRatingAsync(Rating model);
        Task<Rating> GetRatingByModel(Rating model);
        Task UpdateRatingAsync(Rating model);
        Task<int> GetAllRatingForBookAsync(int bookId);
    }
}

