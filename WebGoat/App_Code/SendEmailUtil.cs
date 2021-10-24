using System;
using System.Net;
using System.Net.Mail;

namespace OWASP.WebGoat.NET.App_Code
{
    public class SendEmailUtil
    {
        private const string host = "smtp.gmail.com";
        private const int port = 587;

        public static void SendEmail(string to, string email_subject, string email_body)
        {
            string from = "";
            MailMessage message = new MailMessage(from, to);
            message.Subject = email_subject;
            message.Body = email_body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            
            SmtpClient client = new SmtpClient(host, port);
            NetworkCredential basicCredentials = new NetworkCredential();
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredentials;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}