using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Models;

namespace BiblioServer.Interfaces
{
    public interface IRatingService
    {
        Task AddRatingAsync(Rating model);
    }
}

