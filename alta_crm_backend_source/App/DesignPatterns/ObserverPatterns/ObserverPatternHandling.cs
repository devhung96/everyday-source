using Microsoft.Extensions.Configuration;
using Project.App.Helpers;
using Project.App.Requests;
using System;
using System.Net;
using System.Net.Mail;

namespace Project.App.DesignPatterns.ObserverPatterns
{
    public class ObserverPatternHandling
    {
        public static readonly ObserverPatternHandling instance = new ObserverPatternHandling();
        public static ObserverPatternHandling Instance => instance;
        private IConfiguration Configuration;
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
            });

        }
        private void SendMailSMTP(SendMailRequest request)
        {

            using SmtpClient smtpClient = new SmtpClient
            {
                Host = Configuration["EmailSettings:MailServer"],
                Port = int.Parse(Configuration["EmailSettings:MailPort"]),
                EnableSsl = true,
                Credentials = new NetworkCredential
                {
                    UserName = Configuration["EmailSettings:Sender"],
                    Password = Configuration["EmailSettings:Password"]
                }
            };
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(Configuration["EmailSettings:Sender"], Configuration["EmailSettings:SenderName"]),
                Subject = request.MessageSubject,
                IsBodyHtml = true,
                Body = request.MessageContent,
            };

            mail.To.Add(new MailAddress(request.Contacts[0].ContactEmail));
            smtpClient.Send(mail);

        }

    }
}

