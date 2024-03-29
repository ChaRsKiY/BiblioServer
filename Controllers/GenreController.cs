using Microsoft.AspNetCore.Mvc;
using BiblioServer.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Models;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize(policy: "AdminAuthorize")]
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

    [HttpDelete("{id}")]
    [Authorize(policy: "AdminAuthorize")]
    public async Task<IActionResult> DeleteGenre([FromRoute] int id)
    {
        try
        {
            await _genreService.DeleteGenreAsync(id);
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
