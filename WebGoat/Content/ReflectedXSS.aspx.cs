using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OWASP.WebGoat.NET.App_Code.DB;
using OWASP.WebGoat.NET.App_Code;
using System.Web.Security.AntiXss;

namespace OWASP.WebGoat.NET
{
	public partial class ReflectedXSS : System.Web.UI.Page
	{
        private IDbProvider du = Settings.CurrentDbProvider;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["city"] != null)
                LoadCity(Request["city"]);
        }

		void LoadCity (String city)
		{
            // Whitelist for the cities.
            string[] cities = { "San Francisco", "Boston", "NYC", "Paris", "Tokyo", "Sydney", "London" };

            // Return early if the city is not present in the whitelist.
            if (!cities.Contains(city))
            {
                lblOutput.Text = "Invalid city";
                dtlView.DataSource = null;
                dtlView.DataBind();
                return;
            }
            DataSet ds = du.GetOffice(city);
            lblOutput.Text = $"Here are the details for our {AntiXssEncoder.HtmlEncode(city, false)} Office";
            dtlView.DataSource = ds.Tables[0];
            dtlView.DataBind();
		}

        void FixedLoadCity (String city)
        {
            DataSet ds = du.GetOffice(city);
            lblOutput.Text = "Here are the details for our " + Server.HtmlEncode(city) + " Office";
            dtlView.DataSource = ds.Tables[0];
            dtlView.DataBind();
        }
	}
}