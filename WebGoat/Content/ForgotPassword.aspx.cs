using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OWASP.WebGoat.NET.App_Code;
using OWASP.WebGoat.NET.App_Code.DB;

namespace OWASP.WebGoat.NET
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        private IDbProvider du = Settings.CurrentDbProvider;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PanelForgotPasswordStep2.Visible = false;
                PanelForgotPasswordStep3.Visible = false;
            }
        }

        protected void ButtonCheckEmail_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;

            if (!du.IsEmailValid(email))
            {
                labelQuestion.Text = "";
                PanelForgotPasswordStep2.Visible = false;
                PanelForgotPasswordStep3.Visible = false;

                return;
            }

            string[] result = du.GetSecurityQuestionAndAnswer(email);
            
            if (string.IsNullOrEmpty(result[0]))
            {
                labelQuestion.Text = "Invalid Email Address";
                PanelForgotPasswordStep2.Visible = false;
                PanelForgotPasswordStep3.Visible = false;
                
                return;
            }
            labelQuestion.Text = "Here is the question we have on file for you: <strong>" + result[0] + "</strong>";
            PanelForgotPasswordStep2.Visible = true;
            PanelForgotPasswordStep3.Visible = false;
            
                   
            HttpCookie cookie = new HttpCookie("encr_sec_qu_ans");

            //encode twice for more security!

            cookie.Value = Encoder.Encode(Encoder.Encode(result[1]));

            Response.Cookies.Add(cookie); 
        }

        protected void ButtonRecoverPassword_Click(object sender, EventArgs e)
        {
            try
            {
                //get the security question answer from the cookie
                string encrypted_password = Request.Cookies["encr_sec_qu_ans"].Value.ToString();
                
                //decode it (twice for extra security!)
                string security_answer = Encoder.Decode(Encoder.Decode(encrypted_password));
                
                if (security_answer.Trim().ToLower().Equals(txtAnswer.Text.Trim().ToLower()))
                {
                    SendVerificationEmail(txtEmail.Text);
                    PanelForgotPasswordStep1.Visible = false;
                    PanelForgotPasswordStep2.Visible = false;
                    PanelForgotPasswordStep3.Visible = true;
                    labelPassword.Text = "An email has been sent to the registered email address for changing the password.";
                }
            }
            catch (Exception ex)
            {
                labelMessage.Text = "An unknown error occurred - Do you have cookies turned on? Further Details: " + ex.Message;
            }
        }

        string getPassword(string email)
        {
            string password = du.GetPasswordByEmail(email);
            return password;
        }

        private bool SendVerificationEmail(string email)
        {
            string totpCode = Util.GetTOTPCode();
            string url = "http://localhost:52251/Content/ChangePwd.aspx?token=" + totpCode;
            string email_subject = "Change Password on WebGoatCoins Portal";
            string email_body = "Please follow the link to change your password: " + url;

            try
            {
                du.UpdateDbWithToken(email, totpCode);
                SendEmailUtil.SendEmail(email, email_subject, email_body);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}