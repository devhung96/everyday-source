using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Project.App.Middlewares
{
    public class CheckUserSuperAdminMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory servirceScopeFatory;
        const int Unauthorized = (int)HttpStatusCode.Unauthorized;
        const int Forbidden = (int)HttpStatusCode.Forbidden;
        public CheckUserSuperAdminMiddleware() { }
        public CheckUserSuperAdminMiddleware(RequestDelegate next, IServiceScopeFactory _servirceScopeFatory)
        {
            _next = next;
            servirceScopeFatory = _servirceScopeFatory;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            JObject response = new JObject
            {
                ["statusCode"] = Unauthorized,
                ["code"] = Unauthorized
            };
            using IServiceScope serviceScope = servirceScopeFatory.CreateScope();
            IRepositoryWrapperMariaDB unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();
            var userId = context.User.FindFirst("user_id")?.Value.ToString();
            User user = unitOfWork.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault();
            if (user != null)
            {
                context.Response.StatusCode = Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response.ToString());
            }
            else
            if(user.UserLevel != UserLevelEnum.SUPERADMIN)
            {
                response = new JObject
                {
                    ["statusCode"] = Unauthorized,
                    ["code"] = Unauthorized,
                    ["message"] = "UserNoSuperAdmin"
                };
                context.Response.StatusCode = Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response.ToString());
            }
            await _next(context);
        }
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CheckUserSuperAdminMiddleware>();
        }

    }
}
