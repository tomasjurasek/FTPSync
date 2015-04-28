using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModulWatcher
{
    public static class Helpers
    {
        /// <summary>
        /// Zjisti pomoci cesty a cesty adresare Nazev souboru
        /// </summary>
        /// <param name="originalPath"></param>
        /// <param name="pathFile"></param>
        /// <returns></returns>
        public static string GetPathDirectory(string originalPath, string pathFile)
        {
            int a = originalPath.Length;
            int b = pathFile.Length;
            string path = pathFile.Substring(a+1,b-a-1);
            return path;

        }
        /// <summary>
        /// Zjisti zda se jedna o soubor nebo slozku
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsPathDirectory(FileSystemEventArgs e)
        {
            return !e.FullPath.Contains('.');
        }

    }
}
