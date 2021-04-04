using Project.Modules.Logs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Logs.Requests
{
    public class StoreLogRequest
    {
        public string LogName { get; set; }
        public string DeviceId { get; set; }
        public string LogAccess { get; set; }
        public string UserId { get; set; }
        public LogStatus LogStatus { get; set; }
        public string LogMessage { get; set; }
        public DateTime LogAccessTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
