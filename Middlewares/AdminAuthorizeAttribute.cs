using System;
using Microsoft.AspNetCore.Authorization;

namespace BiblioServer.Middlewares
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public AdminAuthorizeAttribute()
        {
            Policy = "AdminAuthorize";
        }
    }

}

