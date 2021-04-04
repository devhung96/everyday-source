using Microsoft.AspNetCore.Http;
using Project.App.Request;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Requests
{
    public class UpdateDevice 
    {
        public string DeviceName { get; set; }
        public string DevicePass { get; set; }
        public IFormFile Image { get; set; }
        public string DeviceOldPass { get; set; }
        public DEVICESTATUS DeviceStatus { get; set; } = DEVICESTATUS.ACTIVED;
        public string DeviceComment { get; set; }
        public float? DeviceMemory { get; set; }
        [CheckDeviceType]
        public string DeviceTypeId { get; set; } = "1";
        public string DeviceLocation { get; set; }
        public DateTime? DeviceExpired { get; set; }
        public DateTime? DeviceWarrantyExpiresDate { get; set; }
        public string DeviceMACAddress { get; set; }
        public string DeviceSetting { get; set; }
        public string GroupId { get; set; }
    }
}
