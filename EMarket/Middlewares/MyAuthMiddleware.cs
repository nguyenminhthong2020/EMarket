using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace EMarket.Middlewares
{
    public class MyAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public MyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path;
            if (path.Value.ToLower() == "/admin/login.html")
            {
                return _next(httpContext);
            }


            if (path.Value.ToLower().StartsWith("/admin"))
            {

                string taikhoanID = httpContext.Session.GetString("AdminAccountId");
                int? taikhoanRole = httpContext.Session.GetInt32("AdminRoleId");

                if (!(httpContext.User.Identity.IsAuthenticated && (httpContext.User.IsInRole("admin") || httpContext.User.IsInRole("staff")) && taikhoanID != null && taikhoanRole != null))
                {
                    httpContext.Response.Redirect("/admin/login.html");
                }

                return _next(httpContext);
            }

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MyAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyAuthMiddleware>();
        }
    }
}
