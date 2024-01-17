using System;
using BiblioServer.Models;

namespace BiblioServer.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}

