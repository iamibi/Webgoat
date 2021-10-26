using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using OtpNet;

namespace OWASP.WebGoat.NET.App_Code
{
    public class OTPUtil
    {
        private static Totp OTP = new Totp(Encoding.ASCII.GetBytes(GetSecret()), mode: OtpHashMode.Sha512);

        public static Totp Get()
        {
            return OTP;
        }

        private static string GetSecret()
        {
            try
            {
                string json = File.ReadAllText("C:\\Users\\student\\Workspace\\SendEmailCredentials\\credentials.json");
                dynamic creds = JObject.Parse(json);
                return creds.shared_secret;
            }
            catch
            { }
            return null;
        }
    }
}