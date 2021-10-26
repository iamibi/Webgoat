using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OWASP.WebGoat.NET.App_Code.DB;
using OWASP.WebGoat.NET.App_Code;
using System.Web.Security;

namespace OWASP.WebGoat.NET
{
    public partial class ChangePwd : System.Web.UI.Page
    {
        private IDbProvider du = Settings.CurrentDbProvider;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonChangePassword_Click(object sender, EventArgs e)
        {
            if (txtPassword1.Text != null && txtPassword2.Text != null && txtPassword1.Text == txtPassword2.Text)
            {
                int customerNumber;
                try
                {
                    string token = Request.QueryString["token"];
                    if (!Util.VerifyCode(token))
                    {
                        throw new Exception("Invalid Otp");
                    }
                    customerNumber = du.GetCustomerId(token);
                    string output = du.UpdateCustomerPassword(customerNumber, txtPassword1.Text);
                    labelMessage.Text = output;
                }
                catch (Exception ex)
                {
                    labelMessage.Text = "Something went wrong! Please try later.";
                    return;
                }
            }
            else
            {
                labelMessage.Text = "Passwords do not match!  Please try again!";
            }

        }
    }
}