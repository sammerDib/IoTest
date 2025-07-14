using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.Shared.Data;

namespace AppLauncher.ViewModel
{
    internal class LauncherApplicationVM: ObservableObject
    {
        public LauncherApplicationConfig Config { get; set; }

 
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
                        Status = ExecutionStatus.Starting;
                        await StartApplicationAndDependenciesAsync();
                        Status = ExecutionStatus.Running;
                        
                    },
                    canExecute: () => { return Status!= ExecutionStatus.Running && Status != ExecutionStatus.Starting; }
                ));
            }
        }


        private RelayCommand _stopExecution;
        private MainWindowVM _mainWindowVM;

        public LauncherApplicationVM(MainWindowVM mainWindowVM, LauncherApplicationConfig config)
        {
            _mainWindowVM = mainWindowVM;
            Config = config;

            Icon=Helpers.GetApplicationIcon(Config.Path);
            Version=Helpers.GetApplicationVersion(Config.Path);
        }

        public RelayCommand StopExecution
        {
            get
            {
                return _stopExecution ?? (_stopExecution = new RelayCommand(
                    async () =>
                    {
                       await StopApplicationAsync();

                    },
                    () => { return Status == ExecutionStatus.Running; }
                ));
            }
        }

        public async Task StopApplicationAsync( bool silent=false)
        {
            if (Status == ExecutionStatus.Stopped) 
                return; 
            try
            { 
                if (!silent) 
                { 
                    if (MessageBox.Show("You should stop the application directly from the application. If you continue you could loose unsaved work. Do you want to stop the application ?", "Launcher", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.No)
                     return;
                }
                await Task.Run(() => Helpers.StopApplication(Path.GetFileNameWithoutExtension(Config.Path)));
                // Code to execute
                Status = ExecutionStatus.Stopped;

            }
            catch (Exception)
            {
                MessageBox.Show($"Failed to stop the application {Config.Name}", "Launcher", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task StartApplicationAndDependenciesAsync()
        {
            foreach (var serviceDependencyName in Config.ServiceDependencies)
            {
                var service = _mainWindowVM.Services.FirstOrDefault(s => s.Config.Name == serviceDependencyName);
                if (service != null)
                {
                    await service.StartServiceExecutionAsync(true);
                }
                else
                {
                    MessageBox.Show($"The application {Config.Name} depends on the service {serviceDependencyName} but it is not configured", "Launcher", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // We start the application itself after all the dependencies
            try
            {
                await Task.Run(() => Helpers.StartApplication(Config.Path, Config.Arguments, false));
            }
            catch (Exception)
            {

   
                MessageBox.Show($"Failed to start the Application {Config.Name}", "Launcher", MessageBoxButton.OK, MessageBoxImage.Error);
    
            }
        }

        internal void UpdateStatus()
        {
            if (Status == ExecutionStatus.Starting)
                return;

            ExecutionStatus newStatus;

            newStatus = Helpers.GetApplicationRunningStatus(Path.GetFileNameWithoutExtension(Config.Path));

            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                Status = newStatus;
            }));
        }
    }
}
