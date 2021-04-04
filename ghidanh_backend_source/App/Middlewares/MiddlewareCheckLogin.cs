using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Project.App.Databases;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.App.Middlewares
{
    public class MiddlewareCheckLogin
    {
        private readonly IConfiguration configuration;
        private readonly RequestDelegate _next;
        const int Forbidden = (int)HttpStatusCode.Forbidden;
        const int Unauthorized = (int)HttpStatusCode.Unauthorized;
        public MiddlewareCheckLogin()
        {
        }
        public MiddlewareCheckLogin(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            this.configuration = configuration;
        }
        public async Task Invoke(HttpContext context)
        {

            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(configuration["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            using MariaDBContext mariaDBContext = new MariaDBContext(optionsBuilder.Options);
            string AccountId = context.User.FindFirstValue("AccountId").ToString();
            User user = mariaDBContext.Users.FirstOrDefault(m=>m.AccountId.Equals(AccountId));
            if (user is null)
            {
                JObject response = new JObject
                {
                    ["statusCode"] = Unauthorized,
                    ["code"] = Unauthorized,
                    ["message"] = "UserNotExist"
                };
                context.Response.StatusCode = Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response.ToString());
            }
            else
            {
                List<string> RoleDB = mariaDBContext.AccountPermissions.Where(m => m.AccountId.Equals(AccountId)).Select(m => m.PermissionCode).ToList();
                List<string> RoleToken = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(m => m.Value).ToList();
                if (RoleDB.Except(RoleToken).ToList().Count == 0 && RoleToken.Except(RoleDB).ToList().Count == 0)
                {
                    await _next.Invoke(context);
                }
                else
                {
                    JObject response = new JObject
                    {
                        ["statusCode"] = Unauthorized,
                        ["code"] = Unauthorized,
                        ["message"] = "PermissionHasChange"
                    };
                    context.Response.StatusCode = Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(response.ToString());
                }
            }

        }
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<MiddlewareCheckLogin>();
        }

    }
}