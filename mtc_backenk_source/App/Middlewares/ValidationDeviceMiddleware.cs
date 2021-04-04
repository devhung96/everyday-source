using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Devices.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Middlewares
{
    public class ValidationDeviceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _servirceScopeFatory;

        public ValidationDeviceMiddleware()
        {
        }
        public ValidationDeviceMiddleware(RequestDelegate next, IServiceScopeFactory servirceScopeFatory)
        {
            _next = next;
            _servirceScopeFatory = servirceScopeFatory;
        }


        public async Task Invoke(HttpContext httpContext)
        {

            #region Validate token faild
            string token = httpContext.ParsingToken();
            if (String.IsNullOrEmpty(token))
            {
                await SendErrorUnAuthorizedAsync(httpContext);
                return;
            }
            #endregion

            #region Option validate device : Error thì đá nó ra.
            var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            var attributeValidateDevice = endpoint?.Metadata.GetMetadata<ValidateDeviceAtribute>();

            using IServiceScope serviceScope = _servirceScopeFatory.CreateScope();
            IRepositoryWrapperMariaDB repositoryWrapperMariaDB = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

            DeviceTokens deviceToken = repositoryWrapperMariaDB.DeviceTokens.FirstOrDefault(x => x.DeviceToken == token);
            if (deviceToken == null)
            {
                await SendErrorForbiddenAsync(httpContext);
                return;
            }
            var device = repositoryWrapperMariaDB.Devices.FirstOrDefault(x => x.DeviceId == deviceToken.DeviceID);

            if (attributeValidateDevice.IsCheckDeviceExpired)
            {
                if (device is null)
                {
                    await SendErrorForbiddenAsync(httpContext);
                    return;
                }

                var deviceResponse = new DeviceResponse(device);
                if (deviceResponse.DeviceExpired == 1) 
                {
                    await SendError(httpContext, "DeviceIsExpired");
                    return;
                } 
            }
            if (attributeValidateDevice.IsCheckStatus)
            {
                if (device is null)
                {
                    await SendErrorForbiddenAsync(httpContext);
                    return;
                }

                if (device.DeviceStatus == DEVICESTATUS.DEACTIVATED)
                {
                    await SendError(httpContext, "DeviceIsDeactived");
                    return;
                }
            }
            httpContext.Request.Headers.Add("DeviceId", device?.DeviceId);
            #endregion

            await _next.Invoke(httpContext);
        }
        public async Task SendErrorUnAuthorizedAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = 401;
            JObject responseData = new JObject
            {
                ["statusCode"] = 401,
                ["message"] = "UnAuthorized"
            };
            await httpContext.Response.WriteAsync(responseData.ToString());

        }

        public async Task SendErrorForbiddenAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = 403;
            JObject responseData = new JObject
            {
                ["statusCode"] = 403,
                ["message"] = "Forbidden"
            };
            await httpContext.Response.WriteAsync(responseData.ToString());

        }

        public async Task SendError(HttpContext httpContext, string message)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = 400;
            JObject responseData = new JObject
            {
                ["statusCode"] = 400,
                ["message"] = message
            };
            await httpContext.Response.WriteAsync(responseData.ToString());

        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ValidationDeviceMiddleware>();
        }





    }
}
