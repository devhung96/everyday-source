using Microsoft.Extensions.Configuration;
using Project.App.Helpers;
using System.Net;
using System.Net.Mail;

namespace Project.App.Providers
{
    public class TransportPatternHandlingProvider
    {
        public static readonly TransportPatternHandlingProvider instance = new TransportPatternHandlingProvider();
        public static TransportPatternHandlingProvider Instance => instance;

        private IConfiguration Config;
        public TransportPatternHandlingProvider() { }
        public void Connection(IConfiguration config)
        {
            Config = config;
            Handling();
        }
        public void Handling()
        {
            TransportPatternProvider.Instance.On("EventName", (data) =>
            {

            });
            TransportPatternProvider.Instance.On("SendEmail", (data) =>
            {
                SendMailRequest request = data as SendMailRequest;
                SendMailSMTP(request);
            });

        }
        private void SendMailSMTP(SendMailRequest request)
        {

            SmtpClient smtpClient = new SmtpClient
            {
                Host = Config["EmailSettings:MailServer"],
                Port = int.Parse(Config["EmailSettings:MailPort"]),
                EnableSsl = true,
                Credentials = new NetworkCredential
                {
                    UserName = Config["EmailSettings:Sender"],
                    Password = Config["EmailSettings:Password"]
                }
            };
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(Config["EmailSettings:Sender"], Config["EmailSettings:SenderName"]),
                Subject = request.MessageSubject,
                IsBodyHtml = true,
                Body = request.MessageContent,
            };

            mail.To.Add(new MailAddress(request.Contacts[0].ContactEmail));
            smtpClient.Send(mail);

        }
    }
}
