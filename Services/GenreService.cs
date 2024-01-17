using System;
using BiblioServer.Models;
using BiblioServer.Repositories;

namespace BiblioServer.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<IEnumerable<Genre>> GetGenresAsync()
        {
            return await _genreRepository.GetGenresAsync();
        }

        public async Task CreateGenreAsync(Genre model)
        {
            await _genreRepository.CreateGenreAsync(model);
        }
    }
}

