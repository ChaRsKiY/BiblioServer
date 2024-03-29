using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BiblioServer.Models;
using BiblioServer.Services;
using BiblioServer.Interfaces;

namespace BiblioServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IEmailService _emailService;
        private readonly IActivityService _activityService;

        public AdminController(IAdminService adminService, IActivityService activityService, IEmailService emailService)
        {
            _adminService = adminService;
            _activityService = activityService;
            _emailService = emailService;
        }

        [HttpPost("add")]
        [Authorize(policy: "AdminAuthorize")]
        public async Task<IActionResult> AddAdmin([FromBody] AdminAddDeleteModel model)
        {
            var result = await _adminService.AddAdminAsync(model);

            if (result.Length != 0)
            {
                return BadRequest(result);
            }

            return Ok("Successfully added");
        }

        [HttpPost("send-email")]
        [Authorize(policy: "AdminAuthorize")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailModel model)
        {
            try
            {
                await _emailService.SendCustomEmailAsync(model);
                return Ok("Successfully sended");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }    
        }

        [HttpPost("delete")]
        [Authorize(policy: "AdminAuthorize")]
        public async Task<IActionResult> UserOnly([FromBody] AdminAddDeleteModel model)
        {
            var result = await _adminService.DeleteAdminAsync(model);

            if (result.Length != 0)
            {
                return BadRequest(result);
            }

            return Ok("User-only content");
        }

        [HttpGet("dashboard")]
        [Authorize(policy: "AdminAuthorize")]
        public async Task<IActionResult> Dashboard()
        {
            var statistic = await _adminService.GetGeneralStatisticAsync();

            return Ok(statistic);
        }

        [HttpGet("last-activity/{id}")]
        [Authorize(policy: "AdminAuthorize")]
        public async Task<IActionResult> LastActivity([FromRoute] int id)
        {
            var lastActivities = await _activityService.GetActivitiesAsync(id);

            return Ok(lastActivities);
        }
    }
}

