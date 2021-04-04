using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.PermissionGroups.Entities;
using Project.Modules.PermissonUsers;
using Project.Modules.Users.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.App.Middleware
{
    public class CheckTokenMiddleware
    {
        const int Forbidden = (int)HttpStatusCode.Forbidden;
        private readonly RequestDelegate _next;
        private IConfiguration _config;

        public CheckTokenMiddleware() { }
        public CheckTokenMiddleware(RequestDelegate next, IConfiguration configuration)
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
            int ID = int.Parse(context.User.FindFirstValue("UserID"));
            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(_config["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            using (var mariaDBContext = new MariaDBContext(optionsBuilder.Options))
            {
                User user = mariaDBContext.Users.Find(ID);
            
                
                #region Lấy tất cả Role của User trong Db
                List<string> RoleDB = new List<string>();
                List<PermissionUser> permissionUsers = mariaDBContext.PermissionUsers.Where(m => m.UserId.Equals(ID)).ToList();
                if (!(permissionUsers is null))
                    foreach (var item in permissionUsers)
                    {
                        RoleDB.Add(item.PermissionCode);
                    }


                #endregion

                #region Lấy tất cả Role thức tế trong Token
                List<string> RoleToken = new List<string>();
                var Roles = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
                foreach (var item in Roles)
                {
                    RoleToken.Add(item.Value);
                }
                #endregion
            
                if ((RoleDB.Count() != RoleToken.Count())||(RoleDB.Except(RoleToken).Count()!=0)||(RoleToken.Except(RoleDB).Count()!=0))
                {
                    response["message"] = "PermissionHasBeenChanged";
                    context.Response.StatusCode = Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(response.ToString());
                }
                else
                {

                    await _next.Invoke(context);
                }
            }

        }



        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CheckTokenMiddleware>();
        }

    }
  
}

