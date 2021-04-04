using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.UserPermissionEvents.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project.App.Middleware
{
    public class CheckEventMiddleware
    {
        private readonly RequestDelegate _next;
        private IConfiguration _config;

        public CheckEventMiddleware() { }
        public CheckEventMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _config = configuration;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            if(!String.IsNullOrEmpty(httpContext.Request.Headers["event-id"]))
            {
                httpContext.Request.Headers["Event-Id"] = httpContext.Request.Headers["event-id"].ToString();
            }
            if (!String.IsNullOrEmpty(httpContext.Request.Headers["Event-Id"]) && !String.IsNullOrEmpty(httpContext.Request.Headers["Authorization"]))
            {
                string eventId = httpContext.Request.Headers["Event-Id"].ToString();
                (bool flagCheckToken, JwtSecurityToken dataToken) = ValidateToken(httpContext.Request.Headers["Authorization"].ToString().Substring(7));
                if (!flagCheckToken)
                {
                    await ResponseError(httpContext, (int)HttpStatusCode.Unauthorized, "Token invalid");
                }
                else
                {
                    // set permission default 

                    var token = httpContext.Request.Headers["Authorization"].ToString().Substring(7);
                    string dataEvents = dataToken.Claims.First(claim => claim.Type == "PermissionEvents").Value;
                    if (dataEvents != null)
                    {
                   
                        JArray listEvent = JArray.Parse(dataEvents);
                        if (listEvent.Any(x => x.SelectToken("EventID").ToString().Equals(eventId)))
                        {
                            string userID = dataToken.Claims.First(claim => claim.Type == "UserId").Value;
                            if (dataToken.Claims.First(claim => claim.Type == "PermissionDefault").Value.Equals("ADMIN"))
                            {
                                List<Claim> claims = new List<Claim>();
                                int maxFunction = GetMaxFunction();
                                for (int i = 0; i < maxFunction + 1; i++)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "31"));
                                }
                                httpContext.User.AddIdentity(new ClaimsIdentity(claims));
                                await _next.Invoke(httpContext);
                            }
                            else
                            {

                                var getPermissions = GetPermissionEventNew(userID).Where(x => x.SelectToken("EventID").ToString().Equals(eventId)).FirstOrDefault();
                                JObject dataEvent = JObject.Parse(listEvent.FirstOrDefault(x => x.SelectToken("EventID").ToString().Equals(eventId)).ToString());
                                JArray permissions = JArray.Parse(dataEvent["PermissionCode"].ToString());
                                if (getPermissions == null || !getPermissions["PermissionCode"].SequenceEqual(permissions))
                                {
                                    await ResponseError(httpContext, (int)HttpStatusCode.Unauthorized, "Quyền của bạn đã bị thay đổi trong sự kiện này. Vui lòng đăng nhập lại.");
                                }
                                else
                                {
                                    List<Claim> claims = new List<Claim>();
                                    foreach (var permission in permissions)
                                    {
                                        claims.Add(new Claim(ClaimTypes.Role, permission.ToString()));
                                    }
                                    httpContext.User.AddIdentity(new ClaimsIdentity(claims));
                                    await _next.Invoke(httpContext);
                                }
                            }
                        }
                        else
                        {
                            await ResponseError(httpContext, (int)HttpStatusCode.Forbidden, "Bạn không có quyền truy cập vào sự kiện này.");
                        }
                    }
                    else
                    {
                        await _next.Invoke(httpContext);
                    }
                }
            }
            else
            {
                await _next.Invoke(httpContext);
            }
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

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CheckEventMiddleware>();
        }
        private List<JObject> GetPermissionEventNew(string userId)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(_config["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            using (var mariaDBContext = new MariaDBContext(optionsBuilder.Options))
            {
                var permissionEvent = mariaDBContext.UserPermissionEvents.Where(x => x.UserId.Equals(userId)).ToList();
                var listGroupbyEvent = permissionEvent.GroupBy(x => x.EventId).ToList();
                List<JObject> result = new List<JObject>();
                foreach (var item in listGroupbyEvent)
                {
                    var temp = new
                    {
                        EventID = item.Key,
                        PermissionCode = GetPermission(item.Select(x => x.PermissionId).ToList())
                    };
                    result.Add(JObject.FromObject(temp));
                }
            return result;
            }
        }
        //Get Permission
        private List<int> GetPermission(List<int> permissionIDs)
        {
            #region way first
            //List<int> result = new List<int>();
            //List<JObject> pms = new List<JObject>();
            //foreach (int item in permissionIDs) // Get all permision
            //{
            //    var permission = mariaDBContext.Permissions.Find(item);
            //    if (permission != null)
            //    {
            //        var xx = new { index = permission.FunctionId, code = permission.PermissionCode };
            //        pms.Add(JObject.FromObject(xx));
            //    }
            //}
            //for (int i = 0; i < 15; i++)
            //{
            //    result.Add(pms.Where(x => int.Parse(x["index"].ToString()) == i).Sum(x => int.Parse(x["code"].ToString())));
            //}
            //return result;
            #endregion
            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(_config["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            using (var mariaDBContext = new MariaDBContext(optionsBuilder.Options))
            {
                var permissionOrganizes = mariaDBContext.Permissions
                .Where(x => permissionIDs.Contains(x.PermissionId)).ToList()
                .GroupBy(x => x.FunctionId)
                .OrderBy(x => x.Key).ToList();
                int maxFunction = mariaDBContext.Functions.Max(x => x.FunctionId); // số lớn nhất của function (tổng function)
                int[] result = new int[maxFunction + 1];
                foreach (var item in permissionOrganizes)
                {
                    result[item.Key] = item.Sum(x => int.Parse(x.PermissionCode));
                }
                return result.ToList();
            }
        }
        private int GetMaxFunction()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(_config["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            using (var mariaDBContext = new MariaDBContext(optionsBuilder.Options))
            {
                return mariaDBContext.Functions.Max(x => x.FunctionId);
            }
        }
    }
}
