using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Requests
{
    public class DeleteMultiMemoryRequest
    {
        public List<string> deviceIds { get; set; }
    }
}
