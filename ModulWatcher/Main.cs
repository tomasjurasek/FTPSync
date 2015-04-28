using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ModulWatcher
{
    public class Main
    {
        Watch w;
        //static int help = 0;
        public Main()
        {
            w = new Watch();
            w.Check();
            Timer timer = new Timer(10 * 60 * 1000);
            timer.Elapsed += OnTick;
            timer.Start();
        }
        private void OnTick(object source, ElapsedEventArgs e)
        {
            w.ServerSync();
        }
    }
}
