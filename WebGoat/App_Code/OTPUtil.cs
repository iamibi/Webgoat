using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using OtpNet;

namespace OWASP.WebGoat.NET.App_Code
{
    public class OTPUtil
    {
        private static Totp OTP = new Totp(Encoding.ASCII.GetBytes("Shared!secre1"), mode: OtpHashMode.Sha512);

        public static Totp Get()
        {
            return OTP;
        }
    }
}