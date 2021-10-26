using System;
using System.Diagnostics;
using log4net;
using System.Reflection;
using System.IO;
using System.Threading;
using OWASP.WebGoat.NET.App_Code;
using OtpNet;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OWASP.WebGoat.NET.App_Code
{
    public class Util
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Totp OTP = new Totp(Encoding.ASCII.GetBytes(GetSecret()), mode: OtpHashMode.Sha512, step: 60);

        public static int RunProcessWithInput(string cmd, string args, string input)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = Settings.RootDir,
                FileName = cmd,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            using (Process process = new Process())
            {
                process.EnableRaisingEvents = true;
                process.StartInfo = startInfo;

                process.OutputDataReceived += (sender, e) => {
                    if (e.Data != null)
                        log.Info(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        log.Error(e.Data);
                };

                AutoResetEvent are = new AutoResetEvent(false);

                process.Exited += (sender, e) => 
                {
                    Thread.Sleep(1000);
                    are.Set();
                    log.Info("Process exited");

                };

                process.Start();

                using (StreamReader reader = new StreamReader(new FileStream(input, FileMode.Open)))
                {
                    string line;
                    string replaced;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                            replaced = line.Replace("DB_Scripts/datafiles/", "DB_Scripts\\\\datafiles\\\\");
                        else
                            replaced = line;

                        log.Debug("Line: " + replaced);

                        process.StandardInput.WriteLine(replaced);
                    }
                }
    
                process.StandardInput.Close();
    

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
    
                //NOTE: Looks like we have a mono bug: https://bugzilla.xamarin.com/show_bug.cgi?id=6291
                //have a wait time for now.
                
                are.WaitOne(10 * 1000);

                if (process.HasExited)
                    return process.ExitCode;
                else //WTF? Should have exited dammit!
                {
                    process.Kill();
                    return 1;
                }
            }
        }

        public static string GetTOTPCode()
        {
            return OTP.ComputeTotp(DateTime.UtcNow);
        }

        public static bool VerifyCode(string code)
        {
            long timeStep;
            bool valid = OTP.VerifyTotp(code, out timeStep, VerificationWindow.RfcSpecifiedNetworkDelay);
            return valid;
        }

        private static string GetSecret()
        {
            try
            {
                string json = File.ReadAllText("C:\\Users\\student\\Workspace\\SendEmailCredentials\\credentials.json");
                dynamic creds = JObject.Parse(json);
                return creds.shared_secret;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}

