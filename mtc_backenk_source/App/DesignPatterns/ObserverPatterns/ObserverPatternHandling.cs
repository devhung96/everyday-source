using Microsoft.Extensions.Configuration;
using Project.App.Requests;
using System.Net.Mail;

namespace Project.App.DesignPatterns.ObserverPatterns
{
    public class ObserverPatternHandling
    {
        public static readonly ObserverPatternHandling instance = new ObserverPatternHandling();
        public static ObserverPatternHandling Instance => instance;
        public static IConfiguration configuration;
        public ObserverPatternHandling() { }
        public void Connection(IConfiguration _configuration)
        {
            configuration = _configuration;
            Handling();
        }
        public void Handling()
        {
            ObserverPattern.Instance.On("EventName", (data) =>
            {

            });
            ObserverPattern.Instance.On("SendMail", (data) =>
            {
                SendMailRequest mailRequest = data as SendMailRequest;
                SendMail(mailRequest);
            });
        }
        private void SendMail(SendMailRequest mailRequest)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(configuration["SendMail:Server"]);

            mail.From = new MailAddress(configuration["SendMail:Sender"], configuration["SendMail:SenderName"]);
            mail.To.Add(mailRequest.Contacts[0].ContactEmail);
            mail.Subject = mailRequest.MessageSubject;
            mail.Body = mailRequest.MessageContent;
            mail.IsBodyHtml = true;
            if(mailRequest.Attachment !=  null)
            {
                Attachment attachment = mailRequest.Attachment;
                mail.Attachments.Add(attachment);
            }

            SmtpServer.Port = int.Parse(configuration["SendMail:Port"]);
            SmtpServer.Credentials = new System.Net.NetworkCredential(configuration["SendMail:Sender"], configuration["SendMail:Password"]);
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }
    }
}
