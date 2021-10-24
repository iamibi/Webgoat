using System;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OWASP.WebGoat.NET.App_Code.DB;
using OWASP.WebGoat.NET.App_Code;
using log4net;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace OWASP.WebGoat.NET.WebGoatCoins
{
    public partial class CustomerLogin : System.Web.UI.Page
    {
        private IDbProvider du = Settings.CurrentDbProvider;
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string logFileName = "logfile.txt";
        private const string logFilePath = "C:\\Users\\student\\Workspace\\webgoat\\WebGoat\\WebGoatCoins\\Logs\\" + logFileName;
        private const string spaces = "     ";

        protected void Page_Load(object sender, EventArgs e)
        {
            PanelError.Visible = false;

            string returnUrl = Request.QueryString["ReturnUrl"];
            if (returnUrl != null)
            {
                PanelError.Visible = true;
            }
        }

        protected void ButtonLogOn_Click(object sender, EventArgs e)
        {
            string email = txtUserName.Text;
            string pwd = txtPassword.Text;

            log.Info("User " + email + " attempted to log in with password " + pwd);

            bool LoginEntry = CheckLoginEntry(email);

            int cn = -1;
            if (!LoginEntry)
                cn = du.CheckValidCustomerLogin(email, pwd);

            if (cn == -1 || LoginEntry)
            {
                labelError.Text = "Incorrect Login!";
                PanelError.Visible = true;
                LogEntry(email);
                return;
            }

            // put ticket into the cookie
            FormsAuthenticationTicket ticket =
                        new FormsAuthenticationTicket(
                            1, //version 
                            email, //name 
                            DateTime.Now, //issueDate
                            DateTime.Now.AddDays(14), //expireDate 
                            true, //isPersistent
                            cn.ToString(), //userData (customer role)
                            FormsAuthentication.FormsCookiePath //cookiePath
            );

            string encrypted_ticket = FormsAuthentication.Encrypt(ticket); //encrypt the ticket

            // put ticket into the cookie
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted_ticket);

            //set expiration date
            if (ticket.IsPersistent)
                cookie.Expires = ticket.Expiration;
                
            Response.Cookies.Add(cookie);
            
            string returnUrl = Request.QueryString["ReturnUrl"];
            
            if (returnUrl == null) 
                returnUrl = "/WebGoatCoins/MainPage.aspx";
                
            Response.Redirect(returnUrl);        
        }

        private static void LogEntry(string logMessage)
        {
            DateTime utcDateTimeNow = DateTime.UtcNow;
            string utcNow = utcDateTimeNow.ToString();

            try
            {
                // Check if the file exists or not.
                // If not then create it.
                if (!File.Exists(logFilePath))
                {
                    // This is to dispose of the stream or else the next command will fail.
                    using (File.Create(logFilePath))
                    { }
                }

                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(string.Format("{0}" + spaces + "{1}", utcNow, logMessage));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[ERROR] exception occurred while logging: " + ex.Message);
            }
        }

        private bool CheckLoginEntry(string email)
        {
            if (!File.Exists(logFilePath))
                return false;

            List<string> Attempts = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(logFilePath))
                {
                    string line;
                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        // Read next line if the current line doesn't contain email address.
                        if (!line.Contains(email))
                            continue;

                        // If the List reaches a count of 3, remove the first element.
                        if (Attempts.Count == 3)
                            Attempts.RemoveAt(0);

                        Attempts.Add(line);
                    }
                }

                // If the login attemts made are not equal to 3, return false
                if (Attempts.Count != 3)
                    return false;

                // Retrieve the timestamp from the string.
                string[] dateTime = new string[3];
                for(int i = 0; i < 3; i++)
                    dateTime[i] = Regex.Split(Attempts[i], spaces).First();

                // Parse the timestamp to a DateTime object.
                DateTime[] convertedDateTime = new DateTime[3];
                for(int i = 0; i < 3; i++)
                    convertedDateTime[i] = DateTime.Parse(dateTime[i]);

                // Calculate the time difference between the recent request and the last request.
                TimeSpan diff = convertedDateTime[2] - convertedDateTime[0];
                if (diff.TotalHours < 1.0)
                    return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[ERROR] exception occurred while logging: " + ex.Message);
            }
            return false;
        }
    }
}