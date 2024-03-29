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
using BiblioServer.Services;
using BiblioServer.Repositories;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;

namespace BiblioServer.Controllers;

//User Controller
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly ILoginService _loginService;
    private readonly IUserProfileService _userProfileService;
    private readonly IUserService _userService;
    private readonly IResetPasswordService _resetPasswordService;
    private readonly IEmailService _emailService;
    private readonly IChangeEmailService _changeEmailService;

    //Connecting/Getting services throw constructor 
    public UserController(IChangeEmailService changeEmailService, IEmailService emailService, IResetPasswordService resetPasswordService, IRegistrationService registrationService, ILoginService loginService, IUserProfileService userProfileService, IUserService userService)
    {
        _registrationService = registrationService ?? throw new ArgumentNullException(nameof(registrationService));
        _loginService = loginService;
        _userProfileService = userProfileService;
        _userService = userService;
        _resetPasswordService = resetPasswordService;
        _emailService = emailService;
        _changeEmailService = changeEmailService;
    }

    //Register Route
    [HttpPost("register")]
    public async Task<IActionResult> StartRegistration([FromBody] UserRegistrationModel user)
    {
        var token = await _registrationService.RegisterUserAsync(user);

        if (token == "emailExist")
        {
            return BadRequest(token);
        }
        else if (token == "usernameExist")
        {
            return BadRequest(token);
        }

        return Ok();
    }

    [HttpPost("verifyemail")]
    public async Task<IActionResult> CompleteRegistration([FromBody] VerificationModel model)
    {
        var user = await _userService.GetUserByEmail(model.Email);

        if (user == null)
        {
            return BadRequest("userExist");
        }

        if(user.IsVerified)
        {
            return BadRequest("alreadyVerified");
        }

        if(user.VerificationCode != model.VerificationCode)
        {
            return BadRequest("invalidVerificationCode");
        }

        await _registrationService.CompleteRegistration(user);

        return Ok();
    }

    [HttpPost("resendverificationcode")]
    public async Task<IActionResult> ResendVerificationCode([FromBody] ResentVerificationModel model)
    {
        var user = await _userService.GetUserByEmail(model.Email);

        if (user == null)
        {
            return BadRequest("userExist");
        }

        if (user.IsVerified == true)
        {
            return BadRequest("alreadyVerified");
        }

        await _registrationService.ResentVerifyCode(user);

        return Ok();
    }

    [HttpPost("password-reset-request")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestModel model)
    {
        var token = await _resetPasswordService.GeneratePasswordResetToken(model.Email);

        if (token != "userExist")
        {
            _emailService.SendResetPasswordEmail(model.Email, token);

            return Ok();
        }

        return BadRequest(token);
    }

    [HttpGet("email-change-request")]
    [Authorize]
    public async Task<IActionResult> StartChangingEmail()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _userService.GetUserById(int.Parse(userId));

        var token = await _changeEmailService.GenerateChangeEmailToken(int.Parse(userId));

        if (token != "userExist")
        {
            _emailService.SendEmailChangeCode(user.Email, token);

            return Ok();
        }

        return BadRequest(token);
    }

    [HttpPost("email-change")]
    [Authorize]
    public async Task<IActionResult> ChangingEmail([FromBody] ChangeEmailModel model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _userService.GetUserById(int.Parse(userId));

        if(user != null)
        {
            var token = await _changeEmailService.CheckTokenAsync(user, model);

            if(token == "invalidCode")
            {
                return BadRequest(token);
            }

            return Ok();
        }

        return BadRequest("userExist");
    }

    [HttpPost("email-change-confirm")]
    public async Task<IActionResult> ConfirmChangingEmail([FromBody] ChangeEmailModel model)
    {
        var token = await _changeEmailService.ConfirmChangingEmailAsync(model);

        if (token == "invalidCode")
        {
            return BadRequest(token);
        }

        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        var user = await _userService.GetUserByEmail(model.Email);

        if (user != null && user.PasswordResetTokenExpiration > DateTime.UtcNow && model.Token == user.PasswordResetToken)
        {
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, user.Salt);

            user.HashedPassword = newPasswordHash;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiration = null;

            await _userService.UpdateUser(user);

            return Ok();
        }

        return BadRequest();
    }

    //Login Route
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginModel loginModel)
    {
        var token = await _loginService.LoginUserAsync(loginModel);

        if (token == "invalidCredentials")
        {
            return Unauthorized(token);
        }

        if (token == "emailNotVerified")
        {
            return Unauthorized(token);
        }

        return Ok(new { Token = token });
    }

    [HttpPut("update-password")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordModel updateModel)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _userService.GetUserById(int.Parse(userId));

        if(user == null)
        {
            return BadRequest("userExist");
        }

        var result = await _userProfileService.UpdatePasswordAsync(int.Parse(userId), updateModel);

        if(result == "invalidOldPassword")
        {
            return BadRequest(result);
        }

        _emailService.SendSuccessfullResetPasswordEmail(user.Email);

        return Ok();
    }

    //Token Decode Route
    [HttpGet("decode")]
    [Authorize]
    public async Task<IActionResult> DecodeToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return BadRequest("invalidToken");
        }

        var user = await _userService.GetUserById(int.Parse(userId));

        if (user == null)
        {
            return BadRequest("userExist");
        }

        return Ok(new { UserId = userId, UserName = user.UserName, IsAdmin = user.IsAdmin, Avatar = user.Avatar });
    }

    //Get User Data Route
    [HttpGet("userdata")]
    [Authorize]
    public async Task<IActionResult> UserData()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return BadRequest("Invalid or missing token");
        }

        var userProfile = await _userProfileService.GetUserProfileAsync(int.Parse(userId));

        if (userProfile == null)
        {
            return NotFound("User not found");
        }

        return Ok(userProfile);
    }

    [HttpGet("userdata/{id}")]
    [Authorize(policy: "AdminAuthorize")]
    public async Task<IActionResult> UserDataAdmin([FromRoute] int id)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync(id);

        if (userProfile == null)
        {
            return NotFound("User not found");
        }

        return Ok(userProfile);
    }

    //Get Avatar Route
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

    //Update User Route
    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromForm] UserUpdateModel updateModel)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest("Invalid or missing token");
            }

            if (updateModel.AvatarFile != null)
            {
                var supportedTypes = new[] { "jpeg", "jpg", "png", "webp", "gif" };
                var fileExtension = Path.GetExtension(updateModel.AvatarFile.FileName).TrimStart('.');

                if (!supportedTypes.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest("InvalidFileType");
                }
            }

            var response = await _userProfileService.UpdateUserProfileAsync(int.Parse(userId), updateModel);

            if(response == "usernameExist" || response == "userExist")
            {
                return BadRequest(response);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("update/{id}")]
    [Authorize(policy: "AdminAuthorize")]
    public async Task<IActionResult> UpdateAdminProfile([FromRoute] int id, [FromForm] UserUpdateModel updateModel)
    {
        try
        {
            if (updateModel.AvatarFile != null)
            {
                var supportedTypes = new[] { "jpeg", "jpg", "png", "webp", "gif" };
                var fileExtension = Path.GetExtension(updateModel.AvatarFile.FileName).TrimStart('.');

                if (!supportedTypes.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest("InvalidFileType");
                }
            }

            var response = await _userProfileService.UpdateUserProfileAsync(id, updateModel);

            if (response == "usernameExist" || response == "userExist")
            {
                return BadRequest(response);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    //Delete User By Token Route 
    [HttpDelete("delete/{id}")]
    [Authorize(policy: "AdminAuthorize")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        try
        {
            await _userProfileService.DeleteUserAsync(id);

            return Ok("User deleted successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("{page}")]
    [Authorize(policy: "AdminAuthorize")]
    public async Task<IActionResult> GetAllUsers([FromRoute] int page)
    {
        var data = await _userService.GetAllUsersAsync(page);

        if (data == null)
            return BadRequest();

        return Ok(data);
    }

    [HttpGet("admins/{page}")]
    [Authorize(policy: "AdminAuthorize")]
    public async Task<IActionResult> GetAllAdmins([FromRoute] int page)
    {
        var data = await _userService.GetAllAdminsAsync(page);

        if (data == null)
            return BadRequest();

        return Ok(data);
    }

    //Function for get avatar route
    //Returns content type of image
    private string GetContentType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
}