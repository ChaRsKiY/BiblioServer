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
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateComment([FromBody] Comment model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        try
        {
            if(userId != null)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                model.IdUser = Int32.Parse(userId);
              
                await _commentService.CreateCommentAsync(model);
                return Ok();
            }

            return BadRequest("Missing userId");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateComment([FromBody] Comment model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        try
        {
            if (userId != null)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (model.IdUser != Int32.Parse(userId))
                {
                    return Unauthorized();
                }

                await _commentService.UpdateCommentAsync(model);
                return Ok();
            }

            return BadRequest("Missing userId");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("delete/{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        try
        {
            if (commentId <= 0 || userId == null) 
            {
                return BadRequest("Invalid commentId");
            }

            await _commentService.DeleteCommentAsync(commentId, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("delete-admin/{commentId}")]
    [Authorize(policy: "AdminAuthorize")]
    public async Task<IActionResult> DeleteAdminComment(int commentId)
    {
        try
        {
            if (commentId <= 0)
            {
                return BadRequest("Invalid commentId");
            }

            await _commentService.DeleteCommentByIdAsync(commentId);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<CommentsWithPagination>>> GetCommentsByBookId([FromQuery] int bookId, [FromQuery] int page)
    {
        var comments = await _commentService.GetCommentsByBookId(bookId, page);

        if (comments == null)
        {
            return NotFound();
        }

        return Ok(comments);
    }

    [HttpGet("all-comments/{page}")]
    [Authorize(policy: "AdminAuthorize")]
    public async Task<ActionResult<object>> GetAllComments([FromRoute] int page)
    {
        var comments = await _commentService.GetAllComments(page);

        return Ok(comments);
    }
}