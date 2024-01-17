using System;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Text.Encodings.Web;
using BiblioServer.Models;
using System.Drawing;

namespace BiblioServer.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendSuccessfullResetPasswordEmail(string toEmail)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Biblio", "biblio.read.ebooks@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = "Password Changed";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Arial', sans-serif;
                                background-color: #f4f4f4;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                background-color: #fff;
                                border-radius: 5px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                align-items: center;
                            }}
                            h1 {{
                                color: #E7734F;
                                margin-left: 30px;
                            }}
                            a {{
                                color: white;
                            }}
                            .block {{
                                background-image: url(https://media.npr.org/assets/img/2022/12/22/gettyimages-1245203807-1536x1029_wide-9982607ca51f99999656d993bf5511d42533c0f2-s1100-c50.jpg);
                                height: 130px;
                                background-size: cover;
                                width: 100%;
                                display: flex;
                                justify-content: right;
                                margin-right: 10px;
                            }}
                            .title {{
                                padding: 10px;
                                color: white;
                                font-size: 2.5rem;
                                font-weight: 600;
                            }}
                            .button {{
                                text-decoration: none;
                                color: white;
                                padding: 7px 20px;
                                background-color: #E7734F;
                                margin-left: 30px;
                            }}
                            .textbefore {{
                                padding: 20px 20px 10px 20px;

                            }}
                            .footer {{
                                margin-top: 35px;
                                width: 100%;
                                background-color: #dcdcdc;
                                padding: 15px 8px;
                                display: flex;
                                justify-content: space-between;
                            }}
                            .footer a {{
                                text-decoration: none;
                                color: black;
                            }}
                            .footcont {{
                                padding-right: 40px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class=""block"">
                                <div class=""title"">Biblio</div>
                            </div>

                            <div class=""textbefore"">Your password is successfully changed.</p>
                            <p>If you did not changed your password, please reset it now or let us know.</p></div>      

                            <div class=""footer"">
                                <div class=""footcont"">© 2024. All Rights Reserved.</div>
                                <div><a href=""http://localhost:3001/terms"">Terms</a> - <a href=""http://localhost:3001/privacy"">Privacy</a></div>
                            </div>
                        </div>
                    </body>
                    </html>
                ";

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), true);
                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendEmailChangeCode(string toEmail, string code)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Biblio", "biblio.read.ebooks@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = "Email Change";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                    <html>
                        <head>
                            <style>
                                body {{
                                font-family: 'Arial', sans-serif;
                                background-color: #f4f4f4;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                background-color: #fff;
                                border-radius: 5px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                align-items: center;
                            }}
                            h1 {{
                                color: #E7734F;
                                margin-left: 30px;
                            }}
                            a {{
                                color: white;
                            }}
                            .block {{
                                background-image: url(https://media.npr.org/assets/img/2022/12/22/gettyimages-1245203807-1536x1029_wide-9982607ca51f99999656d993bf5511d42533c0f2-s1100-c50.jpg);
                                height: 130px;
                                background-size: cover;
                                width: 100%;
                                display: flex;
                                justify-content: right;
                                margin-right: 10px;
                            }}
                            .title {{
                                padding: 10px;
                                color: white;
                                font-size: 2.5rem;
                                font-weight: 600;
                            }}
                            .button {{
                                text-decoration: none;
                                color: white;
                                padding: 7px 20px;
                                background-color: #E7734F;
                                margin-left: 30px;
                            }}
                            .textbefore {{
                                padding: 20px 20px 10px 20px;

                            }}
                            .footer {{
                                margin-top: 35px;
                                width: 100%;
                                background-color: #dcdcdc;
                                padding: 15px 8px;
                                display: flex;
                                justify-content: space-between;
                            }}
                            .footer a {{
                                text-decoration: none;
                                color: black;
                            }}
                            .footcont {{
                                padding-right: 40px;
                            }}
                            .code {{
                                padding-left: 40px;
                                font-size: 1.8rem;
                            }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class=""block"">
                                    <div class=""title"">Biblio</div>
                                </div>

                                <div class=""textbefore"">This email contains email change code. If you haven't initiated a email reset, please CHANGE YOUR PASSWORD.</div>

                                <h1>Your email change code</h1>
                                <div class=""code"">{code}</div>

                                <div class=""footer"">
                                    <div class=""footcont"">© 2024. All Rights Reserved.</div>
                                    <div><a href=""http://localhost:3001/terms"">Terms</a> - <a href=""http://localhost:3001/privacy"">Privacy</a></div>
                                </div>
                            </div>
                        </body>
                        </html>
                ";

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), true);
                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendResetPasswordEmail(string toEmail, string token)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Biblio", "biblio.read.ebooks@gmail.com")); 
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = "Password Reset";

            string callbackUrl = $"http://localhost:3001/forgetpass/change?email={toEmail}&verificationToken={token}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                    <html>
                        <head>
                            <style>
                                body {{
                                font-family: 'Arial', sans-serif;
                                background-color: #f4f4f4;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                background-color: #fff;
                                border-radius: 5px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                align-items: center;
                            }}
                            h1 {{
                                color: #E7734F;
                                margin-left: 30px;
                            }}
                            a {{
                                color: white;
                            }}
                            .block {{
                                background-image: url(https://media.npr.org/assets/img/2022/12/22/gettyimages-1245203807-1536x1029_wide-9982607ca51f99999656d993bf5511d42533c0f2-s1100-c50.jpg);
                                height: 130px;
                                background-size: cover;
                                width: 100%;
                                display: flex;
                                justify-content: right;
                                margin-right: 10px;
                            }}
                            .title {{
                                padding: 10px;
                                color: white;
                                font-size: 2.5rem;
                                font-weight: 600;
                            }}
                            .button {{
                                text-decoration: none;
                                color: white;
                                padding: 7px 20px;
                                background-color: #E7734F;
                                margin-left: 30px;
                            }}
                            .textbefore {{
                                padding: 20px 20px 10px 20px;

                            }}
                            .footer {{
                                margin-top: 35px;
                                width: 100%;
                                background-color: #dcdcdc;
                                padding: 15px 8px;
                                display: flex;
                                justify-content: space-between;
                            }}
                            .footer a {{
                                text-decoration: none;
                                color: black;
                            }}
                            .footcont {{
                                padding-right: 40px;
                            }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class=""block"">
                                    <div class=""title"">Biblio</div>
                                </div>

                                <div class=""textbefore"">This email confirms a password reset request. If you haven't initiated a password reset, please disregard this message.</div>

                                <h1>Reset Your Password</h1>
                                <a href="" {HtmlEncoder.Default.Encode(callbackUrl)}"" style=""color: white;"" class=""button"">Reset Password</a>

                                <div class=""footer"">
                                    <div class=""footcont"">© 2024. All Rights Reserved.</div>
                                    <div><a href=""http://localhost:3001/terms"">Terms</a> - <a href=""http://localhost:3001/privacy"">Privacy</a></div>
                                </div>
                            </div>
                        </body>
                        </html>
                ";

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), true);
                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendChangeEmailVerificationEmail(string toEmail, string username, string verificationCode, string callbackUrl)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Biblio", "biblio.read.ebooks@gmail.com")); // Replace with your email and display name
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = "Email Verification";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Your verification code is: {verificationCode}";
            bodyBuilder.HtmlBody = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Arial', sans-serif;
                                background-color: #f4f4f4;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                background-color: #fff;
                                border-radius: 5px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                align-items: center;
                            }}
                            h1 {{
                                color: #E7734F;
                                margin-left: 30px;
                            }}
                            a {{
                                color: white;
                            }}
                            .block {{
                                background-image: url(https://media.npr.org/assets/img/2022/12/22/gettyimages-1245203807-1536x1029_wide-9982607ca51f99999656d993bf5511d42533c0f2-s1100-c50.jpg);
                                height: 130px;
                                background-size: cover;
                                width: 100%;
                                display: flex;
                                justify-content: right;
                                margin-right: 10px;
                            }}
                            .title {{
                                padding: 10px;
                                color: white;
                                font-size: 2.5rem;
                                font-weight: 600;
                            }}
                            .button {{
                                text-decoration: none;
                                color: white;
                                padding: 7px 20px;
                                background-color: #E7734F;
                                margin-left: 30px;
                            }}
                            .textbefore {{
                                padding: 20px 20px 10px 20px;

                            }}
                            .footer {{
                                margin-top: 35px;
                                width: 100%;
                                background-color: #dcdcdc;
                                padding: 15px 8px;
                                display: flex;
                                justify-content: space-between;
                            }}
                            .footer a {{
                                text-decoration: none;
                                color: black;
                            }}
                            .footcont {{
                                padding-right: 40px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class=""block"">
                                <div class=""title"">Biblio</div>
                            </div>

                            <div class=""textbefore"">Hello {username}, <p>This email serves as an account verification request. Please click the button below to confirm and activate your account:</p></div>

                            <h1>Account Verification</h1>
                            <a href="" {HtmlEncoder.Default.Encode(callbackUrl)}"" style=""color: white;"" class=""button"">Verify Account</a>

                            <div class=""footer"">
                                <div class=""footcont"">© 2024. All Rights Reserved.</div>
                                <div><a href=""http://localhost:3001/terms"">Terms</a> - <a href=""http://localhost:3001/privacy"">Privacy</a></div>
                            </div>
                        </div>
                    </body>
                    </html>
                ";

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), true);
                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendVerificationEmail(User user, string verificationCode, string callbackUrl)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Biblio", "biblio.read.ebooks@gmail.com")); // Replace with your email and display name
            emailMessage.To.Add(new MailboxAddress("", user.Email));
            emailMessage.Subject = "Email Verification";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Your verification code is: {verificationCode}";
            bodyBuilder.HtmlBody = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Arial', sans-serif;
                                background-color: #f4f4f4;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                background-color: #fff;
                                border-radius: 5px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                align-items: center;
                            }}
                            h1 {{
                                color: #E7734F;
                                margin-left: 30px;
                            }}
                            a {{
                                color: white;
                            }}
                            .block {{
                                background-image: url(https://media.npr.org/assets/img/2022/12/22/gettyimages-1245203807-1536x1029_wide-9982607ca51f99999656d993bf5511d42533c0f2-s1100-c50.jpg);
                                height: 130px;
                                background-size: cover;
                                width: 100%;
                                display: flex;
                                justify-content: right;
                                margin-right: 10px;
                            }}
                            .title {{
                                padding: 10px;
                                color: white;
                                font-size: 2.5rem;
                                font-weight: 600;
                            }}
                            .button {{
                                text-decoration: none;
                                color: white;
                                padding: 7px 20px;
                                background-color: #E7734F;
                                margin-left: 30px;
                            }}
                            .textbefore {{
                                padding: 20px 20px 10px 20px;

                            }}
                            .footer {{
                                margin-top: 35px;
                                width: 100%;
                                background-color: #dcdcdc;
                                padding: 15px 8px;
                                display: flex;
                                justify-content: space-between;
                            }}
                            .footer a {{
                                text-decoration: none;
                                color: black;
                            }}
                            .footcont {{
                                padding-right: 40px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class=""block"">
                                <div class=""title"">Biblio</div>
                            </div>

                            <div class=""textbefore"">This email confirms your request to change your account email address. To finalize your request, please click the button below.</p>
                            <p>If you did not initiate this request, please ignore this email.</p></div>

                            <h1>Account Verification</h1>
                            <a href="" {HtmlEncoder.Default.Encode(callbackUrl)}"" style=""color: white;"" class=""button"">Verify Account</a>

                            <div class=""footer"">
                                <div class=""footcont"">© 2024. All Rights Reserved.</div>
                                <div><a href=""http://localhost:3001/terms"">Terms</a> - <a href=""http://localhost:3001/privacy"">Privacy</a></div>
                            </div>
                        </div>
                    </body>
                    </html>
                ";

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), true);
                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }

}