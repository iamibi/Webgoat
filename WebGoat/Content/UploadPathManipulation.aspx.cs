using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace OWASP.WebGoat.NET
{
    public partial class UploadPathManipulation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                try
                {
                    string filename = Path.GetFileName(FileUpload1.FileName);

                    // Create the temporary directory and get the path to it.
                    // It will be located in AppData/Local/Temp folder.
                    string tempDirectoryWithPath = GetTempDirectoryWithPath();

                    FileUpload1.SaveAs(Path.Combine(tempDirectoryWithPath, filename));
                    labelUpload.Text = $"<div class='success' style='text-align:center'>The file {filename} has been saved.</div>";

                    
                }
                catch (Exception ex)
                {
                    labelUpload.Text = "<div class='error' style='text-align:center'>Upload Failed: " + ex.Message + "</div>";
                }
                finally
                {
                    labelUpload.Visible = true;
                }
            }
        }

        private string GetTempDirectoryWithPath() 
        {
            string tempDirectoryWithPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectoryWithPath);
            return tempDirectoryWithPath;
        }
    }
}