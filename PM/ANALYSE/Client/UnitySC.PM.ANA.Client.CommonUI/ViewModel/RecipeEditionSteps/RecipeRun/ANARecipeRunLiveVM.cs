using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

using static System.Windows.Forms.AxHost;



namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class ANARecipeRunLiveVM : ObservableRecipient
    {
        private ANARecipeSupervisor _recipeSupervisor;
        private GlobalStatusSupervisor _globalStatusSupervisor;
        private ILogger _logger;
        private bool _isReconnectNeeded=true;
        private bool _isCameraStarted = false;

        public RecipeRunDashboardVM DashboardVM { get; set; }

        private bool _isServerReady = false;

        public bool IsServerReady
        {
            get => _isServerReady; set { if (_isServerReady != value) { _isServerReady = value; OnPropertyChanged(); } }
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private string _busyMessage;
        private bool _shouldStartCamera;

        public string BusyMessage
        {
            get => _busyMessage; set { if (_busyMessage != value) { _busyMessage = value; OnPropertyChanged(); } }
        }

        public ANARecipeRunLiveVM()
        {
            _recipeSupervisor = ServiceLocator.ANARecipeSupervisor;
            _recipeSupervisor.RecipeStartedChangedEvent += _recipeSupervisor_RecipeStartedChangedEvent;

            _globalStatusSupervisor = ServiceLocator.GlobalStatusSupervisor;
           
            _logger = ClassLocator.Default.GetInstance<ILogger>();

            DashboardVM = new RecipeRunDashboardVM(true);

            // This task is used to connect to the server and reconnect in case the server is restarted
            var task = new Task(async () =>
            {
                while (true)
                {

                    try
                    {
                        if (_isReconnectNeeded)
                        {
                            _globalStatusSupervisor.Init();
                            DoSubscribeToServerChanges();
                            _isReconnectNeeded = false;
                        }
                    }
                    catch (Exception)
                    {
                        IsServerReady = false;
                    }

                    if (!_globalStatusSupervisor.IsChannelOpened())
                    {
                        _isReconnectNeeded = true;
                        _isCameraStarted = false;
                    }

                    StartCameraIfNeeded();

                    await Task.Delay(5000);
                }
            });

            task.Start(TaskScheduler.Current);
        }

        public void StartCamera()
        {
           _shouldStartCamera= true;
        }

        public void StopCamera()
        {
            _shouldStartCamera = false;
            if (IsServerReady && _isCameraStarted)
            {
                ServiceLocator.CamerasSupervisor.GetMainCamera()?.StopStreaming();
                _isCameraStarted = false;
            }
        }

        private void DoSubscribeToServerChanges()
        {
            var serverState=_globalStatusSupervisor.GetServerState().Result;
            if ((serverState == PMGlobalStates.Free) || (serverState == PMGlobalStates.Busy))
            {
                _recipeSupervisor.SubscribeToChanges();

                IsServerReady = true;
            }
            else
            {
                throw new Exception("Server is not ready yet");
            }
        }

        private void StartCameraIfNeeded()
        {
            if (IsServerReady && _shouldStartCamera)
            {
                try
                {
                    if (!_isCameraStarted)
                    {
                        ServiceLocator.CamerasSupervisor.GetMainCamera()?.StartStreaming();
                        _isCameraStarted = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void _recipeSupervisor_RecipeStartedChangedEvent(ANARecipeWithExecContext startedRecipe)
        {
            Task.Run(() =>
            {
                var mapper = ClassLocator.Default.GetInstance<Mapper>();
                var anaRecipeVM = mapper.AutoMap.Map<ANARecipeVM>(startedRecipe.Recipe);
                var recipe = mapper.AutoMap.Map<ANARecipe>(anaRecipeVM);
                var estimatedExecutionTime = ServiceLocator.ANARecipeSupervisor.GetEstimatedTime(recipe);

                var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
                axesSupervisor.AxesVM.WaferMap = startedRecipe.Recipe.WaferMap?.WaferMapData;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    DashboardVM.ResetProgressState();
                    DashboardVM.EditedRecipe = anaRecipeVM;
                    DashboardVM.JobId = startedRecipe.JobId;
                    DashboardVM.DataflowRecipeName = startedRecipe.DFRecipeName;
                    DashboardVM.Update(estimatedExecutionTime);
                    DashboardVM.IsRecipeRunning = true;
                }));
            });
        }
    }
}
