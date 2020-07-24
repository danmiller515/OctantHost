using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;

namespace OctantApp.Tray
{
    public class OctantHost: INotifyPropertyChanged
    {
        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);
        delegate bool ConsoleCtrlDelegate(uint CtrlType);

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _ProcessingState;
        private State _desiredState;
        private Timer _watchdogTimer;
        private Process _process;

        public OctantHost()
        {
            _watchdogTimer = new Timer(2000);
            _watchdogTimer.Elapsed += OnWatchdogCheckEvent;
            _watchdogTimer.AutoReset = true;
            _watchdogTimer.Enabled = true;
        }

        public void Start()
        {
            _desiredState = State.Running;
        }

        private void OnWatchdogCheckEvent(Object source, ElapsedEventArgs e)
        {
            if(_ProcessingState == false)
            {
                _ProcessingState = true;

                if (_desiredState == State.Running && !IsRunning)
                {
                    StartProcess();
                    RaisePropertyChange("IsRunning");
                }
                else if (_desiredState == State.Stopped && IsRunning)
                {
                    StopProcess();
                    RaisePropertyChange("IsRunning");
                }

                _ProcessingState = false;
            }
        }

        private void StartProcess()
        {
            try
            {
                var rootFolder = Directory.GetCurrentDirectory();

                _process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "octant\\octant.exe",
                        WorkingDirectory = rootFolder,
                        Arguments = "-v --disable-open-browser",
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                _process.Start();
            }
            catch
            {

            }
        }

        private void StopProcess()
        {
            try
            {
                _process.StandardInput.Close();

                if (AttachConsole((uint)_process.Id))
                {
                    SetConsoleCtrlHandler(null, true);
                    try
                    {
                        if (!GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0))
                            return;
                        _process.WaitForExit();
                    }
                    finally
                    {
                        FreeConsole();
                        SetConsoleCtrlHandler(null, false);
                    }
                    return;
                }

                _process.WaitForExit();
                _process.Close();
                _process.Kill();
                _process.Dispose();
            }
            catch
            {

            }
        }

        public void RaisePropertyChange(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        internal const int CTRL_C_EVENT = 0;

        public void Stop()
        {
            _desiredState = State.Stopped;
        }

        public void Restart()
        {
            if (_process != null && _process.HasExited == false)
            {
                Stop();
            }

            Start();
        }

        public bool IsRunning
        {
            get
            {
                if (_process != null)
                {
                    try
                    {
                        if(!_process.HasExited)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                return false;
            }
        }
    }
}
