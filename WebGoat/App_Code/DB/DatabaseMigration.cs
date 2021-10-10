using System;
using System.Collections.Generic;
using System.IO;

namespace OWASP.WebGoat.NET.App_Code.DB
{
    public class DatabaseMigration
    {
        public static void HashUsingPBKDF2()
        {
            try
            {
                string filepath = "C:\\Users\\student\\Workspace\\webgoat\\WebGoat\\DB_Scripts\\datafiles\\customerlogin.txt";
                StreamReader sr = new StreamReader(filepath);
                StreamWriter sw = new StreamWriter("C:\\Users\\student\\Workspace\\webgoat\\WebGoat\\DB_Scripts\\datafiles\\new_customerlogin.txt");

                string line = sr.ReadLine();
                int processedUsers = 1;

                while (line != null)
                {
                    List<string> splitLineStrings = new List<string>(line.Split('|'));
                    string password = splitLineStrings[2];
                    string decodedPassword = Encoder.Decode(password);
                    HashingAlgorithm hash = new HashingAlgorithm(decodedPassword);
                    splitLineStrings[2] = hash.Digest;
                    splitLineStrings.Add(hash.Salt);
                    sw.WriteLine(String.Join("|", splitLineStrings));
                    line = sr.ReadLine();
                    processedUsers++;
                }
                Console.WriteLine("Completed users: " + processedUsers);

                sr.Close();
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Occurred: " + ex);
            }
        }
    }
}