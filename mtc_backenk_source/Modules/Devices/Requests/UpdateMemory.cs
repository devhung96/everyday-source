using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Requests
{
    public class UpdateMemory
    {
        public double? DeviceMemory { get; set; }
        public double? DeviceUseMemory { get; set; }
    }
}
