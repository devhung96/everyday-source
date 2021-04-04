using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Project.App.Database;
using Project.Modules.Logs.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Project.App.Providers
{
    public class TransportPatternHandlingProvider
    {
        public static readonly TransportPatternHandlingProvider instance = new TransportPatternHandlingProvider();
        public static TransportPatternHandlingProvider Instance => instance;
        public static IConfiguration _config;
        public TransportPatternHandlingProvider() { }
        public void Connection(IConfiguration configuration)
        {
            _config = configuration;
            Handling();
        }

        private void SendMailSMTP(SendMailRequest request)
        {
            int.TryParse(_config["EmailSettings:MailPort"], out int port);
            SmtpClient smtpClient = new SmtpClient(_config["EmailSettings:MailServer"], port)
            {
                Credentials = new System.Net.NetworkCredential(_config["EmailSettings:Username"], _config["EmailSettings:Password"]),
                EnableSsl = true,
                //UseDefaultCredentials = true
            };

            MailMessage mail = new MailMessage
            {
                //Setting From , To and CC
                From = new MailAddress(_config["EmailSettings:Sender"], _config["EmailSettings:SenderName"]),
                Subject = request.MessageSubject,
                IsBodyHtml = true,
                Body = request.MessageContent,
            };

            mail.To.Add(new MailAddress(request.Contacts[0].ContactEmail));

            var a = smtpClient.SendMailAsync(mail);
            Console.WriteLine("________________________________________________________");
            Console.WriteLine("Send Mail");
            Console.WriteLine(JsonConvert.SerializeObject(a));
            Console.WriteLine("________________________________________________________");
        }

        private void SendMailAPI(SendMailRequest request)
        {
            string url = _config["EmailSettings:UrlApi"];
            Dictionary<string, object> header = new Dictionary<string, object>
            {
                {"Authorization", $"Bearer {_config["EmailSettings:Token"]}" }
            };
            (string message, int? statusCode) = HttpMethod.Post.SendRequestWithStringContentAsync(url, JsonConvert.SerializeObject(request), header).Result;
            Console.WriteLine("------------------------------");
            Console.WriteLine(message);
            Console.WriteLine(statusCode);
            Console.WriteLine(request.Contacts[0].ContactEmail);
        }
        public void Handling()
        {
            TransportPatternProvider.Instance.On("WriteLogAccess", (data) =>
            {
                AddLogRequest request = data as AddLogRequest;
                if(!GeneralHelper.TypeLogAccess.TryGetValue(request.TypeLog, out string action))
                {
                    return;
                }

                using MongoDBContext dbContext = new MongoDBContext(_config);
                dbContext.LogAccesses.InsertOne(new LogAccess
                {
                    IpAddress = request.IpAddress,
                });
            });


            TransportPatternProvider.Instance.On("SendEmail", (data) =>
            {
                SendMailRequest request = data as SendMailRequest;
                SendMailSMTP(request);
                //SendMailAPI(request);
            });
        }
    }

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

    public class AddLogRequest
    {
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public int TypeLog { get; set; }
    }
}
