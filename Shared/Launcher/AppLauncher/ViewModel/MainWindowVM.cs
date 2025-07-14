using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using AppLauncher.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace AppLauncher.ViewModel
{
    internal class MainWindowVM:ObservableObject
    {

        private int _checkIntervalMilliseconds = 5000; // 5 seconds
        private bool _isClosing = false;
        private CancellationTokenSource _ts = new CancellationTokenSource();

        public ObservableCollection<LauncherServiceVM> Services { get; set; }
        public ObservableCollection<LauncherApplicationVM> Applications { get; set; }

        private Thread _checkStatusThread;
        public MainWindowVM()
        {
            Services = new ObservableCollection<LauncherServiceVM>();
            Applications = new ObservableCollection<LauncherApplicationVM>();
            LoadConfiguration();

            // Set the interval at which you want to check the application status (in milliseconds)


            // Create and start the background timer
            // Create and start a new thread for the periodic status check
            _checkStatusThread = new Thread(() =>
            {
                CheckApplicationsAndServicesStatus();
            });

            // Start the thread
            _checkStatusThread.Start();

        }


        private  async void CheckApplicationsAndServicesStatus()
        {
            while (!_ts.IsCancellationRequested)
            {        
                foreach (var application in Applications)
                {
                    if(!_ts.IsCancellationRequested)
                       application.UpdateStatus();
                }

                foreach (var service in Services)
                {
                    if (!_ts.IsCancellationRequested)
                        service.UpdateStatus();
                }

                // time interval rest
                try
                {
                    CancellationToken cts = _ts.Token;
                    await Task.Delay(_checkIntervalMilliseconds, cts);
                }
                catch(AggregateException)
                {
                    _isClosing = true;
                }
                catch (Exception)
                {
                    _isClosing = true;
                }
            }

            if (_isClosing)
            {
                Debug.WriteLine("Launcher thread closing...");
            }
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Handle closing logic, set e.Cancel as needed
            _ts.Cancel();
            _isClosing = true;

            if(!_checkStatusThread.Join(_checkIntervalMilliseconds))
                _checkStatusThread.Abort();
        }

        public LauncherConfig Config { get; set; }

        private void LoadConfiguration()
        {
            string configFilePath=string.Empty;
            try
            {
                Config = new LauncherConfig();
                configFilePath=((App)Application.Current).ConfigurationFilePath;
                Config = XML.Deserialize<LauncherConfig>(configFilePath);
                foreach (var service in Config.Services)
                {
                    var serviceVM = new LauncherServiceVM(service);
                    Services.Add(serviceVM);
                }
                foreach (var application in Config.Applications)
                {
                    var applicationVM = new LauncherApplicationVM(this, application);
                    Applications.Add(applicationVM);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MWVM catch Exception : {ex.Message}");
                MessageBox.Show($"Failed to load the launcher configuration {configFilePath}", "Launcher", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current?.Shutdown();
            }
            
        }

        private RelayCommand _stopAllExecution;

        public RelayCommand StopAllExecution
        {
            get
            {
                return _stopAllExecution ?? (_stopAllExecution = new RelayCommand(
                    async () =>
                    {
                        if (MessageBox.Show("You should stop the applications directly from the applications. If you continue you could loose unsaved work. Do you want to stop the applications ?", "Launcher", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                            return;
                        foreach (var application in Applications)
                        {
                            await application.StopApplicationAsync(true);
                        }

                        // Stop the services in the reverse order
                        for (int i = Services.Count - 1; i >= 0; i--)
                        {
                            await Services[i].StopServiceExecutionAsync();
                        }

                     },
                    () => { return true; }
                ));
            }
        }
    }
}
