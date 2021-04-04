using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.DeviceTypes.Requests
{
    public class UpdateDeviceTypeRequest
    {
        [Required]
        public string DeviceTypeName { get; set; }
        //[AllowedExtensions(new string[] { ".jpg", ".png" , ".bmp" })]
        public IFormFile typeIcon { get; set; }
        [Required]
        public string DeviceTypeComment { get; set; }
    }
}
