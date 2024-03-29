using System;
using Microsoft.AspNetCore.Mvc;
using BiblioServer.Models;
using BiblioServer.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BiblioServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BookmarksController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;

        public BookmarksController(IBookmarkService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }

        // GET: api/bookmarks
        [HttpGet("{bookId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Bookmark>>> GetBookmarks([FromRoute] int bookId)
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(id != null)
            {
                var bookmarks = await _bookmarkService.GetBookmarksAsync(id, bookId);
                return Ok(bookmarks);
            }

            return BadRequest();
        }

        // POST: api/bookmarks
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Bookmark>> CreateBookmark(CreateBookmark bookmark)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId != null)
            {
                var newBookmark = new Bookmark
                {
                    UserId = userId,
                    BookId = bookmark.BookId,
                    Color = bookmark.Color,
                    PageNumber = bookmark.PageNumber
                };

                var createdBookmark = await _bookmarkService.CreateBookmarkAsync(newBookmark);

                return Ok(createdBookmark);
            }

            return BadRequest();
        }

        // DELETE: api/bookmarks/5
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteBookmark([FromQuery] int bookId, [FromQuery] int page)
        {
            await _bookmarkService.DeleteBookmarkAsync(bookId, page);
            return NoContent();
        }
    }

}

