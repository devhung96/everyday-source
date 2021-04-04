using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Requests
{
    public class CopyGroupRequest
    {
        public int GroupId { get; set; }
        public string NewName { get; set; }
    }
}
