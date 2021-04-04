using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.Logs.Entities;

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
            //  Enable seeking
            context.Request.EnableBuffering();
            //  Read the stream as text
            var bodyAsText = await new System.IO.StreamReader(context.Request.Body).ReadToEndAsync();
            //  Set the position of the stream to 0 to enable rereading
            context.Request.Body.Position = 0;
            // --------------------------------------------------------------------------------

            var originBody = context.Response.Body;
            var newBody = new MemoryStream();
            context.Response.Body = newBody;
            await _next.Invoke(context);

            Console.WriteLine("________________________________________________");
            Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            Console.WriteLine(context.Request.Path);
            Console.WriteLine(context.Connection.RemoteIpAddress.ToString());
            Console.WriteLine("________________________________________________");

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
            
            context.Response.ContentLength = null;
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
                        List<string> messages = new List<string>();
                        Dictionary<string, List<string>> errors = result["errors"].ToObject<Dictionary<string, List<string>>>();
                        foreach (var item in errors)
                        {
                            //messages.AddRange(item.Value.Select(x => string.IsNullOrEmpty(item.Key) ? x : item.Key + ": " + x));
                            // devhung edit message error 400 khong dung
                            messages.AddRange(item.Value.Select(x => string.IsNullOrEmpty(item.Key) ? x : item.Key.First().ToString().ToUpper() + item.Key.Substring(1) + x));
                        }
                        response["message"] = JToken.FromObject(messages.FirstOrDefault());
                        await context.Response.WriteAsync(response.ToString());
                    }
                    else
                    {
                        await context.Response.WriteAsync(responseOriginData);
                    }
                }
                else
                {
                    IConfiguration configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json").Build();

                    using MongoDBContext dbContext = new MongoDBContext(configuration);


                    //dbContext.LogAccesses.InsertOne(new LogAccess
                    //{
                    //    Body = bodyAsText,
                    //    Path = context.Request.Path,
                    //    Header = JsonConvert.SerializeObject(context.Request.Headers.ToList()),
                    //    Query = JsonConvert.SerializeObject(context.Request.Query.ToList()),
                    //    //Response = responseOriginData,
                    //    IpAddress = context.Connection.RemoteIpAddress.ToString()
                    //});
                    await context.Response.WriteAsync(responseOriginData);
                }
            }
        }
    }
}
