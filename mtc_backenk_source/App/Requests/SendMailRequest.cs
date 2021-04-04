using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Project.App.Requests
{
    public class SendMailRequest
    {
        public string MessageSubject { get; set; }
        public string MessageContent { get; set; }
        public List<SendMailContact> Contacts { get; set; }
        public Attachment Attachment { get; set; }
    }
    public class SendMailContact
    {
        public string ContactEmail { get; set; }
        public string ContactName { get; set; } = "UserName";
    }
}