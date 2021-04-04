using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Detections.Requests
{
    public class DetectTagDeviceRequest
    {
        public string DeviceId { get; set; }
        public string ModeId { get; set; }
        public string KeyCode { get; set; }
        public IFormFile Image { get; set; }
    }
}
