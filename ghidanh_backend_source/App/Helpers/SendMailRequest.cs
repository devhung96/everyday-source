using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Helpers
{
    public class SendMailRequest
    {
        public List<SendMailContact> Contacts { get; set; }
        public string MessageSubject { get; set; }
        public string MessageContent { get; set; }
        public DateTime MessageScheduleTime { get; set; } = DateTime.Now.AddSeconds(-40);
    }

    public class SendMailContact
    {
        public string ContactEmail { get; set; }
        public string ContactName { get; set; } = "UserName";
    }
}
