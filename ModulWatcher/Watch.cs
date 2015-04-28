using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModulWatcher
{
    public class Watch
    {
        FTP ftp = new FTP();
        Mutex mutex = new Mutex();
        List<Thread> listVlaken = new List<Thread>();
        string path = Shared.Settings.Location;

        FileSystemWatcher watcher;

        

        public Watch()
        {

        }

        /// <summary>
        /// Nastavi hlidani slozky
        /// </summary>
        /// <param name="path"></param>
        public void Check()
        {
            watcher = new FileSystemWatcher();

            
            //Musi brat lokaci z xml
            watcher.Path = path;// Shared.Settings.Location;

            
            //Nastavi priznaky
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
            | NotifyFilters.FileName | NotifyFilters.DirectoryName |NotifyFilters.Attributes;

            // Typy souboru
            watcher.Filter = "*";

            //Pridane akce na eventy
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnDeleted;
            //watcher.Renamed += OnChanged;


            //Zacne sledovat
            watcher.EnableRaisingEvents = true;

            FirstCheckLokal();
            //Jen v debug modu
            //while (true);
        }

        /// <summary>
        /// Metoda se spusti pri zapnuti sluzby a udela zakladni synchronizaci
        /// </summary>
        /// <param name="targetDirectory"></param>
        public void FirstCheckLokal()
        {
            string targetDirectory = path;
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            watcher.EnableRaisingEvents = false;
            foreach (string fileName in fileEntries)
            {
               
                Thread vlakno = new Thread(new ParameterizedThreadStart(ftp.SyncFromLocal));
                vlakno.Start(fileName);
                //vlakno.Join();
                listVlaken.Add(vlakno);


            }
            //Pocka az se vykonaji vsechny vlakna
            foreach (var item in listVlaken)
            {
                item.Join();
            }
            //Potom zapne opacnou synchonizaci
            
            ftp.SyncFromFTP();
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Sync se serveru na timer
        /// </summary>
        public void ServerSync()
        {
            watcher.EnableRaisingEvents = false;
            ftp.SyncFromFTP();
            watcher.EnableRaisingEvents = true;
        }


        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Thread vlakno = new Thread(new ParameterizedThreadStart(ftp.UploadFile));
            vlakno.Start(e);
            vlakno.Join();
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            Thread vlakno = new Thread(new ParameterizedThreadStart(ftp.DeleteFile));
            vlakno.Start(e);
            vlakno.Join();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            //mutex.WaitOne();
            //Thread vlakno = new Thread(new ParameterizedThreadStart(ftp.RenameFile));
            //vlakno.Start(e);
            //vlakno.Join();
            //mutex.ReleaseMutex();
        }
        
    }
}
