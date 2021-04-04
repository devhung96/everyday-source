using Microsoft.AspNetCore.Http;
//using Project.App.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.DeviceTypes.Requests
{
    public class CreateDeviceType
    {
        [Required]
        public string typeName { get; set; }
        //[AllowedExtensions(new string[] { ".jpg", ".png" , ".bmp" })]
        public IFormFile typeIcon { get; set; }
        public string typeComment { get; set; }
    }
}
