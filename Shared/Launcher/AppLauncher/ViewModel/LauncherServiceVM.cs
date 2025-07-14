using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppLauncher.ViewModel
{
    internal class LauncherServiceVM : ObservableObject
    {
        public LauncherServiceVM(LauncherServiceConfig config)
        {
            Config = config;

            if (Config.IsConsoleMode)
            {
                // The service is a console application
                Icon = Helpers.GetApplicationIcon(Config.Path);
                Version = Helpers.GetApplicationVersion(Config.Path);
            }
            else
            {
                // The service is a windows service
                Icon = Helpers.GetServiceIcon(Config.ServiceName);
                Version = Helpers.GetServiceVersion(Config.ServiceName);
            }
        }

        public LauncherServiceConfig Config { get; set; }

        private ExecutionStatus _status = ExecutionStatus.Stopped;
        public ExecutionStatus Status
        {
            get => _status;
            set
            {
                SetProperty(ref _status, value);
                StartExecution.NotifyCanExecuteChanged();
                StopExecution.NotifyCanExecuteChanged();
            }
        }

        private ImageSource _icon = null;

        public ImageSource Icon
        {
            get => _icon; set => SetProperty(ref _icon, value);
        }


        private string _version = null;

        public string Version
        {
            get => _version; set => SetProperty(ref _version, value);
        }

        private RelayCommand _startExecution;

        public RelayCommand StartExecution
        {
            get
            {
                return _startExecution ?? (_startExecution = new RelayCommand(
                    async () =>
                    {
                        await StartServiceExecutionAsync(true);
                    },
                    canExecute: () => { return Status != ExecutionStatus.Running && Status != ExecutionStatus.Starting && Status != ExecutionStatus.Stopping; }
                ));
            }
        }


        private RelayCommand _stopExecution;

        public RelayCommand StopExecution
        {
            get
            {
                return _stopExecution ?? (_stopExecution = new RelayCommand(
                    async () =>
                    {
                        await StopServiceExecutionAsync();
                    },
                    () => { return Status == ExecutionStatus.Running; }
                ));
            }
        }


        public async Task StartServiceExecutionAsync(bool applyDelay)
        {
            if (GetExecutionStatus() == ExecutionStatus.Running)
                return;
            Status = ExecutionStatus.Starting;

            try
            {
                if (Config.IsConsoleMode)
                {
                    // The service is a console application
                    await Task.Run(() => Helpers.StartApplication(Config.Path, Config.Arguments, !Config.ShowConsoleWindow));

                }
                else
                {
                    // The service is a windows service
                    await Task.Run(() => Helpers.StartWindowsService(Config.ServiceName));
                }
                if (applyDelay)
                    await Task.Delay(Config.DelayBeforeLaunchingNextService * 1000);
                Status = ExecutionStatus.Running;
            }
            catch (Exception)
            {
                Status = ExecutionStatus.Stopped;
                MessageBox.Show($"Failed to start the service {Config.Name}", "Launcher", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task StopServiceExecutionAsync()
        {
            if (Status == ExecutionStatus.Stopped)
                return;

            Status = ExecutionStatus.Stopping;
            try
            {
                if (Config.IsConsoleMode)
                {
                    // The service is a console application
                    await Task.Run(() => Helpers.StopApplication(Path.GetFileNameWithoutExtension(Config.Path)));

                }
                else
                {
                    // The service is a windows service
                    await Task.Run(() => Helpers.StopWindowsService(Config.ServiceName));
                }
                Status = ExecutionStatus.Stopped;

            }
            catch (Exception)
            {
                MessageBox.Show($"Failed to stop the service {Config.Name}", "Launcher", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal void UpdateStatus()
        {
            if (Status == ExecutionStatus.Starting || Status == ExecutionStatus.Stopping)
                return;

            ExecutionStatus newStatus;

            newStatus = GetExecutionStatus();

            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                Status = newStatus;
            }));

        }

        private ExecutionStatus GetExecutionStatus()
        {
            ExecutionStatus newStatus;
            if (Config.IsConsoleMode)
            {
                // The service is a console application
                newStatus = Helpers.GetApplicationRunningStatus(Path.GetFileNameWithoutExtension(Config.Path));
            }
            else
            {
                newStatus = Helpers.GetServiceRunningStatus(Config.ServiceName);
            }

            return newStatus;
        }
    }


}
