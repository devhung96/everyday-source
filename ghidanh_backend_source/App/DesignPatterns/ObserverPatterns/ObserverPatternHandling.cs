using Microsoft.Extensions.Configuration;
using Project.App.Helpers;
using System;
using System.Net;
using System.Net.Mail;

namespace Project.App.DesignPatterns.ObserverPatterns
{
    public class ObserverPatternHandling
    {
        public static readonly ObserverPatternHandling instance = new ObserverPatternHandling();
        public static ObserverPatternHandling Instance => instance;
        public static IConfiguration Configuration;
        public ObserverPatternHandling() { }
        public void Connection(IConfiguration configuration)
        {
            Configuration = configuration;
            Handling();
        }
        public void Handling()
        {
            ObserverPattern.Instance.On("EventName", (data) =>
            {

            });
            ObserverPattern.Instance.On("SendEmail", (data) =>
            {
                SendMailRequest request = data as SendMailRequest;
                SendMailSMTP(request);
                //SendMailAPI(request);
            });
        }
        private void SendMailSMTP(SendMailRequest request)
        {

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = Configuration["EmailSettings:MailServer"];
            smtpClient.Port = int.Parse(Configuration["EmailSettings:MailPort"]);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential
            {
                UserName = Configuration["EmailSettings:Sender"],
                Password = Configuration["EmailSettings:Password"]
            };
            MailMessage mail = new MailMessage
            {
                //Setting From , To and CC
                From = new MailAddress(Configuration["EmailSettings:Sender"], Configuration["EmailSettings:SenderName"]),
                Subject = request.MessageSubject,
                IsBodyHtml = true,
                Body = request.MessageContent,
            };

            mail.To.Add(new MailAddress(request.Contacts[0].ContactEmail));
            var data = smtpClient.SendMailAsync(mail);
            
            Console.WriteLine(data.IsCompletedSuccessfully);

        }

    }


}
