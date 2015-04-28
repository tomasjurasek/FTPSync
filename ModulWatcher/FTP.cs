using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FtpLib;
using System.IO;
using System.Net;
using Shared;
using System.Diagnostics;
using System.Threading;

namespace ModulWatcher
{
    public class FTP
    {
        /// <summary>
        /// Metoda projde soubory na lokalu a porovna se serverem a nahraje novejsi
        /// </summary>
        /// <param name="filePathObj"></param>
        public void SyncFromLocal(object filePathObj)
        {
            string filePath = (string)filePathObj;
            string path = Helpers.GetPathDirectory(Settings.Location, filePath);

            FileSystemEventArgs file = new FileSystemEventArgs(WatcherChangeTypes.All, Settings.Location, path);

            if (CheckIfFileExistsOnFtp(path))
            {
                DateTime timeFTP = GetLastModifiedFileonFTP(path);
                DateTime timeLocal = GetLastModifiedFileOnLocal(filePath);

                if (timeLocal > timeFTP)
                    UploadFile(file);
                else
                    DownloadFile(file);
            }
            else
                UploadFile(file);

        }

        /// <summary>
        /// Metoda zjisti soubory na serveru a vrati list souboru
        /// </summary>
        public void SyncFromFTP()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Settings.FTPServer);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(Settings.Login, Settings.Password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            using(Stream responseStream = response.GetResponseStream())
            { 
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    List<string> list = new List<string>();
                    string name = string.Empty;
                    //REFAKTOR
                    while ((name = reader.ReadLine()) != null)
                    {

                        list.Add(name);
                    }

                    Sync(list);
                }
            }
        }

        /// <summary>
        /// Metoda stahne soubory ze serveru na lokal pokud chyby a nebo nejsou aktualni
        /// </summary>
        /// <param name="listFile"></param>
        public void Sync(List<string> listFile)
        {
            //Vlakna dodelat
            foreach (string item in GetFiles(listFile))
            {
                Thread vlakno = new Thread(new ParameterizedThreadStart(SyncServer));
                vlakno.Start(item);
                vlakno.Join();
            }

        
        }

        private void SyncServer(object obj)
        {
            string item = (string)obj;
            FileSystemEventArgs file = new FileSystemEventArgs(WatcherChangeTypes.All, Shared.Settings.Location, item);

            //Pokud server je na lokalu, tak se overi cas upravy
            //if (CheckIfFileExistOnLocal(item))
            //{
            //    DateTime lokalFile = GetLastModifiedFileOnLocal(item);
            //    DateTime serverFile = GetLastModifiedFileonFTP(item);

            //    if (lokalFile > serverFile)
            //        UploadFile(file);
            //    else
            //        DownloadFile(file);
            //}
            //else
                DownloadFile(file);
        }

        /// <summary>
        /// Metoda dostane list a vrati list jen se soubory
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<string> GetFiles(List<string> list)
        {
            List<string> result = new List<string>();
            result = list.Where(s => s.Contains('.')).ToList();

            return result;
        }

        /// <summary>
        /// Metoda nahraje soubor na server
        /// </summary>
        /// <param name="obj"></param>
        public void UploadFile(object obj)
        {
            FileSystemEventArgs file = (FileSystemEventArgs)obj;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Settings.FTPServer + file.Name);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(Settings.Login, Settings.Password);
            try
            {


                byte[] fileContents = File.ReadAllBytes(file.FullPath);
                request.ContentLength = fileContents.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { }
                Trace.WriteLine(string.Format("{0} byl nahran", file.Name));
            }
            catch(Exception ex)
            {
                Trace.WriteLine(string.Format("{0}", ex));
            }

            
        }

        /// <summary>
        /// Smaze soubor na serveru
        /// </summary>
        /// <param name="obj"></param>
        public void DeleteFile(object obj)
        {
            FileSystemEventArgs file = (FileSystemEventArgs)obj;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Settings.FTPServer + file.Name);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(Settings.Login, Settings.Password);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { }
            Trace.WriteLine(string.Format("{0} byl smazan", file.Name));
        }

        /// <summary>
        /// Zmena nazvu souboru
        /// </summary>
        /// <param name="obj"></param>
        public void RenameFile(object obj)
        {
            RenamedEventArgs file = (RenamedEventArgs)obj;
           
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Settings.FTPServer + file.OldName);
            request.Method = WebRequestMethods.Ftp.Rename;
            request.RenameTo = file.Name;
            request.Credentials = new NetworkCredential(Settings.Login, Settings.Password);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { }
            
        }

        /// <summary>
        /// Metoda stahne soubor ze serveru
        /// </summary>
        /// <param name="file"></param>
        public void DownloadFile(FileSystemEventArgs file)
        {

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Settings.FTPServer + file.Name);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(Settings.Login, Settings.Password);

            try
            {

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            using (StreamWriter outfile = new StreamWriter(Shared.Settings.Location + @"\" + file.Name + ""))
                            {
                                outfile.Write(reader.ReadToEnd());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0}", ex));
            }
        }

        /// <summary>
        /// Metoda zjisti posledni modifikaci souboru na serveru
        /// </summary>
        /// <param name="pathFile"></param>
        /// <returns></returns>
        public DateTime GetLastModifiedFileonFTP(string pathFile)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Settings.FTPServer + pathFile);
            request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            request.Credentials = new NetworkCredential(Settings.Login, Settings.Password);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                return response.LastModified;
            }

        }

        /// <summary>
        /// Metoda zjisti posledni modifikaci na lokalu
        /// </summary>
        /// <param name="pathFile"></param>
        /// <returns></returns>
        public DateTime GetLastModifiedFileOnLocal(string pathFile)
        {
            FileInfo info = new FileInfo(pathFile);
            return info.LastWriteTime;
        }

        /// <summary>
        /// Metoda zjisti zda soubor existuje na lokalu
        /// </summary>
        /// <param name="pathFile"></param>
        /// <returns></returns>
        public bool CheckIfFileExistOnLocal(string pathFile)
        {
            bool isExist = true;
            try
            {
                FileInfo info = new FileInfo(pathFile);
            }
            catch(Exception ex)
            {
                Trace.WriteLine(string.Format("{0}", ex));
                isExist = false;
            }

            return isExist;
        }

        /// <summary>
        /// Metoda zjisti zda soubor existuje na serveru
        /// </summary>
        /// <param name="pathFile"></param>
        /// <returns></returns>
        public bool CheckIfFileExistsOnFtp(string pathFile)
        {

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Settings.FTPServer + pathFile);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.Credentials = new NetworkCredential(Settings.Login, Settings.Password);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { };

            }
            catch (WebException ex)
            {
                Trace.WriteLine(string.Format("{0}", ex));
                using (FtpWebResponse response = (FtpWebResponse)ex.Response)
                {
                    if (response.StatusCode ==
                        FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Overi zda je pripojeni na server
        /// </summary>
        /// <returns></returns>
        public bool CheckConnectiontoFTP(string Server, string Login, string Password)
        {
            Uri url = new Uri(Server);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.Credentials = new NetworkCredential(Login, Password);
            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { };
                return true;
            }
            catch(Exception ex)
            {
                Trace.WriteLine(string.Format("{0}", ex));
                return false;
            }
        }
    }
}
