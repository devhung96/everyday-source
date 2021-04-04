using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Requests
{
    public class ShowType
    {
        [Required]
        public int DeviceType { get; set; }
        [Required]
        public int draw { get; set; }
        [Required]
        public int length { get; set; }
        [Required]
        public int start { get; set; }
    }
}
