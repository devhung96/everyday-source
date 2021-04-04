using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Requests
{
    public class UpdateDeviceRequest
    {
        public object DeviceSettingData { get; set; }
        public string DeviceCode { get; set; }
        public string DevicePassword { get; set; }
        public string DeviceName { get; set; }
        public string DeviceMac { get; set; }
        public string DeviceTypeId { get; set; }
    }
}
