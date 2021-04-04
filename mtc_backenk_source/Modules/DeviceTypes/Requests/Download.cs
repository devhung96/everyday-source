using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.DeviceTypes.Requests
{
    public class Download
    {
        public string DeviceToken { get; set; }
        public int MediaID { get; set; }
    }
}
