using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.Organizes.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.App.Middleware
{
    public class CheckLoginClient
    {
        const int Forbidden = (int)HttpStatusCode.Forbidden;
        private readonly RequestDelegate _next;
        private IConfiguration _config;

        public CheckLoginClient() { }
        public CheckLoginClient(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _config = configuration;

        }
        public async Task Invoke(HttpContext context)
        {
            JObject response = new JObject
            {
                ["statusCode"] = Forbidden,
                ["code"] = Forbidden
            };
            if(String.IsNullOrEmpty(context.Request.Headers["Authorization"].ToString()))
            {
                Console.WriteLine("token null");
                await _next.Invoke(context);
                return;
            }
            
            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(_config["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            using (var mariaDBContext = new MariaDBContext(optionsBuilder.Options))
            {
                (bool flagCheckToken, JwtSecurityToken dataToken) = ValidateToken(context.Request.Headers["Authorization"].ToString().Substring(7));
                if (!flagCheckToken)
                {
                    await ResponseError(context, (int)HttpStatusCode.Unauthorized, "Token invalid");
                }
                else
                {
                    var token = context.Request.Headers["Authorization"].ToString().Substring(7);
                    if (context.User.FindFirstValue("PermissionDefault").Equals("CLIENT") && !String.IsNullOrEmpty(context.User.FindFirstValue("EventId")))
                    {
                        string userID = context.User.FindFirst("UserID").Value;
                        string eventID = context.User.FindFirstValue("EventId");
                        EventUser eventUser = mariaDBContext.EventUsers.FirstOrDefault(x => x.UserId.Equals(userID) && x.EventId.Equals(eventID));
                        if(eventUser != null)
                        {
                            if(!(eventUser.UserToken?.Equals(token) ?? false))
                            {
                                await ResponseError(context, (int)HttpStatusCode.Unauthorized, "Tài khoản đã được đăng nhập từ một nơi khác.");
                            }
                            else
                            {
                                await _next.Invoke(context);
                            }
                        }
                        else
                        {
                            await _next.Invoke(context);
                        }
                    }
                    else
                    {
                        await _next.Invoke(context);
                    }
                }
            }
        }
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CheckLoginClient>();
        }
        private static Task ResponseError(HttpContext httpContext, int statusCode, string message)
        {
            JObject response = new JObject
            {
                ["statusCode"] = statusCode,
                ["code"] = statusCode,
                ["message"] = message
            };
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";
            return httpContext.Response.WriteAsync(response.ToString());
        }

        private static (bool flagCheckToken, JwtSecurityToken dataToken) ValidateToken(string token)
        {
            JwtSecurityToken dataToken = new JwtSecurityToken();
            try
            {
                dataToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            }
            catch (Exception)
            {
                return (false, null);
            }
            return (true, dataToken);
        }
    }
}
