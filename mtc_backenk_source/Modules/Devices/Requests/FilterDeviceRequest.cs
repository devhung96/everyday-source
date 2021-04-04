using Project.App.Helpers;
using Project.Modules.Devices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Requests
{
    public class FilterDeviceRequest : PaginationRequest
    {
        public DEVICESTATUS? DeviceStatus { get; set; }
    }
}
