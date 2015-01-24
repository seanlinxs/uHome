using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using uHome.Models;

namespace uHome.Services
{
    public class MessageService
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void SendMail(string From, string To, string Subject, string Message)
        {
            // Credentials:
            var credentialUserName = ConfigurationManager.AppSettings["MailAccount"];
            var pwd = ConfigurationManager.AppSettings["MailPassword"];

            // Configure the client:
            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["MailServer"]);
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Create the credentials:
            NetworkCredential credentials = new NetworkCredential(credentialUserName, pwd);
            client.EnableSsl = true;
            client.Credentials = credentials;

            // Create the message:
            var mail = new MailMessage(From, To);
            mail.Subject = Subject;
            // Send html message body instead of plain text
            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(Message, null, MediaTypeNames.Text.Html));

            // Send:
            const int MAX_RETRY = 5;
            int retry = 0;

            while (retry < MAX_RETRY)
            {
                try
                {
                    retry += 1;
                    client.Send(mail);
                    break;
                }
                catch (Exception e)
                {
                    logger.Debug(e.Message);
                    Thread.Sleep(1000);
                    continue;
                }
            }
        }

        public static void SendMailAsync(string From, string To, string Subject, string Message)
        {
            Thread emailThread = new Thread(() =>
                {
                    SendMail(From, To, Subject, Message);
                });
            emailThread.IsBackground = true;
            emailThread.Start();
        }
    }
}