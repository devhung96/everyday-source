using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.RegisterDetects.Requests;
using Project.Modules.RegisterDetects.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetectsController : BaseController
    {
        private readonly IRegisterDetectService _registerDetectService;
        public DetectsController(IRegisterDetectService registerDetectService)
        {
            _registerDetectService = registerDetectService;
        }
        [HttpPost("Register")]
        public IActionResult RegisterUserDetect([FromBody] RegisterUserDetectRequest request)
        {
            (object result, string message) = _registerDetectService.RegisterUserDetect(request);
            if (result is null) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }

        [HttpPost("RegisterMutil")]
        public IActionResult RegisterUserDetectMutil([FromBody] RegisterUserDetectMutilRequest request)
        {
            (object result, string message) = _registerDetectService.RegisterUserDetectMutil(request);
            if (result is null) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }



        [HttpPost("UnRegister")]
        public IActionResult UnRegisterUserDetect([FromBody] UnRegisterUserDetectRequest request)
        {
            (bool result, string message) = _registerDetectService.UnRegisterUserDetect(request);
            if (!result) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }


        [HttpPost("UnRegisterMutil")]
        public IActionResult UnRegisterUserDetectMutil([FromBody] UnRegisterUserDetectMutilRequest request)
        {
            (bool result, string message) = _registerDetectService.UnRegisterUserMutil(request);
            if (!result) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }

        [HttpDelete("UnRegisterByUser/{userId}")]
        public IActionResult UnRegisterUserByUserId(string userId)
        {
            (bool result, string message) = _registerDetectService.UnRegisterUserWithUserId(userId);
            if (!result) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }


    }
}
