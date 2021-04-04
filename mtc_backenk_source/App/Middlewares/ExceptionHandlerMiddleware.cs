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
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.Value.Contains("/api/supportapp/download/")|| httpContext.Request.Path.Value.Contains("/api/SupportApp/download/") )
            {
                await _next(httpContext);
                return;
            }

            using MemoryStream memoryStream = new MemoryStream();
            Stream originalResponseBody = httpContext.Response.Body;
            httpContext.Response.Body = memoryStream;
          
            await _next(httpContext);


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
                        ["message"] = /*errors.FirstOrDefault().Value[0].Contains(" ") ? "RequestInvalid" :*/ errors.FirstOrDefault().Value[0]
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
    

}
