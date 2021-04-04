using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Detections.Requests;
using Project.Modules.Detections.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Detections.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetectionsController : BaseController
    {
        private readonly IDetectService DetectService;

        public DetectionsController(IDetectService detectService)
        {
            DetectService = detectService;
        }

        /// <summary>
        /// ### Effect -- Xác thực người dùng qua thiết bị (Hiện chỉ có FaceId)
        /// ### Artist -- An
        /// ### Des -- App
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Detection([FromForm] DetectTagDeviceRequest request)
        {
            var detect = DetectService.Detect(request.DeviceId, request.ModeId, request.KeyCode, request.Image);

            if (!detect.check)
            {
                return ResponseBadRequest(detect.message);
            }

            return ResponseOk(detect.data, detect.message);
        }
    }
}
