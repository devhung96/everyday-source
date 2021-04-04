using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Requests
{
    public class DeleteMultiDeviceRequest
    {
        [Required]
        public List<string> DeviceIds { get; set; }
    }
}
