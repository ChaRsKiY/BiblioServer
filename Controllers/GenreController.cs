using Microsoft.AspNetCore.Mvc;
using BiblioServer.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Models;

[ApiController]
[Route("[controller]")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenre([FromBody] Genre model)
    {
        try
        {
            await _genreService.CreateGenreAsync(model);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGenres()
    {
        var genres = await _genreService.GetGenresAsync();
        return Ok(genres);
    }
}
