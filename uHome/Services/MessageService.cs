using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using uHome.Models;

namespace uHome.Services
{
    public static class MessageService
    {
        public static SmtpClient client;

        static MessageService()
        {
            // Credentials:
            var credentialUserName = ConfigurationManager.AppSettings["MailAccount"];
            var pwd = ConfigurationManager.AppSettings["MailPassword"];

            // Configure the client:
            client = new System.Net.Mail.SmtpClient("smtp-mail.outlook.com");
            client.Port = 587;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Create the credentials:
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
            client.EnableSsl = true;
            client.Credentials = credentials;
        }

        public static Task SendMail(string From, string To, string Subject, string Message)
        {
           // Create the message:
            var mail = new System.Net.Mail.MailMessage(From, To);
            mail.Subject = Subject;
            // Send html message body instead of plain text
            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(Message, null, MediaTypeNames.Text.Html));

            // Send:
            const int MAX_RETRY = 5;
            Task result = null;
            int retry = 0;

            while (result == null && retry < MAX_RETRY)
            {
                try
                {
                    retry += 1;
                    result = client.SendMailAsync(mail);
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    continue;
                }
            }

            return result;
        }
    }
}