using System;
using BiblioServer.Models;

namespace BiblioServer.Repositories
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetGenresAsync();
        Task CreateGenreAsync(Genre model);
        Task DeleteGenreAsync(int id);
    }
}

