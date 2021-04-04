
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
//using System.Web.Http.Controllers;

namespace Project.Modules.Question.Validation
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public string AccessLevel { get; set; }

        public CustomAuthorizeAttribute()
        {
            AccessLevel = "AccessLevel";
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

        }
    }
}
