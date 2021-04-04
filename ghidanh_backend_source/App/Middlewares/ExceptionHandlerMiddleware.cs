using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Project.App.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate RequestDelegate;

        public ExceptionHandlerMiddleware(RequestDelegate requestDelegate)
        {
            RequestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if(httpContext.Request.Path.ToString().Contains("/api/class/"))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (httpContext.Request.Path.ToString().Contains("/api/subjects") ||
                httpContext.Request.Path.ToString().Contains("/api/subjectGroups") ||
                httpContext.Request.Path.ToString().Contains("/api/slots") ||
                httpContext.Request.Path.ToString().Contains("/api/courses"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (httpContext.Request.Path.ToString().Contains("/api/EmployeeSalary"))
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine("___________________________________________");
            Console.WriteLine(httpContext.Request.Path);
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine("___________________________________________");

            using MemoryStream memoryStream = new MemoryStream();
            Stream originalResponseBody = httpContext.Response.Body;
            httpContext.Response.Body = memoryStream;

            await RequestDelegate(httpContext);

            int currentStatusCode = httpContext.Response.StatusCode;

            memoryStream.Seek(0, SeekOrigin.Begin);
            string responseOriginData = new StreamReader(memoryStream).ReadToEnd();

            httpContext.Response.Body = originalResponseBody;
            httpContext.Response.ContentLength = null;
            httpContext.Response.ContentType = "application/json";

            if (currentStatusCode == (int)HttpStatusCode.BadRequest)
            {
                (bool resultParse, JToken dataResponse) = IsValidJson(responseOriginData);
                if (resultParse && dataResponse["errors"] != null)
                {
                    Dictionary<string, List<string>> errors = dataResponse["errors"].ToObject<Dictionary<string, List<string>>>();
                    JObject responseData = new JObject
                    {
                        ["statusCode"] = currentStatusCode,
                        ["message"] = JToken.FromObject(errors.FirstOrDefault().Value[0])
                    };
                    await httpContext.Response.WriteAsync(responseData.ToString());
                }
                else
                {
                    await httpContext.Response.WriteAsync(responseOriginData);
                }
            }
            else
            {
                await httpContext.Response.WriteAsync(responseOriginData);
            }
        }

        private (bool resultParse, JToken dataParse) IsValidJson(string strInput)
        {
            try
            {
                JToken token = JContainer.Parse(strInput);
                return (true, token);
            }
            catch (Exception)
            {
                return (false, null);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
