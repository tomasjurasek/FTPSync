using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class Helpers
    {
       public static void SendMail()
        {
            var client = new SmtpClient("smtp.gmail.com", 587); // udaje na server
            {
                client.Credentials = new NetworkCredential(Settings.Gmail, Settings.GmailPassword); // prihlasovaci udaje na smtp gmailu
                client.EnableSsl = true;

            };

            try
            {
                client.Send(Settings.Gmail, Settings.Gmail,"Zastavení service", "Service byla zastavena"); // odeslani
            }

            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0}", ex.ToString()));
            }
        }
    }
}
