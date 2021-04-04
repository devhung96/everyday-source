using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Project.App.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandler
    {
        const int BAD_REQUEST = (int)HttpStatusCode.BadRequest;
        const int UNAUTHORIZED = (int)HttpStatusCode.Unauthorized;
        const int NOT_FOUND = (int)HttpStatusCode.NotFound;
        const int METHOD_NOT_ALLOWED = (int)HttpStatusCode.MethodNotAllowed;

        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originBody = context.Response.Body;
            var newBody = new MemoryStream();
            context.Response.Body = newBody;

            await _next.Invoke(context);

            int STATUS_CODE = context.Response.StatusCode;
            JObject response = new JObject
            {
                ["statusCode"] = STATUS_CODE,
                ["code"] = STATUS_CODE
            };

            newBody.Seek(0, SeekOrigin.Begin);
            string responseOriginData = new StreamReader(newBody).ReadToEnd();

            context.Response.Body = originBody;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = STATUS_CODE;

            if (String.IsNullOrEmpty(responseOriginData))
            {
                if (STATUS_CODE == UNAUTHORIZED)
                {
                    response["message"] = "Unauthorized.";
                    await context.Response.WriteAsync(response.ToString());
                }
                else if (STATUS_CODE == NOT_FOUND)
                {
                    response["message"] = "Not Found.";
                    await context.Response.WriteAsync(response.ToString());
                }
                else if (STATUS_CODE == METHOD_NOT_ALLOWED)
                {
                    response["message"] = "Method Not Allowed.";
                    await context.Response.WriteAsync(response.ToString());
                }
                else
                {
                    await context.Response.WriteAsync(responseOriginData);
                }
            }
            else
            {
                if (STATUS_CODE == BAD_REQUEST)
                {
                    JObject result = JObject.Parse(responseOriginData);
                    if (result["errors"] != null)
                    {
                        JObject errors = JObject.Parse(result["errors"].ToString());
                        var keys = errors
                            .Properties()
                            .Select(p => p.Value)
                            .FirstOrDefault();

                        response["message"] = keys[0];
                        await context.Response.WriteAsync(response.ToString());
                    }
                    else
                    {
                        await context.Response.WriteAsync(responseOriginData);
                    }
                }
                else
                {
                    await context.Response.WriteAsync(responseOriginData);
                }
            }
        }
    }
}
