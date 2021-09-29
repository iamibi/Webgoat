using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Security.AccessControl;

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
                    // It will be located in WebGoatCoins/uploads folder.
                    string tempDirectoryWithPath = GetTempDirectoryWithPath();

                    // Store the file in the temporary directory.
                    string fileLocation = Path.Combine(tempDirectoryWithPath, filename);
                    FileUpload1.SaveAs(fileLocation);

                    // Remove the execution flag for the file.
                    SetFileAccess(fileLocation);

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
            string tempDirectoryWithPath = Path.Combine(Server.MapPath("~/WebGoatCoins/uploads"), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectoryWithPath);
            return tempDirectoryWithPath;
        }

        private void SetFileAccess(string path)
        {
            string identity = "student";
            var fileSecurity = new FileSecurity();
            var readAndExecuteRule = new FileSystemAccessRule(identity, FileSystemRights.ReadAndExecute, AccessControlType.Deny);
            fileSecurity.AddAccessRule(readAndExecuteRule);
            File.SetAccessControl(path, fileSecurity);
        }
    }
}