using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Shared
{
    public static class Settings
    {
        public static string Location { get { return GetValue("Location"); } }

        public static string FTPServer { get { return GetValue("FTPServer"); } }

        public static string Login { get { return GetValue("Login"); } }

        public static string Password { get { return GetValue("Password"); } }

        public static string ModulPath { get { return GetValue("Modul"); } }

        public static string Gmail { get { return GetValue("Gmail"); } }

        public static string GmailPassword { get { return GetValue("PasswordGMail"); } }


        private static string GetValue(string key)
        {
            try
            {
                var path = @"C:\Configuration.xml";
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = XmlReader.Create(fileStream))
                {
                    var configuration = new XmlSerializer(typeof(Configuration)).Deserialize(reader) as Configuration;

                    if (configuration == null) return null;

                    return configuration.Item.Where(s => s.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Select(s => s.Value).FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("{0}",exception.ToString()));
                return null;
            }

        }
    }
}

