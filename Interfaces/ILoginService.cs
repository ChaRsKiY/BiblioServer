using System;
using BiblioServer.Models;

namespace BiblioServer.Services
{
    public interface ILoginService
    {
        Task<string> LoginUserAsync(UserLoginModel loginModel);
    }
}

