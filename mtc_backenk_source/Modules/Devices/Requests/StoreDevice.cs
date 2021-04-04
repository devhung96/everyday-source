using Microsoft.AspNetCore.Http;
using Project.App.Request;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Validations;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Requests
{
    public class StoreDevice
    {
        /// <summary>
        /// 
        /// </summary>
        ///<example>Led</example>
        public string DeviceName { get; set; }
        public IFormFile Image { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ///<example>192.168.11.27</example>
        [CheckExitsDeviceIpValidation]
        public string LoginName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ///<example>123123123</example>
        [DataType(DataType.Password)]
        //[StringLength(40, MinimumLength = 8, ErrorMessage = "Password cannot be longer than 40 characters and less than 8 characters")]
        public string DevicePass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ///<example>1</example>
        public DEVICESTATUS DeviceStatus { get; set; } = DEVICESTATUS.ACTIVED;
        /// <summary>
        /// 
        /// </summary>
        ///<example>led comment</example>
        public string DeviceComment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ///<example>10000</example>
        public float? DeviceMemory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ///<example>1</example>
        public string DeviceTypeId { get; set; } = "1";
        /// <summary>
        /// 
        /// </summary>
        ///<example>TPHCM</example>
        public string DeviceLocation { get; set; }
        public DateTime DeviceExpired { get; set; }
        public DateTime DeviceWarrantyExpiresDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ///<example>192.168.11.23</example>
        public string DeviceMACAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ///<example>1921681123</example>
        public string DeviceSku { get; set; }
        public string DeviceSetting { get; set; }
        public string GroupId { get; set; }


    }
}
