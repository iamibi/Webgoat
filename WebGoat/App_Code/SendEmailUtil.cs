using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OWASP.WebGoat.NET.App_Code
{
    public class SendEmailUtil
    {
        private const string host = "smtp.gmail.com";
        private const int port = 587;

        public static void SendEmail(string to, string email_subject, string email_body)
        {
            string from = "WebGoatCoins@webgoatcoins.net";
            MailMessage message = new MailMessage(from, to);
            message.Subject = email_subject;
            message.Body = email_body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            
            SmtpClient client = new SmtpClient(host, port);
            Dictionary<string, string> credentials = LoadCredentials();
            NetworkCredential basicCredentials = new NetworkCredential(credentials["email_id"], credentials["password"]);
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

        private static Dictionary<string, string> LoadCredentials()
        {
            Dictionary<string, string> credentials = new Dictionary<string, string>();
            try
            {
                string json = File.ReadAllText("C:\\Users\\student\\Workspace\\SendEmailCredentials\\credentials.json");
                dynamic creds = JObject.Parse(json);

                credentials["email_id"] = creds.email_id;
                credentials["password"] = creds.password;
            }
            catch (Exception ex)
            {
            }
            return credentials;
        }
    }
}