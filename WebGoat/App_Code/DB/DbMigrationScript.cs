// DbMigrationScript.cs
// 
// This script was written with the intention of replacing the entries in the entire
// database. It replaces the file "customerlogin.txt" entries with a more secure hash
// and a strong salt per customer. After the file is replaced, we use the existing
// mechanism of "Rebuild Database" which essentially reads from the above file. Before
// that, we make sure that we change the schema in the "create_webgoatcoins.sql" and 
// "create_webgoatcoins_sqplite3.sql" to update the password and add salt column. Once
// the DB is rebuilt, it will pick up the new hashes.

using System;
using System.Collections.Generic;
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
                string filepath = "C:\\Users\\student\\Workspace\\webgoat\\WebGoat\\DB_Scripts\\datafiles";
                StreamReader sr = new StreamReader(filepath + "\\customerlogin.txt");
                StreamWriter sw = new StreamWriter(filepath + "\\new_customerlogin.txt");
                string line = sr.ReadLine();

                while (line != null)
                {
                    // Split the line with '|' as the separator.
                    List<string> splitLineStrings = new List<string>(line.Split('|'));

                    // The 3rd entry is the currently stored encoded password.
                    string password = splitLineStrings[2];
                    string decodedPassword = Encoder.Decode(password);

                    // Pass the plaintext password to the Hashing algorithm and get the digest hash.
                    var hash = HashingAlgo.GetHashDigestWithSalt(decodedPassword);

                    // Replace the second entry, which was encoded password with the new Hash Digest and append Salt at the end of the list.
                    splitLineStrings[2] = hash.Digest;
                    splitLineStrings.Add(hash.Salt);

                    // Write this array as a new line of string.
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