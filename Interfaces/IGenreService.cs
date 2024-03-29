using System;
using BiblioServer.Models;

namespace BiblioServer.Services
{

    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetGenresAsync();
        Task CreateGenreAsync(Genre model);
        Task DeleteGenreAsync(int id);
    }
}

