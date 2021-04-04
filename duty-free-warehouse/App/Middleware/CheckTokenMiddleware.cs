using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.App.Middleware
{
    public class CheckTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        public CheckTokenMiddleware() { }
        public CheckTokenMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.FindFirst("UserName") != null)
            {
                string userName = context.User.FindFirst("UserName").Value.ToString();
                string message = null;
                List<string> permissions = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
                var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
                optionsBuilder.UseMySql(_config["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
                using (var _mariaDBContext = new MariaDBContext(optionsBuilder.Options))
                {
                    User user = _mariaDBContext.Users.Include(x => x.UserPermissions).FirstOrDefault(x => x.UserName.Equals(userName));
                    if (user is null)
                        message = "Tài khoản không tồn tại.";
                    else if (!CompareTwoList(user.UserPermissions.Select(x => x.PermissionCode).ToList(), permissions))
                    {
                        message = "Tài khoản có sự thay đổi quyền trên hệ thống.";
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
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CheckTokenMiddleware>();
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

    }
}
