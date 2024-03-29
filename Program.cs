using Microsoft.EntityFrameworkCore;
using BiblioServer.Context;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using BiblioServer.Services;
using BiblioServer.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using BiblioServer.Middlewares;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BiblioServer.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//Data Base context connection
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

///// Dependency Injection - Custom Services /////

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IDownloadReadUserRepository, DownloadReadUserRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();

string securityKey = builder.Configuration["JwtSettings:SecurityKey"] ?? throw new InvalidOperationException("JwtSettings:SecurityKey is missing in configuration.");
builder.Services.AddScoped<ITokenService, TokenService>(provider => new TokenService(securityKey));

builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IResetPasswordService, ResetPasswordService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IChangeEmailService, ChangeEmailService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IDownloadReadUserService, DownloadReadUserService>();
builder.Services.AddScoped<IRatingService, RatingService>();


builder.Services.AddScoped<IBookService, BookService>();

////////////////////////////////////////////////

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
                };
            });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminAuthorize", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "Admin");
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:3000") //Front-End url for cors 
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // Set the limit to 50 MB (in bytes)
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//app.UseMiddleware<RequestRateLimitMiddleware>(3, "/User/resendverificationcode");
//app.UseMiddleware<RequestRateLimitMiddleware>(3, "/User/password-reset-request");

//app.UseMiddleware<EmailRateLimitMiddleware>(3, "EmailService");

app.Run();
