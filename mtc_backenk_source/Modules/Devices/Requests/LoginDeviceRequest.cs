using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Requests
{
    public class LoginDeviceRequest
    {
        [Required]
        public string DeviceIp { get; set; }
        [Required]
        public string DevicePass { get; set; }
    }
}
