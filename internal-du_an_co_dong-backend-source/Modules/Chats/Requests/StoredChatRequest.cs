using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Chats.Requests
{
    public class StoredChatRequest
    {
        public string UserSent { get; set; }
        public string ChatContent { get; set; }
        public string UserRecieve { set; get; }
        public string Event_Id { set; get; }
    }
}
