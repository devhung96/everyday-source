using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Chats.Requests
{
    public class ShowAllUserSentRequest
    {
        public string EventId { get; set; }
        public string UserSentRequest { get; set; }
    }
}
