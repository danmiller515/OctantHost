using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace OctantApp.Tray
{
    public class IconViewModel: INotifyPropertyChanged
    {
        public IconViewModel()
        {
            App.OctantHost.PropertyChanged += OctantHost_PropertyChanged;
        }

        private void OctantHost_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsRunning")
            {
                RaisePropertyChange("Status");
                RaisePropertyChange("StatusImage");
            }
        }

        public string StatusImage
        {
            get
            {
                if (App.OctantHost.IsRunning)
                {
                    return "/Images/Online.png";
                }
                else
                {
                    return "/Images/Offline.png";
                }
            }
        }

        public string Status
        {
            get
            {
                if(App.OctantHost.IsRunning)
                {
                    return "Status: Running";
                }
                else
                {
                    return "Status: Stopped";
                }
            }
        }

        public ICommand NewOctantWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        var newMainWindow = new MainWindow();
                        newMainWindow.Show();
                    },
                    CanExecuteFunc = () => App.OctantHost.IsRunning
                };
            }
        }

        public ICommand StartOctantCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => App.OctantHost.Start(),
                    CanExecuteFunc = () => !App.OctantHost.IsRunning
                };
            }
        }

        public ICommand StopOctantCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => App.OctantHost.Stop(),
                    CanExecuteFunc = () => App.OctantHost.IsRunning
                };
            }
        }

        public ICommand RestartOctantCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => App.OctantHost.Restart(),
                    CanExecuteFunc = () => App.OctantHost.IsRunning
                };
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChange(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
