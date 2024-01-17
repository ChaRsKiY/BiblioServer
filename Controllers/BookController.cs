using System.Security.Claims;
using System.Text;
using BiblioServer.Models;
using BiblioServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetBooks([FromQuery] BookQueryParameters queryParameters)
    {
        var res = await _bookService.GetBooksAsync(queryParameters);
        return Ok(res);
    }

    [HttpGet("popular")]
    public async Task<ActionResult<IEnumerable<Book>>> GetPopularBooks()
    {
        var res = await _bookService.GetPopularBooksAsync();
        return Ok(res);
    }

    [HttpGet("trending")]
    public async Task<ActionResult<IEnumerable<Book>>> GetTrendingBooks()
    {
        var res = await _bookService.GetTrendingBooksAsync();
        return Ok(res);
    }

    [HttpGet("bookcover/{fileName}")]
    public IActionResult GetBook(string fileName)
    {
        var filePath = Path.Combine("wwwroot/covers", fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File not found");
        }

        var fileStream = new FileStream(filePath, FileMode.Open);
        return File(fileStream, GetContentType(fileName));
    }

    private string GetContentType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> AddBook([FromForm] AddBookModel createModel)
    {
        try
        {
            var userId = 0;//User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest("Invalid or missing token");
            }

            if (createModel.Image != null)
            {
                var supportedTypes = new[] { "jpeg", "jpg", "png", "webp" };
                var fileExtension = Path.GetExtension(createModel.Image.FileName).TrimStart('.');

                if (!supportedTypes.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest("InvalidFileType");
                }
            }

            await _bookService.AddBookAsync(userId, createModel);

            //if (response == "usernameExist" || response == "userExist")
            //{
            //    return BadRequest(response);
            //}

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
    {
        var updatedBook = await _bookService.UpdateBookAsync(id, book);

        if (updatedBook == null)
        {
            return NotFound();
        }

        return Ok(updatedBook);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var result = await _bookService.DeleteBookAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("get-content/{fileName}")]
    public ActionResult<object> GetBookContent([FromRoute] string fileName, [FromQuery] int page = 1, [FromQuery] int pageSize = 1000)
    {
        var filePath = Path.Combine("wwwroot/texts", fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var content = ReadFileContent(filePath, page, pageSize);

        return new
        {
            Content = content,
            TotalPages = CalculateTotalPages(filePath, pageSize),
            CurrentPage = page
        };
    }

    private string ReadFileContent(string filePath, int page, int pageSize)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            var fileContent = streamReader.ReadToEnd();

            var words = fileContent.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var startIndex = (page - 1) * pageSize;

            var wordsOnPage = words.Skip(startIndex).Take(pageSize);

            var result = string.Join(" ", wordsOnPage);

            return result;
        }
    }

    private int CalculateTotalPages(string filePath, int pageSize)
    {
        var fileSize = new FileInfo(filePath).Length;
        return (int)Math.Ceiling((double)fileSize / pageSize);
    }
}
