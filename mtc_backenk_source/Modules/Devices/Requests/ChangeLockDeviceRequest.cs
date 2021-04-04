using Project.Modules.Devices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Requests
{
    public class ChangeLockDeviceRequest
    {
        public ENUMDEVICELOCK DeviceLock { get; set; }
    }
    public class ChangeLockMultiDeviceRequest
    {
        public ENUMDEVICELOCK DeviceLock { get; set; }
        public string DeviceId { get; set; }
    }
}
