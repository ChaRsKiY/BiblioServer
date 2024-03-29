using System;
using BiblioServer.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BiblioServer.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _securityKey;

        public TokenService(string securityKey)
        {
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }

        public string GenerateJwtToken(User user)
        {
            var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

            bool isAdmin = user.IsAdmin ?? false;

            //Token data
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
            };

            //Creating a token
            var token = new JwtSecurityToken(
                issuer: "",
                audience: "",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(72),
                signingCredentials: credentials
            );

            //Signing a token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

