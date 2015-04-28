using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AT.NET_FTPSync
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        //Pokud testujeme servisu v debugu
        public void OnDebug()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                Assembly assem = Assembly.LoadFile(Shared.Settings.ModulPath + @"\ModulWatcher.dll");
                Type mat = assem.GetType("ModulWatcher.Main");

                Object matematika = Activator.CreateInstance(mat);

            }
            catch (Exception ex)
            {

                //File.WriteAllText(@"C:\Users\Tomáš\Documents\Visual Studio 2013\Projects\AT.NET-FTPSync\ModulWatcher\bin\Debug\text.txt", ex.ToString());
                Trace.WriteLine(string.Format(" OnStart: {0}", ex));
            }
        }
        protected override void OnStop()
        {
            Shared.Helpers.SendMail();
        }
    }
}
