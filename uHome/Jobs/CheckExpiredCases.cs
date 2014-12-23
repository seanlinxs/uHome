using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using uHome.Models;
using uHome.Services;

namespace uHome.Jobs
{
    public class CheckExpiredCases : IJob
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            var From = ConfigurationManager.AppSettings["MailSentFrom"];
            var managers = UserService.FindUsersByRoleName("Manager").ToList();
            var admins = UserService.FindUsersByRoleName("Admin").ToList();
            var validDays = double.Parse(ConfigurationManager.AppSettings["ValidDays"]);

            using (var db = new ApplicationDbContext())
            {
                var now = System.DateTime.Now;
                var before = now.AddDays(-1 * validDays);

                foreach (var c in db.Cases.Where(i => i.State != CaseState.CLOSED).Where(i => i.UpdatedAt < before).ToList())
                {
                    var recipients = new HashSet<string>();

                    foreach (var user in managers.Concat(admins))
                    {
                        recipients.Add(user.Email);
                    }

                    // Close it
                    c.OldState = c.State;
                    c.State = CaseState.CLOSED;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        logger.Debug(string.Format("\nException: {0}", e.InnerException.Message));
                    }

                    recipients.Add(c.CreatedBy.Email);

                    if (c.CaseAssignment.Assignee != null)
                    {
                        recipients.Add(c.CaseAssignment.Assignee.Email);
                    }

                    logger.Debug(string.Format("\nrecipients: {0}", string.Join(", ", recipients)));
                    string To = string.Join(",", recipients);
                    string Subject = string.Format(Resources.Resources.CaseExpiredSubject, c.Title);
                    string Message = string.Format(Resources.Resources.CaseExpiredMessage, c.Title, validDays);
                    MessageService.SendMail(From, To, Subject, Message);
                }
            }
        }
    }
}