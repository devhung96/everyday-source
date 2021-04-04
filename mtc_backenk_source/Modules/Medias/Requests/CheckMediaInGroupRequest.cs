using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class CheckMediaInGroupRequest
    {
        public List<string> MediaIds { get; set; }
        public string GroupId { get; set; }
    }
}
