using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PlayLists.Requests
{
    public class CopyPlaylistDetailRequest
    {
        public string PlayListIdCopy { get; set; }
        public string PlayListIdPaste { get; set; }
    }
}
