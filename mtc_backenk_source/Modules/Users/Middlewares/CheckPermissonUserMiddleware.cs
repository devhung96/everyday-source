using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.Modules.Users.Middlewares
{
    /// <summary>
    /// Kiểm tra quyền user có thay dổi trong db so với token . Có thì bắt login lại để cập nhật lại quyền cho user.
    /// </summary>
    public class CheckPermissonUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _provider;
        public CheckPermissonUserMiddleware(RequestDelegate next, IServiceProvider provider)
        {
            _next = next;
            _provider = provider;
        }

        public async Task Invoke(HttpContext context)
        {

            if (context.Request.Path.Value.Contains("/api/user/login"))
            {
                await _next(context);
                return;
            }
            if (context.User.FindFirst("user_id") != null)
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                string token = null;
                if (authHeader != null && authHeader.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }

                var userId = context.User.FindFirst("user_id");
                string message = null;
                List<string> permissions = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

                using (var scope = _provider.CreateScope())
                {
                    var _userService = scope.ServiceProvider.GetService<IUserService>();
                    (User user, string messageCheckUser) = _userService.ShowDetail(userId.ToString());

                    var preparePermissionInDB = new List<string>();
                    if (user is null)
                        message = "Account does not exist!.";
                    else 
                    {
                        preparePermissionInDB = user.UserPermissions is null ? new List<string>() : user.UserPermissions.Select(x => x.PermissionCode).ToList();
                        if (!CompareTwoList(preparePermissionInDB, permissions))
                            message = "Account with permission changes on the system.";
                    }
                }

                if (message != null)
                {
                    JObject responseData = new JObject();
                    responseData["statusCode"] = (int)HttpStatusCode.Unauthorized;
                    responseData["code"] = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    responseData["message"] = message;
                    await context.Response.WriteAsync(responseData.ToString());
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }

        public bool CompareTwoList<T>(List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            foreach (var item in list1)
            {
                if (!list2.Contains(item))
                    return false;
            }
            return true;
        }

        //public class CheckPermissonUserMiddlewareAttribute
        //{
        //    public void Configure(IApplicationBuilder app)
        //    {
        //        app.UseMiddleware<CheckPermissonUserMiddleware>();
        //    }
        //}

    }
}
