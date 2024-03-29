using Microsoft.AspNetCore.Mvc;
using BiblioServer.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using BiblioServer.Models;
using System;
using BiblioServer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("[controller]")]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddRating([FromBody] Rating model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        try
        {
            if (userId != null)
            {
                model.IdUser = int.Parse(userId);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _ratingService.AddRatingAsync(model);

                return Ok();
            }

            return BadRequest("Missing userId");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

   
}