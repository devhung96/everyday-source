using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Templates.Entities;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Project.App.Middlewares
{
    public class CheckTemplateMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly IServiceScopeFactory ServirceScopeFatory;

        public CheckTemplateMiddleware() { }

        public CheckTemplateMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            Next = next;
            ServirceScopeFatory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            (bool flagCheckToken, JwtSecurityToken dataToken) = httpContext.Request.Headers["Authorization"].ToString().Substring(7).ValidateToken();

            if (!flagCheckToken)
            {
                await ResponseError(httpContext, (int)HttpStatusCode.Unauthorized, "TokenInvalid");
            }

            else
            {
                string userId = httpContext.User.FindFirst("user_id")?.Value;
                string groupId = httpContext.User.FindFirst("group_id")?.Value;

                using IServiceScope serviceScope = ServirceScopeFatory.CreateScope();
                IRepositoryWrapperMariaDB RepositoryWrapperMariaDB = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

                List<TemplateShare> templateShares = RepositoryWrapperMariaDB.TemplateShares.FindByCondition(x => x.GroupId.Equals(groupId)).ToList();
                List<Template> templates = RepositoryWrapperMariaDB.Templates.FindByCondition(x => x.UserId.Equals(userId)).ToList();

                //if (templateShares.Count == 0 && templates.Count == 0)
                //{
                //    await ResponseError(httpContext, (int)HttpStatusCode.Forbidden, "YouDoNotHaveAccessToAnyTemplate");
                //}

                if (templateShares.Count + templates.Count == 0)
                {
                    await ResponseError(httpContext, (int)HttpStatusCode.Forbidden, "YouDoNotHaveAccessToAnyTemplate");
                }

                else
                {
                    await Next.Invoke(httpContext);
                }
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



        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CheckTemplateMiddleware>();
        }
    }
}
