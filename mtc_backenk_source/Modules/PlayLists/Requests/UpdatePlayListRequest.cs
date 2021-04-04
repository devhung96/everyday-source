using Project.App.Request;
using Project.Modules.PlayLists.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PlayLists.Requests
{
    public class UpdatePlayListRequest : BaseRequest<PlayList>
    {
        public string PlayListName { get; set; }
        public PlayListLoopEnum PlayListLoop { get; set; }
        public string PlayListComment { get; set; }
        public string PlayListAssignUserId { get; set; }
    }
}
