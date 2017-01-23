using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Cox.Helpers
{
    public static class Mailer
    {
        public static void SendMail(string receivers, string attachment)
        {
            string smtpHost = ConfigurationManager.AppSettings["SMTP_Host"].ToString();
            int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_Port"].ToString());
            string sender = ConfigurationManager.AppSettings["Email_Sender"].ToString();
            string body = ConfigurationManager.AppSettings["Email_Body"].ToString();
            string subject = ConfigurationManager.AppSettings["Email_Subject"].ToString();
            string emailDelimiter = ConfigurationManager.AppSettings["Email_Delimiter"];


            MailMessage mail = new MailMessage();

            SmtpClient smtpServer = new SmtpClient(smtpHost);

            smtpServer.Port = smtpPort;

            mail.From = new MailAddress(sender);

            if (receivers.Contains(emailDelimiter))
            {
                string[] receiverList = receivers.Split(emailDelimiter.ToCharArray());

                foreach (var receiver in receiverList)
                {
                    if (string.IsNullOrEmpty(receiver)) continue;
                    mail.To.Add(receiver.Trim());
                }

            }
            else
            {
                mail.To.Add(receivers.Trim());
            }


            mail.Subject = subject;
            mail.Body = body;

            mail.Attachments.Add(new Attachment(attachment));

            smtpServer.Send(mail);

            mail.Dispose();
        }
    }
}