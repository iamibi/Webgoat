using System;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OWASP.WebGoat.NET
{
    public partial class RegexDoS : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Code from https://www.owasp.org/index.php/Regular_expression_Denial_of_Service_-_ReDoS
        /// </summary>
        protected void btnCheckUsername_Click(object sender, EventArgs e)
        {
            string userName = txtUsername.Text;
            string available = txtAvailable.Text;
            string regex = "^(([A-Za-z])+.)+[A-Za-z]([0-9])+$";

            Regex testUsername = new Regex(regex);

            // Since .NET framework 4 doesn't support Regex timeout option
            // We are implementing our own timeout using the Task library.
            try
            {
                var task = Task.Factory.StartNew(() => testUsername.Match(userName));

                // Should complete in the 5 second window.
                bool completeInAllotedTime = task.Wait(TimeSpan.FromSeconds(5));
                
                // Kill the task thread that is running in the background.
                task.Wait(new System.Threading.CancellationToken(true));

                if (!completeInAllotedTime)
                {
                    lblError.Text = "Username does not meet acceptable standards.";
                }
                else
                {
                    lblError.Text = "Good username.";
                }
            }
            catch (Exception ex) 
            {
                lblError.Text = "Username does not meet acceptable standards. Please enter a valid username.";
            }
        }
    }
}

