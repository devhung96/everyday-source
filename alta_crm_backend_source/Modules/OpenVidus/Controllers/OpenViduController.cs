using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.Modules.OpenVidus.Enities;
using Project.Modules.OpenVidus.Extension;
using Project.Modules.OpenVidus.Requests;
using Project.Modules.OpenVidus.Response;
using Project.Modules.OpenVidus.Services;
using Project.Modules.Users.Requests;
using System;
using System.Security.Claims;

namespace Project.Modules.OpenVidus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenViduController : BaseController
    {
        private readonly IViduService ViduService;
        private readonly Helper TokenHelper;
        private readonly IConfiguration Configuration;
        public OpenViduController(IViduService viduService, IConfiguration configuration)
        {
            ViduService = viduService;
            Configuration = configuration;
            TokenHelper = new Helper(configuration);
        }

        [HttpPost("register/{roomId}")]
        public IActionResult Register([FromBody] FlagIsPublish flagIsPublish, string roomId)
        {
            string cusId = "pikachu";
            string userId = User?.FindFirstValue("UserId") ?? cusId + "-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
            flagIsPublish.IsPublisher = true;
            (int? status, string data, SessionStream sessionStream) = ViduService.RegisterUserToOpenViduAsync(userId, roomId, flagIsPublish.IsPublisher);
            GetToken getToken = JsonConvert.DeserializeObject<GetToken>(data);
            if (!(status is null))
            {
                return ResponseOk(new 
                { 
                    token = getToken.token,
                    message = getToken.message,
                    sessionStream = sessionStream,
                    userId = userId
                }, "RegisterViduSuccess");
            }
            return ResponseBadRequest(getToken.message);
        }

        [HttpPost("remove-vidu/{roomId}")]
        public IActionResult Remove([FromBody] TokenVidu tokenVidu, string roomId)
        {
            (int? status, string data) = ViduService.RemoveRoomVidu(tokenVidu.UserId, roomId, tokenVidu.Token, tokenVidu.IsPublisher);
            if (!(status is null))
            {
                return ResponseOk(data, "RemoveTokenViduSuccess");
            }
            return ResponseBadRequest(data);
        }
    }
}
