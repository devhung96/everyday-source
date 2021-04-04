using Project.Modules.Logs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Logs.Models
{
    public class ResponseLog
    {
        public string UserId { get; set; }
        public string LogName { get; set; }
        public string LogAccess { get; set; }
        public DateTime LogAccessTime { get; set; }
        public string DeviceName { get; set; }
        public string LogRegion { get; set; }
        public string DeviceType { get; set; }
        public string LogMessage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public LogStatus LogStatus { get; set; }
    }
}
