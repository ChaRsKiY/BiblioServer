using System;
using BiblioServer.Models;

namespace BiblioServer.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(User user, string verificationCode, string callbackUrl);
        Task SendResetPasswordEmail(string toEmail, string token);
        Task SendEmailChangeCode(string toEmail, string code);
        Task SendChangeEmailVerificationEmail(string toEmail, string username, string verificationCode, string callbackUrl);
        Task SendSuccessfullResetPasswordEmail(string toEmail);
        Task SendCustomEmailAsync(SendEmailModel model);
    }
}

