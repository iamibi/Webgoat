using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using OWASP.WebGoat.NET.App_Code.Hashing_Utility;

namespace OWASP.WebGoat.NET.App_Code.DB
{
    public class DbMigrationScript
    {
        public void MigrateMySqlDb()
        {
            try
            {
                string filepath = "C:\\Users\\student\\source\\repos\\DatabaseMigration\\customerlogin.txt";
                StreamReader sr = new StreamReader(filepath);
                StreamWriter sw = new StreamWriter("C:\\Users\\student\\source\\repos\\DatabaseMigration\\new_customerlogin.txt");
                string line = sr.ReadLine();

                while (line != null)
                {
                    List<string> splitLineStrings = new List<string>(line.Split('|'));
                    string password = splitLineStrings[2];
                    string decodedPassword = Encoder.Decode(password);
                    var hash = HashingAlgo.GetHashDigestWithSalt(decodedPassword);
                    splitLineStrings[2] = hash.Digest;
                    splitLineStrings.Add(hash.Salt);
                    sw.WriteLine(string.Join("|", splitLineStrings));
                    line = sr.ReadLine();
                }
                Console.WriteLine("Completed");

                sr.Close();
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred", ex);
            }
        }
    }
}