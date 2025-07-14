using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;

using AcquisitionAdcExchange;

using ADC.Model;
using ADC.ViewModel.Graph;
using ADC.ViewModel.Operator;

using ADCEngine;

using AdcTools;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Win32;

using Serilog;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADC.ViewModel
{
    /// <summary>
    /// The view-model for the Operator view
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class OperatorViewModel : ObservableRecipient
    {
        private IAdcExecutor _adcDistantExecutor;
        private IAdcExecutor _adcLocalExecutor = new AdcExecutor();
        private ChannelFactory<IAdcExecutor> _adcExecutorChannelFactory;
        private RecipeId _recipeId;
        private ServiceHost host;
        private const string AdaToAdcWindowsServiceName = "AdaToAdc";
        private ServiceControllerStatus? _adaToAdcWindowsServiceStatus;

        private IAdcExecutor AdcExecutor
        {
            get
            {
                if (_adcDistantExecutor == null)
                {
                    // Connection au service WCF
                    if (_adcExecutorChannelFactory == null)
                    {
                        _adcExecutorChannelFactory = new ChannelFactory<IAdcExecutor>("IAdcExecutor");
                        ADCEngine.ADC.log("Connecting to service on \"" + _adcExecutorChannelFactory.Endpoint.Address + "\"");
                    }

                    _adcDistantExecutor = _adcExecutorChannelFactory.CreateChannel();
                    ((ICommunicationObject)_adcDistantExecutor).Faulted += AdcExecutor_Faulted;
                }
                return _adcDistantExecutor;
            }
        }

        #region Internal Data Members

        private RecipeGraphViewModel _recipeGraphVM;
        public RecipeGraphViewModel RecipeGraphVM
        {
            get { return _recipeGraphVM; }
        }

        private WaferInfoViewModel _waferInfoVM;
        public WaferInfoViewModel WaferInfoVM
        {
            get { return _waferInfoVM; }
        }

        private bool _isRecipeRunning = false;
        public bool IsRecipeRunning
        {
            get { return _isRecipeRunning; }
            set
            {
                if (_isRecipeRunning == value)
                    return;
                _isRecipeRunning = value;
                OnPropertyChanged();
            }
        }

        public Recipe Recipe { get { return ServiceRecipe.Instance().RecipeCurrent; } }

        /// <summary>
        /// Timer used in running to display information from modules 
        /// </summary>
        private DispatcherTimer _timerOperatorMode;


        /// <summary>
        /// Défini si on utilise une recette local dans le mode opérateur 
        /// </summary>
        private bool _localRecipeImprovementIsEnabled;
        public bool LocalRecipeImprovementIsEnabled
        {
            get => _localRecipeImprovementIsEnabled;
            set
            {
                if (_localRecipeImprovementIsEnabled != value)
                {
                    _localRecipeImprovementIsEnabled = value;
                    OnPropertyChanged();
                    if (!_localRecipeImprovementIsEnabled)
                        LocalRecipeImprovementPath = null;
                }
            }
        }

        /// <summary>
        /// Chemin local de la recette pour le mode opérateur
        /// </summary>
        private string _localRecipeImprovementPath;
        public string LocalRecipeImprovementPath
        {
            get => _localRecipeImprovementPath;
            set
            {
                _localRecipeImprovementPath = value;
                OnPropertyChanged();
                try
                {
                    AdcExecutor.SetLocalRecipePath(_localRecipeImprovementPath);
                }
                catch (Exception ex)
                {
                    Log.Debug("SetLocalRecipePath error" + ex.ToString());
                }
            }
        }

        #endregion Internal Data Members


        /// <summary>
        /// Timer pour le mode Opérateur
        /// </summary>
        private void timerOperatorMode_Tick(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
               {
                   try
                   {
                       if (!HideAdaToAdcWindowsServiceStatus)
                           RefreshAdaToAdcWindowsServiceState();
                       RefreshStatsInOperatorMode();
                       Application.Current.Dispatcher.Invoke(() =>
                       {
                           IsConnectedToAdcEngine = true;
                           _timerOperatorMode.Interval = new TimeSpan(0, 0, 0, seconds: 1);
                       });
                   }
                   catch (System.TimeoutException)
                   {
                   }
                   catch (CommunicationException)
                   {
                   }
               });

        }

        private void AdcExecutor_Faulted(object sender, EventArgs e)
        {
            if (sender != _adcDistantExecutor)
                throw new ApplicationException("WCF fault on unkown object: " + sender);

            Application.Current.Dispatcher.Invoke(() =>
            {
                IsConnectedToAdcEngine = false;
            });
            _timerOperatorMode.Interval = new TimeSpan(0, 0, 0, seconds: 5);

            ICommunicationObject com = (ICommunicationObject)sender;
            com.Faulted -= AdcExecutor_Faulted;
            com.Abort();
            _adcDistantExecutor = null;
        }

        private bool _isConnectedToAdcEngined;
        public bool IsConnectedToAdcEngine
        {
            get { return _isConnectedToAdcEngined; }
            set
            {
                if (value == _isConnectedToAdcEngined)
                    return;

                _isConnectedToAdcEngined = value;
                if (!_isConnectedToAdcEngined)
                {
                    CloseRecipe();
                    _waferInfoVM?.ClearInfos();
                    LocalRecipeImprovementIsEnabled = false;
                }

                OnPropertyChanged();
            }
        }

        #region AdaToAdcService

        public bool HideAdaToAdcWindowsServiceStatus { get; private set; }

        private bool _isAdaToAdcServiceWindowsRunning;
        public bool IsAdaToAdcServiceWindowsRunning
        {
            get => _isAdaToAdcServiceWindowsRunning; set { if (_isAdaToAdcServiceWindowsRunning != value) { _isAdaToAdcServiceWindowsRunning = value; OnPropertyChanged(); } }
        }

        private void RefreshAdaToAdcWindowsServiceState()
        {
            _adaToAdcWindowsServiceStatus = GetServiceState();
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsAdaToAdcServiceWindowsRunning = _adaToAdcWindowsServiceStatus.HasValue ? _adaToAdcWindowsServiceStatus.Value == ServiceControllerStatus.Running : false;
                if (!IsAdaToAdcServiceWindowsRunning)
                    AdaToAdcServiceStatus = _adaToAdcWindowsServiceStatus.HasValue ? _adaToAdcWindowsServiceStatus.ToString() : "Not Installed";
                else
                    AdaToAdcServiceStatus = "Running...";
                AdaToAdcWindowsServiceCommand.NotifyCanExecuteChanged();
            });
        }

        private string _adaToAdcserviceStatus;
        public string AdaToAdcServiceStatus
        {
            get => _adaToAdcserviceStatus; set { if (_adaToAdcserviceStatus != value) { _adaToAdcserviceStatus = value; OnPropertyChanged(); } }
        }

        private ServiceControllerStatus? GetServiceState()
        {
            try
            {
                using (ServiceController sc = new ServiceController(AdaToAdcWindowsServiceName))
                {
                    return sc.Status;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Get service state error" + ex.ToString());
                return null;
            }
        }

        private void StartAdaToAdcWindowsService()
        {
            try
            {
                using (ServiceController sc = new ServiceController(AdaToAdcWindowsServiceName))
                {
                    if (sc.Status == ServiceControllerStatus.Paused || sc.Status == ServiceControllerStatus.Stopped)
                    {
                        sc.Start();
                        RefreshAdaToAdcWindowsServiceState();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Start service error" + ex.ToString());
            }
        }


        private void StopAdaToAdcWindowsService()
        {
            try
            {
                using (ServiceController sc = new ServiceController(AdaToAdcWindowsServiceName))
                {
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        sc.Stop();
                        RefreshAdaToAdcWindowsServiceState();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Stop service error" + ex.ToString());
            }
        }

        #endregion


        private void RefreshStatsInOperatorMode()
        {
            // Récupération des stat de la recette actuelle
            if (_recipeId != null)
            {
                RecipeStat rstat = AdcExecutor.GetRecipeStat(_recipeId);
                if (rstat != null)
                {
                    RefreshNodeStatistics(rstat);
                    return;
                }
                _recipeId = null;
            }

            // Récupération d'un Recipe ID
            _recipeId = AdcExecutor.GetCurrentRecipeId();
            if (_recipeId == null)
                return;

            // Récupération des infos du wafer
            WaferInfo waferInfo = AdcExecutor.GetCurrentWaferInfo();
            if (waferInfo != null)
            {
                if (_localRecipeImprovementIsEnabled)
                {
                    waferInfo.dico[eWaferInfo.ADCRecipeFileName] = LocalRecipeImprovementPath;
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _waferInfoVM.UpdateWafer(waferInfo);
                });
            }

            // Récupération de la recette
            byte[] data = AdcExecutor.GetRecipe(_recipeId);
            if (data == null)
            {
                _recipeId = null;
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                using (Stream stream = new MemoryStream(data))
                {
                    try
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load(stream);
                        CloseRecipe();
                        LoadRecipe(xmldoc);
                    }
                    catch (Exception ex)
                    {
                        ExceptionMessageBox.Show("Failed to get recipe", ex);
                        _recipeId = null;
                        return;
                    }
                }
            });

            // Récupération des stat de la recette
            {
                RecipeStat rstat = AdcExecutor.GetRecipeStat(_recipeId);
                if (rstat == null)
                {
                    _recipeId = null;
                    return;
                }

                RefreshNodeStatistics(rstat);
            }
        }

        public void CloseRecipe()
        {
            _recipeGraphVM.ClearGraph();
            ServiceRecipe.Instance().CloseRecipe();
        }

        public OperatorViewModel(IMessenger messenger) : base(messenger)
        {
            _recipeGraphVM = new RecipeGraphViewModel();
            _waferInfoVM = new WaferInfoViewModel();
            bool hideAdaToAdcWindowsServiceStatus;
            bool.TryParse(ConfigurationManager.AppSettings["Editor.HideAdaToAdcWindowsServiceStatus"], out hideAdaToAdcWindowsServiceStatus);
            HideAdaToAdcWindowsServiceStatus = hideAdaToAdcWindowsServiceStatus;
            if (!hideAdaToAdcWindowsServiceStatus)
                RefreshAdaToAdcWindowsServiceState();

            // Timer pour le mode opérateur
            _timerOperatorMode = new DispatcherTimer();
            _timerOperatorMode.Tick += new EventHandler(timerOperatorMode_Tick);

            InitEngine();
        }

        public void LoadRecipe(XmlDocument xmldoc)
        {
            _recipeGraphVM.LoadRecipe(xmldoc);
            OnPropertyChanged(nameof(RecipeGraphVM));
        }

        #region Private Methods       

        private void RefreshNodeStatistics()
        {
            if (_recipeGraphVM == null)
                return;

            if (_recipeId == null)
                return;

            RecipeStat rstat = AdcExecutor.GetRecipeStat(_recipeId);
            if (rstat == null)
                return;

            RefreshNodeStatistics(rstat);
        }

        private void InitEngine()
        {
            bool enableTransferToRobot = bool.Parse(ConfigurationManager.AppSettings["AdaToAdc.TransferToRobot.Enable"]);
            if (enableTransferToRobot)
                ADCEngine.ADC.Instance.TransferToRobotStub.Connect();
            _timerOperatorMode.Interval = new TimeSpan(0, 0, 0, seconds: 1);
            _timerOperatorMode.Start();
            string engineType = ConfigurationManager.AppSettings["AdcEngine.ProductionMode"];
            if (engineType == "InADC")
            {
                // Démarrage du service WCF
                _adcDistantExecutor = _adcLocalExecutor;

                host = new ServiceHost(AdcExecutor);
                foreach (var endpoint in host.Description.Endpoints)
                    ADCEngine.ADC.log("Creating service on \"" + endpoint.Address + "\"");
                host.Open();
            }
            else if (engineType == "InAcquisition")
            {
            }
            else
            {
                throw new ApplicationException("unknown mode for AdcEngine.ProductionMode: " + engineType);
            }
        }


        private void RefreshNodeStatistics(RecipeStat rstat)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (ModuleNodeViewModel node in _recipeGraphVM.Nodes)
                    node.RefreshStatistics(rstat);

                if (!rstat.IsRunning)
                    IsRecipeRunning = false;
            });
        }


        /// <summary>
        /// Selection d'une recette local pour le mode opérateur
        /// </summary>
        private void SelectLocalRecipeImprovement()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();

            openFileDlg.Filter = "Recipe files (*.adcrcp)|*.adcrcp";
            openFileDlg.InitialDirectory = ConfigurationManager.AppSettings["Editor.RecipeFolder"];

            if (openFileDlg.ShowDialog() != true)
                return;

            LocalRecipeImprovementPath = openFileDlg.FileName;
        }

        private void OpenReprocessAda()
        {
            ReprocessAdaViewModel reprocessAdaViewModel = new ReprocessAdaViewModel();
            Services.Services.Instance.PopUpService.ShowDialogWindow("Reprocess Ada", reprocessAdaViewModel, 500, 500, true);
            reprocessAdaViewModel.Dispose();
        }

        #endregion Private Methods

        #region Commands

        private AutoRelayCommand _exitAppCommand = null;
        public AutoRelayCommand ExitAppCommand
        {
            get
            {
                return _exitAppCommand ?? (_exitAppCommand = new AutoRelayCommand(() =>
                {
                    Services.Services.Instance.ShutdownService.ShutdownApp();
                }, () => { return true; }));
            }
        }

        private AutoRelayCommand _selectLocalRecipeImprovementCommand;
        public AutoRelayCommand SelectLocalRecipeImprovementCommand
        {
            get
            {
                return _selectLocalRecipeImprovementCommand ?? (_selectLocalRecipeImprovementCommand = new AutoRelayCommand(
              () =>
              {
                  SelectLocalRecipeImprovement();
              },
              () => { return LocalRecipeImprovementIsEnabled && IsConnectedToAdcEngine; }));
            }
        }

        private AutoRelayCommand _switchLocalRecipeCommand;
        public AutoRelayCommand SwitchLocalRecipeCommand
        {
            get
            {
                return _switchLocalRecipeCommand ?? (_switchLocalRecipeCommand = new AutoRelayCommand(
              () =>
              {
                  LocalRecipeImprovementIsEnabled = !LocalRecipeImprovementIsEnabled;
              },
              () => { return IsConnectedToAdcEngine; }));
            }
        }

        private AutoRelayCommand _reprocessAda;
        public AutoRelayCommand ReprocessAda
        {
            get
            {
                return _reprocessAda ?? (_reprocessAda = new AutoRelayCommand(
              () =>
              {
                  OpenReprocessAda();
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand _adaToAdcWindowsServiceCommand;
        public AutoRelayCommand AdaToAdcWindowsServiceCommand
        {
            get
            {
                return _adaToAdcWindowsServiceCommand ?? (_adaToAdcWindowsServiceCommand = new AutoRelayCommand(
              () =>
              {
                  if (_adaToAdcWindowsServiceStatus == ServiceControllerStatus.Running)
                      StopAdaToAdcWindowsService();
                  else
                      StartAdaToAdcWindowsService();
              },
              () =>
              {
                  return _adaToAdcWindowsServiceStatus.HasValue ? (_adaToAdcWindowsServiceStatus == ServiceControllerStatus.Running
                                                                         || _adaToAdcWindowsServiceStatus == ServiceControllerStatus.Stopped)
                                                                         : false;
              }));
            }
        }

        #endregion
    }
}
