using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class ShareMultipleIntoGroupRequest
    {
        public string MediaId { get; set; }
        public List<string> GroupIds { get; set; }
    }
}
