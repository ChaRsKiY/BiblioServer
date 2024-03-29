using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Interfaces;
using BiblioServer.Models;
using BiblioServer.Repositories;

namespace BiblioServer.Services
{
    public class RatingService : IRatingService
    {
        private readonly IBookService _bookService;
        private readonly IRatingRepository _ratingRepository;

        public RatingService(IRatingRepository ratingRepository, IBookService bookService)
        {
            _ratingRepository = ratingRepository;
            _bookService = bookService;
        }

        public async Task AddRatingAsync(Rating model)
        {
            var rated = await _ratingRepository.GetRatingByModel(model);

            if (rated != null)
            {
                await _ratingRepository.UpdateRatingAsync(model);
            }
            else
            {
                await _ratingRepository.AddRatingAsync(model);
            }

            var count = await _ratingRepository.GetAllRatingForBookAsync(model.IdBook);
            await _bookService.UpdateRatingAsync(count, model.IdBook);
        }
    }
}

