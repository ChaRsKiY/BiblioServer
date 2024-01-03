using System.Collections.ObjectModel;
using BiblioServer.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using BiblioServer.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;

namespace BiblioServer.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationModel user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _context.Users.AnyAsync(x => x.Email == user.Email))
        {
            return BadRequest("emailExist");
        }

        if (await _context.Users.AnyAsync(x => x.UserName == user.UserName))
        {
            return BadRequest("usernameExist");
        }

        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);

        var newUser = new User
        {
            UserName = user.UserName,
            Email = user.Email,
            Salt = salt,
            RegistrationDate = DateTime.Now,
            HashedPassword = HashPassword(user.Password, salt),
        };

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(newUser);

        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginModel loginModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == loginModel.Email);

        if (user == null || !VerifyPassword(loginModel.Password, user.HashedPassword))
        {
            return Unauthorized("�������� ������� ������");
        }

        var token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

    [HttpGet("decode")]
    [Authorize] 
    public IActionResult DecodeToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        Console.WriteLine(userId);

        if (userId == null || userName == null)
        {
            return BadRequest("Invalid or missing token");
        }

        return Ok(new { UserId = userId, UserName = userName });
    }

    [HttpGet("userdata")]
    [Authorize]
    public async Task<IActionResult> UserData()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return BadRequest("Invalid or missing token");
        }

        var user = await _context.Users
        .Where(x => x.Id.ToString() == userId)
        .Select(x => new ProfileUserModel
        {
            UserName = x.UserName,
            Email = x.Email,
            Name = x.Name,
            Surname = x.Surname,
            Bio = x.Bio,
            Avatar = x.Avatar,
            RegistrationDate = x.RegistrationDate
        })
        .SingleOrDefaultAsync();

        if (user == null)
        {
            return NotFound("User not found");
        }

        return Ok(user);
    }

    [HttpGet("avatar/{fileName}")]
    public IActionResult GetAvatar(string fileName)
    {
        var filePath = Path.Combine("wwwroot/avatars", fileName);

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

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromForm] ProfileUserModel updateModel)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest("Invalid or missing token");
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!string.IsNullOrEmpty(updateModel.UserName))
            {
                user.UserName = updateModel.UserName;
            }

            if (!string.IsNullOrEmpty(updateModel.Name))
            {
                user.Name = updateModel.Name;
            }

            if (!string.IsNullOrEmpty(updateModel.Surname))
            {
                user.Surname = updateModel.Surname;
            }

            if (!string.IsNullOrEmpty(updateModel.Bio))
            {
                user.Bio = updateModel.Bio;
            }

            if (updateModel.AvatarFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateModel.AvatarFile.FileName);
                var filePath = Path.Combine("wwwroot/avatars", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateModel.AvatarFile.CopyToAsync(stream);
                }

                user.Avatar = fileName; 
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("delete")]
    [Authorize]
    public async Task<IActionResult> DeleteUser()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest("Invalid or missing token");
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
            {
                return NotFound("User not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    private string HashPassword(string password, string salt)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    private bool VerifyPassword(string EnteredPassword, string PasswordHash)
    {
        return BCrypt.Net.BCrypt.Verify(EnteredPassword, PasswordHash);
    }

    private string GenerateJwtToken(User user)
    {
        var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("My_Security_KeyMy_Security_KeyMy_Security_KeyMy_Security_KeyMy_Security_KeyMy_Security_KeyMy_Security_KeyMy_Security_KeyMy_Security_Key"));
        var Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
        };

        var token = new JwtSecurityToken(
            issuer: "",
            audience: "",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(72),
            signingCredentials: Credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}