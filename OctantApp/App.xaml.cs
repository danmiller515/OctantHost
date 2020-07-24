using Hardcodet.Wpf.TaskbarNotification;
using OctantApp.Tray;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OctantApp
{
    public partial class App : Application
    {
        private TaskbarIcon tb;
        public static OctantHost OctantHost = new OctantHost();

        Mutex mutex;
        [DllImport("user32", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string cls, string win);

        [DllImport("user32")]
        static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            mutex = new Mutex(true, "{2df6606b-3aef-4be5-9d1b-2a9c9b6296f2}",
            out bool isNewInstance);
            if (isNewInstance)
            {
                tb = (TaskbarIcon)FindResource("NotifyIcon");
                OctantHost.Start();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (OctantHost.IsRunning)
            {
                OctantHost.Stop();
            }

            while(OctantHost.IsRunning)
            {
                Task.Delay(100).Wait();
            }
            tb.Dispose();
        }
    }
}
