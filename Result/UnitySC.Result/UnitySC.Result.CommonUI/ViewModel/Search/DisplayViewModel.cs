using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Result.CommonUI.ViewModel.LotWafer;
using UnitySC.Result.CommonUI.ViewModel.Wafers;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.ResultUI.Common;
using UnitySC.Shared.ResultUI.Common.Enums;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.CommonUI.ViewModel.Search
{
    public class DisplayViewModel : LotViewHeaderVM
    {
        #region Readonly Fields

        private readonly AutoResetEvent _eventBackgroundJobTaskDone = new AutoResetEvent(false);

        private readonly DuplexServiceInvoker<IResultService> _resultService;

        private readonly IMessenger _messenger;

        private readonly IResultDataFactory _resultDataFactory;

        private readonly Dictionary<ResultType, LotStatsVM> _lotStatsDictionary;

        private readonly Dictionary<ResultType, ResultWaferVM> _resultWaferDictionary;

        private readonly object _lockDictionaryJob = new object();

        private readonly object _lockResultUpdate = new object();

        private readonly object _selectedJobLock = new object();

        #endregion

        #region Fields

        public event Action<int> OnDisplayWaferPage;

        private LotWaferSlotVM[] _emptyWaferSlots;

        private Dictionary<string, Dictionary<string, LotWaferSlotVM[]>> _jobResultLotsDictionary;

        private KlarfSettingsData _klarfSettings;

        private Task _backgroundJobTask;

        private CancellationTokenSource _cancellationTokenJobTask;

        // Last process module information

        private string _lastProcessModuleName = ActorType.DEMETER.GetLabelName();

        // Last post process information

        private ResultType _lastPostProcessResultType = ResultType.ADC_Klarf;

        private string _lastPostProcessResultName = ResultType.ADC_Klarf.GetLabelName();

        private Type _lastPostProcessType = typeof(PostProcessViewModel);

        #endregion

        #region Properties
        private string _currentLoadingThreadToken = string.Empty;

        public LotWafersVM LotWafers { get; }

        /// <summary>
        /// Current lot view for container presenter.
        /// </summary>
        private ObservableRecipient _currentLotViewModel;

        public ObservableRecipient CurrentLotViewModel
        {
            get => _currentLotViewModel;
            private set => SetProperty(ref _currentLotViewModel, value);
        }

        /// <summary>
        /// Current Result VM wafer selected for detailed wafer page
        /// </summary>
        private ResultWaferVM _currentResultWafer;

        public ResultWaferVM CurrentResultWafer
        {
            get => _currentResultWafer;
            set
            {
                SetProperty(ref _currentResultWafer, value);
                WaferPage.CurrentResultWaferVM = value;
            }
        }

        /// <summary>
        /// Wafer detailed page VM (owned by MainResultVM)
        /// </summary>
        private WaferPageVM _waferPage;

        public WaferPageVM WaferPage
        {
            get => _waferPage;
            set => SetProperty(ref _waferPage, value);
        }

        /// <summary>
        /// Indicate if UI is busy
        /// </summary>
        private bool _isUiBusy;

        public bool IsUiBusy
        {
            get => _isUiBusy;
            set => SetProperty(ref _isUiBusy, value);
        }

        private string _jobRunIterName = string.Empty;

        private string JobRunIterName
        {
            set
            {
                if (!SetProperty(ref _jobRunIterName, value)) return;

                _messenger.Send(new DisplayJobRunIterNameMessage
                {
                    JobRunIterName = _jobRunIterName
                });
            }
        }

        /// <summary>
        /// Process Modules (PM) List (PM = DEMETER, LS etc...)
        /// </summary>
        private ObservableCollection<PMViewModel> _processModules;
        public ObservableCollection<PMViewModel> ProcessModules
        {
            get => _processModules;
            private set
            {
                if (_processModules == value) return;

                _processModules = value;
                OnPropertyChanged();

                //Selection PM and Post Process Results at job initialization
                SelectedProcessModule = _processModules == null ? null : _processModules.FirstOrDefault(x => x.LabelName == _lastPostProcessResultName) ?? _processModules.FirstOrDefault();
            }
        }

        /// <summary>
        /// Current Process Module selected
        /// </summary>
        private PMViewModel _selectedProcessModule;
        public PMViewModel SelectedProcessModule
        {
            get => _selectedProcessModule;
            set
            {
                if (_selectedProcessModule == value) return;

                _selectedProcessModule = value;

                if (_selectedProcessModule == null)
                {
                    SelectedPostProcessModule = null;
                }
                else
                {
                    _lastProcessModuleName = _selectedProcessModule.LabelName;

                    // Keep previous process module selection persistent
                    SelectedPostProcessModule = SelectedProcessModule?.GetFirstMatchingPostProcessViewModel(_lastPostProcessResultName, _lastPostProcessType);
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Current Post-Process Module selected (Post-Process module (PProc) could be a set of results (Klarf, ASO) or a set of acquisition maps or data...
        /// </summary>
        private PPViewModel _selectedPostProcessModule;
        public PPViewModel SelectedPostProcessModule
        {
            get => _selectedPostProcessModule;
            set
            {
                if (_selectedPostProcessModule == value) return;

                _selectedPostProcessModule = value;

                if (_selectedPostProcessModule != null)
                {
 //                   System.Diagnostics.Debug.WriteLine($"PPM changed =  {_selectedPostProcessModule.ResultLabelName} selected");

                    // Update token to stop current loading thread
                    _currentLoadingThreadToken = _lastPostProcessResultType != _selectedPostProcessModule.ResultType
                                                ? _lastPostProcessResultType.ToString() : _selectedPostProcessModule.ResultType.ToString();

//                    System.Diagnostics.Debug.WriteLine($"STOP {_currentLoadingThreadToken} thread ");

                    _lastPostProcessResultType = _selectedPostProcessModule.ResultType;
                    _lastPostProcessResultName = _selectedPostProcessModule.ResultLabelName;
                    _lastPostProcessType = _selectedPostProcessModule.GetType();
                }

                OnPropertyChanged();

                try
                {
                    LotWafers.UpdateSlots(SelectedProcessModule != null
                        ? _jobResultLotsDictionary[CurrentProcessModuleName][CurrentPostProcessingResultLabel]
                        : _emptyWaferSlots);

                    // Get new selected wafer (depending on existing results for selected ppm)
                    if (LotWafers.SelectedWafer != null)
                    {
                        WaferPage.SelectSlotIndex = LotWafers.SelectedWafer.SlotIndex;
                    }

                    WaferPage.UpdateSlots(LotWafers.Slots);

                    if (CurrentWaferResults != null)
                        ManageLotView(LotSelectedView.Key);
                }
                catch (Exception ex)
                {
                    var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                    notifierVM.AddMessage(new Message(MessageLevel.Error, ex.Message));
                }
            }
        }

        /// <summary>
        /// short Label information of selected wafer (SlotID + WaferName)
        /// </summary>
        private string _selectedWaferDetailName;

        public string SelectedWaferDetailName
        {
            get => _selectedWaferDetailName;
            set
            {
                if (_selectedWaferDetailName == value) return;
                _selectedWaferDetailName = value;
                OnPropertyChanged();
                _messenger.Send(new DisplaySelectedWaferDetaillNameMessage
                {
                    SelectedWaferDetaillName = _selectedWaferDetailName
                });
            }
        }

        private bool _showConnectionErrorPopup;

        public bool ShowConnectionErrorPopup
        {
            get => _showConnectionErrorPopup;
            set
            {
                if (_showConnectionErrorPopup == value) return;
                _showConnectionErrorPopup = value;
                OnPropertyChanged();
            }
        }

        public string CurrentProcessModuleName => SelectedProcessModule == null ? _lastProcessModuleName : SelectedProcessModule.LabelName;

        public WaferResultData[] CurrentWaferResults => (SelectedPostProcessModule as PostProcessViewModel)?.ResultData;

        public ResultType CurrentPostProcessingResultType => SelectedPostProcessModule?.ResultType ?? _lastPostProcessResultType;

        public string CurrentPostProcessingResultLabel => SelectedPostProcessModule == null ? _lastPostProcessResultName : SelectedPostProcessModule.ResultLabelName;

        #endregion Properties

        #region Selections

        private Job _selectedJob;
        public Job SelectedJob
        {
            get
            {
                Job ret;
                lock (_selectedJobLock)
                {
                    ret = _selectedJob;
                }
                return ret;
            }
            set
            {
                if (_selectedJob != value)
                {
                    lock (_selectedJobLock)
                    {
                        _selectedJob = null; // to avoid handling callbacks while changing jobs
                        DisplaySelectedJobData(value);
                        _selectedJob = value;
                    }
                    OnPropertyChanged();
                }
            }
        }

        #endregion Selections

        #region Constructor

        public DisplayViewModel(DuplexServiceInvoker<IResultService> resultService)
        {
            _resultService = resultService;
            _resultDataFactory = ClassLocator.Default.GetInstance<IResultDataFactory>();
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();

            LotWafers = new LotWafersVM(_resultService);

            _lotStatsDictionary = ViewerVMBuilder.Singleton.BuildDicoStatsVM(); //call builder for stats
            _resultWaferDictionary = ViewerVMBuilder.Singleton.BuildDicoResultVM(_resultDataFactory); //call builder for Wafer result

            ClearProcessModules();

            _messenger.Register<ResultNotificationMessage>(this, (r, m) => OnAddNewResultMessage(m));
            _messenger.Register<ResultStatsNotificationMessage>(this, (r, m) => OnAddNewStatsMessage(m));
            _messenger.Register<DisplayManageLotViewMessage>(this, (r, m) => OnNewStatsLotViewChange(m));
            //// Reception of the message triggered by the Search button in the event of a service connection error.
            _messenger.Register<DisplayErrorMessage>(this, (r, message) =>
            {
                // Checks the actual content of the message.
                switch (message.ErrorMessage)
                {
                    case SearchViewModel.ConnectionErrorMessage:
                        ShowConnectionErrorPopup = true;
                        break;
                }
            });
        }

        #endregion Constructor

        #region Methods

        public void Init()
        {
            LotViews = StatsFactory.GetEnumsWithDescriptions<LotView>();
            LotSelectedView = new KeyValuePair<LotView, string>();

            _emptyWaferSlots = BuildEmptyLotWaferSlotVM();
            Array.ForEach(_emptyWaferSlots, a => a.UnRegisterMessenger());

            LotWafers.Slots = BuildEmptyLotWaferSlotVM();
            LotWafers.UpdateSlots(_emptyWaferSlots);

            ResetAll();
        }

        private void BuildJobLot_async(CancellationToken token, List<ProcessModuleResult> jobProcessModulesResult)
        {
            lock (_lockDictionaryJob)
            {
                // Key 1: PM Label Name (from actor type) || Key 2 : Results Label Name (From Result Type and Index)
                _jobResultLotsDictionary = new Dictionary<string, Dictionary<string, LotWaferSlotVM[]>>();

                if (jobProcessModulesResult == null)
                    return;

                foreach (var processModuleResult in jobProcessModulesResult)
                {
                    if (token.IsCancellationRequested)
                        return;

                    var lotWaferSlotDictionary = new Dictionary<string, LotWaferSlotVM[]>();

                    if (!_jobResultLotsDictionary.ContainsKey(processModuleResult.LabelPMName))
                    {
                        _jobResultLotsDictionary.Add(processModuleResult.LabelPMName, lotWaferSlotDictionary);
                    }
                    else
                    {
                        // this means that we have 2 identical process module for one job how is it possible ?
                        throw new Exception("Two identical module for one job");
                    }

                    foreach (string postProcessingResultName in processModuleResult.PostProcessingResultLabels)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        var lotWaferSlot = BuildEmptyLotWaferSlotVM();

                        if (token.IsCancellationRequested)
                            return;

                        int isDirectoryNotAccessible = -1;

                        foreach (var result in processModuleResult.PostProcessingResults[postProcessingResultName])
                        {
                            if (token.IsCancellationRequested)
                                return;

                            if (result == null)
                                continue;

                            if (isDirectoryNotAccessible < 0 && result.ResultItem != null)
                            {
                                string directoryName = System.IO.Path.GetDirectoryName(result.ResultItem.ResPath);
                                isDirectoryNotAccessible = !System.IO.Directory.Exists(directoryName) ? 1 : 0;
                            }

                            lotWaferSlot[result.SlotId - 1].SetResult_Async(new LotItem(result.ResultItem), isDirectoryNotAccessible == 1);
                        }

                        lotWaferSlotDictionary.Add(postProcessingResultName, lotWaferSlot);
                    }

                    foreach (string acquisitionResultName in processModuleResult.AcquisitionResultsLabels)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        var lotWaferSlot = BuildEmptyLotWaferSlotVM();

                        if (token.IsCancellationRequested)
                            return;

                        int isDirectoryNotAccessible = -1;

                        foreach (var acquisitionResult in processModuleResult.AcquisitionResults[acquisitionResultName])
                        {
                            if (token.IsCancellationRequested)
                                return;

                            if (acquisitionResult == null)
                                continue;

                            if (isDirectoryNotAccessible < 0 && acquisitionResult.AcqItem != null)
                            {
                                string directoryName = System.IO.Path.GetDirectoryName(acquisitionResult.AcqItem.ResPath);
                                isDirectoryNotAccessible = !System.IO.Directory.Exists(directoryName) ? 1 : 0;
                            }

                            lotWaferSlot[acquisitionResult.SlotId - 1].SetResult_Async(new LotItem(acquisitionResult.AcqItem), isDirectoryNotAccessible == 1);
                        }

                        lotWaferSlotDictionary.Add(acquisitionResultName, lotWaferSlot);
                    }
                }
            }
        }

        private void OnAddNewResultMessage(ResultNotificationMessage msg)
        {
            if (SelectedJob == null || msg.ActorType == -1) return;

            if (SelectedJob.Id != msg.JobID) return;

            // need to update wafer-data & dictionary job -_-' for the moment
            // we assume consistency between wafer-data and dictionary job 

            var actorType = (ActorType)msg.ActorType;
            string actorName = actorType.GetLabelName();
            int slotIndex = msg.SlotID - 1;
            var resType = msg.ResultItem.ResultTypeEnum;
            string resultName = msg.ResultItem.Name ?? resType.DefaultLabelName((byte)msg.ResultItem.Idx);

            // this is our current job so perform our updates
            lock (_lockDictionaryJob)
            {
                if (_jobResultLotsDictionary == null)
                    return;

                if (_jobResultLotsDictionary.ContainsKey(actorName))
                {
                    // reach PM VM
                    var processModule = ProcessModules.First(x => x.LabelName == actorName);
                    // reach lotDictionary
                    var lotDictionary = _jobResultLotsDictionary[actorName];
                    if (lotDictionary.ContainsKey(resultName))
                    {
                        // Add result with existing PostProcess and PM

                        // reach wafer-data array
                        var ppm = processModule.PostProcessList.First(x => x.ResultLabelName == resultName);
                        // update wafer-data
                        ppm.ResultData[slotIndex] = msg.WaferResultData;
                        // reach result array
                        var waferSlot = lotDictionary[resultName];

                        if ((CurrentProcessModuleName == actorName) && (CurrentPostProcessingResultLabel == resultName))
                        {
                            // force coherent result
                            waferSlot[slotIndex].SetResult_ByPass(msg.ResultItem);

                            // update result
                            _ = Task.Run(() => waferSlot[slotIndex].SetResult_Async(new LotItem(msg.ResultItem))).ConfigureAwait(false);

                            //update wafer detail page
                            _ = Task.Run(() => WaferPage.UpdateWaferSlotVM(waferSlot[slotIndex])).ConfigureAwait(false);
                        }
                        else
                        {
                            // update result
                            _ = Task.Run(() => waferSlot[slotIndex].SetResult_Async(new LotItem(msg.ResultItem))).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        // Add NEW result Type on existing PM

                        // create & reach wafer-data array
                        var newPostProcess = new PostProcessViewModel(resultName, resType, new WaferResultData[25])
                        {
                            ResultData =
                            {
                                [slotIndex] = msg.WaferResultData
                            }
                        };
                        // create results array and update dictionary lot
                        var emptyLotWaferSlot = BuildEmptyLotWaferSlotVM();
                        lotDictionary.Add(resultName, emptyLotWaferSlot);

                        // update result
                        _ = Task.Run(() => emptyLotWaferSlot[slotIndex].SetResult_Async(new LotItem(msg.ResultItem))).ConfigureAwait(false);

                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            // Update observable collection
                            processModule.PostProcessList.Add(newPostProcess);
                        });
                    }
                }
                else
                {
                    // Add NEW Process module result

                    // create dictionary lot
                    var waferSlotDictionary = new Dictionary<string, LotWaferSlotVM[]>();
                    _jobResultLotsDictionary.Add(actorName, waferSlotDictionary);

                    // create result array
                    var emptyLotWaferSlot = BuildEmptyLotWaferSlotVM();
                    waferSlotDictionary.Add(resultName, emptyLotWaferSlot);

                    // create PM VM
                    var processModule = new PMViewModel
                    {
                        LabelName = actorName,
                        ActorType = actorType,
                        ChamberId = msg.ResultItem.Result.ChamberId,
                        PostProcessList = new ObservableCollection<PostProcessViewModel>(),
                        AcquisitionList = new ObservableCollection<AcquisitionDataViewModel>()
                    };

                    // create & reach wafer-data array
                    var postProcess = new PostProcessViewModel(resultName, resType, new WaferResultData[25])
                    {
                        ResultData =
                        {
                            [slotIndex] = msg.WaferResultData
                        }
                    };

                    // create Post process VM
                    processModule.PostProcessList.Add(postProcess);

                    // update result
                    _ = Task.Run(() => emptyLotWaferSlot[slotIndex].SetResult_Async(new LotItem(msg.ResultItem))).ConfigureAwait(false);

                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        // Update observable Process Module list
                        ProcessModules.Add(processModule);
                    });
                }
            }
        }

        private void OnAddNewStatsMessage(ResultStatsNotificationMessage msg)
        {
            if (SelectedJob != null && msg.ActorType != -1)
            {
                if (SelectedJob.Id == msg.JobID)
                {
                    // we assume here that datA has been already added (cf OnAddNewResultMessage)
                    var actorType = (ActorType)msg.ActorType;
                    string sActorLabel = actorType.GetLabelName();
                    int slotIndex = msg.SlotID - 1;
                    var resType = msg.ResultItem.ResultTypeEnum;
                    string sResLabelName = resType.DefaultLabelName((byte)msg.ResultItem.Idx);

                    lock (_lockDictionaryJob)
                    {
                        if (_jobResultLotsDictionary == null)
                            return;

                        // reach PM VM
                        var processModule = ProcessModules.First(x => x.LabelName == sActorLabel);
                        // reach wafer-data array
                        var postProcess = processModule.PostProcessList.First(x => x.ResultLabelName == sResLabelName);
                        // update wafer-data
                        var waferData = postProcess.ResultData[slotIndex];
                        if (waferData != null)
                        {
                            waferData.ResultItem.ResultItemValues = msg.ResultItem.ResultItemValues;
                        }
                    }

                    if (LotSelectedView.Key == LotView.Stats)
                    {
                        if (CurrentProcessModuleName == sActorLabel && CurrentPostProcessingResultLabel == sResLabelName)
                        {
                            Application.Current?.Dispatcher.Invoke(() =>
                            {
                                ManageLotView(LotSelectedView.Key);
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get selected job results.
        /// </summary>
        /// <param name="job"></param>
        ///<param name="token"></param>
        private void NewSelectedJobResults_Async(Job job, CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested)
                    return;

                string colorMapHazeSettingsName = _resultService.Invoke(x => x.GetHazeSettingsFromTables());
                const bool queryAcqData = true;

                _klarfSettings = _resultService.Invoke(x => x.GetKlarfSettingsFromTables());
                _lotStatsDictionary[ResultType.ADC_Klarf].UpdateKlarfSettings(_klarfSettings);
                _resultDataFactory.GetDisplayFormat(ResultType.ADC_Klarf)
                    .UpdateInternalDisplaySettingsPrm(_klarfSettings.RoughBins, _klarfSettings.SizeBins);
                _resultDataFactory.GetDisplayFormat(ResultType.ADC_Haze)
                    .UpdateInternalDisplaySettingsPrm(colorMapHazeSettingsName);

                if (token.IsCancellationRequested)
                    return;

                var jobProcessModulesResult = _resultService.Invoke(x => x.GetJobProcessModulesResults(job.ToolId, job.Id, queryAcqData));

                if (token.IsCancellationRequested)
                    return;

                // Populate process modules
                var processModules = new ObservableCollection<PMViewModel>();
                foreach (var moduleResult in jobProcessModulesResult)
                {
                    processModules.Add(new PMViewModel(moduleResult));
                }

                BuildJobLot_async(token, jobProcessModulesResult);

                if (token.IsCancellationRequested)
                    return;

                ProcessModules = processModules;

                if (token.IsCancellationRequested)
                    return;

                if (_processModules.Count > 0)
                {
                    SelectedResultFullName = $"{job.ToolId} - {job.LotName} - {job.RecipeName} - {job.Date} ";

                    _messenger.Send(new DisplaySelectedResultFullNameMessage
                    {
                        SelectedResultFullName = _selectedResultFullName
                    });

                    _lotSelectedView = LotViews.DefaultIfEmpty(new KeyValuePair<LotView, string>()).FirstOrDefault();

                    lock (_lockDictionaryJob)
                    {
                        LotWafers.Init_Async(_jobResultLotsDictionary[CurrentProcessModuleName][CurrentPostProcessingResultLabel], token);
                    }
                }
                else
                {
                    LotWafers.Init_Async(_emptyWaferSlots, token);
                }

                LotWafers.IsLotViewEnabled = true;

                StatsFactory.LastSelectedHistogram = new KeyValuePair<HistogramType, string>();
                StatsFactory.LastResultValueType = ResultValueType.Count;

                if (token.IsCancellationRequested)
                    return;

                LotWafers.Refresh_Async();

                // Notify UI
                //..........
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    OnPropertyChanged(nameof(ProcessModules));
                    OnPropertyChanged(nameof(SelectedProcessModule));
                    OnPropertyChanged(nameof(SelectedPostProcessModule));

                    OnPropertyChanged(nameof(SelectedResultFullName));
                    OnPropertyChanged(nameof(LotSelectedView));

                    ManageLotView(LotSelectedView.Key);
                    WaferPage.UpdateSlots(LotWafers.Slots);
                    IsUiBusy = false;
                });
            }
            catch (Exception ex)
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                    notifierVM.AddMessage(new Message(MessageLevel.Error, ex.Message));
                });
            }
            finally
            {
                _ = _eventBackgroundJobTaskDone.Set();
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    IsUiBusy = false;
                });
            }
        }

        private static LotWaferSlotVM[] BuildEmptyLotWaferSlotVM()
        {
            var slots = new LotWaferSlotVM[25];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new LotWaferSlotVM(i + 1);
            }
            return slots;
        }

        /// <summary>
        /// Display lot appropriate view depending user selection.
        /// </summary>
        /// <param name="lotSelectedView"></param>
        private void ManageLotView(LotView lotSelectedView)
        {
            ObservableRecipient ObservableRecipient = null;
            try
            {
                switch (lotSelectedView)
                {
                    case LotView.Wafers:
                        ObservableRecipient = LotWafers;
                        break;

                    case LotView.Stats:
                        // To Validate - (add combobox to change view in view)
                        if (CurrentPostProcessingResultType == ResultType.ADC_Klarf)
                        {
                            _lotStatsDictionary[CurrentPostProcessingResultType].UpdateKlarfSettings(_klarfSettings);
                        }

                        _lotStatsDictionary[CurrentPostProcessingResultType].UpdateStats(CurrentWaferResults);
                        _lotStatsDictionary[CurrentPostProcessingResultType].SelectLotView(LotView.Stats);
                        ObservableRecipient = _lotStatsDictionary[CurrentPostProcessingResultType];
                        break;

                    default:
                        // Exception
                        throw new Exception(lotSelectedView + " view is not implemented !");
                }
            }
            catch (Exception ex)
            {
                var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVM.AddMessage(new Message(MessageLevel.Error, ex.Message));
            }

            CurrentLotViewModel = ObservableRecipient;
        }

        private void OnNewStatsLotViewChange(DisplayManageLotViewMessage message)
        {
            LotSelectedView = message.SelectedStatsLotview;
        }


        private void ClearProcessModules()
        {
            if (ProcessModules != null)
            {
                foreach (var module in ProcessModules)
                {
                    module.Dispose();
                }
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    ProcessModules.Clear();
                });
            }

            ProcessModules = null;
        }

        /// <summary>
        /// Reset all data.
        /// </summary>
        private void ResetAll()
        {
            //Cleaning existing data
            ClearProcessModules();

            LotSelectedView = new KeyValuePair<LotView, string>();
            SelectedResultFullName = string.Empty;
            SelectedWaferDetailName = string.Empty;
            JobRunIterName = string.Empty;
            LotWafers.IsLotViewEnabled = false;

            CleanJobResultLots();

            ManageLotView(LotSelectedView.Key);
        }

        private void CleanJobResultLots()
        {
            lock (_lockDictionaryJob)
            {
                if (_jobResultLotsDictionary != null)
                {
                    foreach (var jobResults in _jobResultLotsDictionary.Values)
                    {
                        foreach (var jobResultsValue in jobResults.Values)
                        {
                            Array.ForEach(jobResultsValue, a => a.Cleanup());
                        }

                        jobResults.Clear();
                    }
                    _jobResultLotsDictionary.Clear();
                }
                _jobResultLotsDictionary = null;
            }

            GC.Collect();
        }

        private void WaferDetailSelection(bool changePage)
        {
            var selectedWafer = LotWafers.SelectedWafer;
            // case wafer not present or in error result cannot be displayed
            if (selectedWafer == null || selectedWafer.Item == null || selectedWafer.Item.State < (int)ResultState.Ok)
                return;
            //Set cursor to busy mode
            if (changePage) BusyHourglass.SetBusyState();

            // raised priority for thumbnail generation
            if (selectedWafer.Item.InternalState == (int)ResultInternalState.NotProcess)
            {
                _ = _resultService.Invoke(x => x.ResultScanRequest(selectedWafer.Item.Id, selectedWafer.Item.IsAcquisition));
            }

            if (selectedWafer.ResultDataObj == null)
            {
                try
                {
                    selectedWafer.ResultDataObj = _resultDataFactory.CreateFromFile(selectedWafer.Item.ResultTypeEnum, selectedWafer.Item.Id, selectedWafer.Item.ResPath);
                    // in very specific cases we can have some new rough bins unknown and not define in database, and not yet added by result scanner.
                    // This should be very rare case... if for any reason it happens... we should insert in database the new rough bins encountered here and update color-size/rough bins maps
                }
                catch (Exception ex)
                {
                    var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                    notifierVM.AddMessage(new Message(MessageLevel.Error, $"An error occurred while opening the result named : {selectedWafer.WaferName}. Error : " + ex.Message));
                    return;
                }
            }

            if (WaferPage.SelectSlotIndex != selectedWafer.SlotIndex)
            {
                WaferPage.SelectSlotIndex = selectedWafer.SlotIndex;
            }

            SelectedWaferDetailName = string.IsNullOrEmpty(selectedWafer.WaferName) ? $"Slot {selectedWafer.SlotID}" : $"{selectedWafer.WaferName}";

            lock (_lockResultUpdate)
            {
                try
                {
                    CurrentResultWafer = _resultWaferDictionary[CurrentPostProcessingResultType];
                    CurrentResultWafer.UpdateResData(selectedWafer.ResultDataObj);
                }
                catch (Exception ex)
                {
                    var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                    notifierVM.AddMessage(new Message(MessageLevel.Error, $"An error occurred while opening the result named : {selectedWafer.WaferName}. Error : " + ex.Message));
                }
            }

            if (changePage)
            {
                OnDisplayWaferPage?.Invoke(0);
            }
        }

        public void WaferDetailSelectionFromWaferPage()
        {
            if (WaferPage.SelectSlotIndex < 0) return;

            if (!CurrentResultWafer.IsBusy)
            {
                var lotWafersSlot = LotWafers.Slots[WaferPage.SelectSlotIndex];

                if (lotWafersSlot == null || !lotWafersSlot.IsResultExist) return;

                if (LotWafers.SelectedWafer == lotWafersSlot && !WaferPage.IsWaferDetailDisplayed) return;

                // Same PPM but slot changed : update token to stop previous thread
                if (WaferPage.SelectSlotIndex != LotWafers.SelectedWafer.SlotIndex)
                {
                    _currentLoadingThreadToken = SelectedPostProcessModule.ResultType.ToString();
                }

                _messenger.Send(new ResultsDisplayChangedMessage()
                {
                    ResultsDisplayName = _selectedPostProcessModule.ResultLabelName
                }, _currentLoadingThreadToken
                );

                LotWafers.SelectedWafer = lotWafersSlot;
                WaferDetailSelection(false);
            }
        }

        /// <summary>
        /// Return true if any connection is done with the service
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetConnectionAsync()
        {
            IsUiBusy = true;
            ShowConnectionErrorPopup = false;
            bool isServiceConnected = false;

            await Task.Run(() =>
            {
                int tryNumber = 0;
                const int tryMax = 2;

                while (!isServiceConnected && tryNumber < tryMax)
                {
                    tryNumber++;
                    try
                    {
                        _resultService.ResetConnexion();
                        _ = _resultService.Invoke(x => x.GetTools());
                        isServiceConnected = true;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            });

            IsUiBusy = false;
            ShowConnectionErrorPopup = !isServiceConnected;

            return isServiceConnected;
        }

        /// <summary>
        /// Check the if the connection to the service can be made
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> IsConnectionAvailableAsync()
        {
            bool isServiceConnected = false;

            await Task.Run(() =>
            {
                try
                {
                    isServiceConnected = true;
                }
                catch (Exception)
                {
                    isServiceConnected = false;
                }
            });
            return isServiceConnected;
        }

        /// <summary>
        /// Show the selected job data if any error occur show empty wafers
        /// </summary>
        /// <param name="job"></param>
        private async void DisplaySelectedJobData(Job job)
        {
            IsUiBusy = true;
            bool isServiceConnectionAvailable = await IsConnectionAvailableAsync();

            try
            {
                if (!isServiceConnectionAvailable)
                {
                    ShowConnectionErrorPopup = true;
                    IsUiBusy = false;
                    return;
                }

                if (_backgroundJobTask != null)
                {
                    _cancellationTokenJobTask?.Cancel();

                    // await previous job cancellation
                    _ = await Task.Run(() => _eventBackgroundJobTaskDone.WaitOne()).ConfigureAwait(false);

                    _backgroundJobTask = null;
                    // note that here busy flag could be @false
                }

                LotWafers.UpdateSlots(_emptyWaferSlots);

                LotWafers.SelectedWafer = null;
                WaferPage.UpdateSlots(LotWafers.Slots);

                ResetAll();

                if (job == null)
                {
                    IsUiBusy = false;
                    return;
                }

                JobRunIterName = (job.RunIter == 0) ? string.Empty : $"#{job.RunIter}";

                _ = _eventBackgroundJobTaskDone.Reset();

                _cancellationTokenJobTask = new CancellationTokenSource();
                var token = _cancellationTokenJobTask.Token;

                IsUiBusy = true; // need to raise it up in case some previous background task has down the busy flag

                _backgroundJobTask = Task.Run(() => NewSelectedJobResults_Async(job, token), token);
            }
            catch (Exception ex)
            {
                if (_backgroundJobTask != null)
                {
                    _cancellationTokenJobTask?.Cancel();
                    _ = _eventBackgroundJobTaskDone.Set();
                }

                var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVM.AddMessage(new Message(MessageLevel.Error, $"{ex.Message}\n{ex.StackTrace}"));

                IsUiBusy = false;
            }
        }

        #region Overrides of LotViewHeaderVM

        protected override void OnSelectedResultFullNameChanged(string name)
        {
            _messenger.Send(new DisplaySelectedResultFullNameMessage { SelectedResultFullName = name });
        }

        protected override void OnLotSelectedViewChanged(KeyValuePair<LotView, string> selectedView)
        {
            ManageLotView(selectedView.Key);
        }
        protected override void Cleanup()
        {
            _messenger.Unregister<DisplayErrorMessage>(this);
            _messenger.Unregister<ResultNotificationMessage>(this);
            _messenger.Unregister<ResultStatsNotificationMessage>(this);
            _messenger.Unregister<DisplayManageLotViewMessage>(this);
        }
        #endregion

        #endregion Methods

        #region Commands

        /// <summary>
        /// Wafer details page navigation
        /// </summary>
        private AutoRelayCommand _selectedWaferDetail;

        public AutoRelayCommand SelectedWaferDetail
        {
            get
            {
                return _selectedWaferDetail ?? (_selectedWaferDetail = new AutoRelayCommand(
              () =>
              {
                  WaferDetailSelection(true);
              },
              () => !WaferPage.IsWaferDetailDisplayed));
            }
        }

        /// <summary>
        /// Relaunch Lot Thumbnail generation and display of the current result type.
        /// </summary>
        private AutoRelayCommand _refreshThumbnailsCommand;

        public AutoRelayCommand RefreshThumbnailsCommand
        {
            get
            {
                return _refreshThumbnailsCommand ?? (_refreshThumbnailsCommand = new AutoRelayCommand(
                    () =>
                    {
                        _ = Task.Run(() =>
                        {
                            IsUiBusy = true;
                            LotWafers.RefreshThumbnail();
                            WaferPage.UpdateSlots(LotWafers.Slots);
                            IsUiBusy = false;
                        });
                    },
                    () => (CurrentWaferResults != null) && !IsUiBusy && (LotWafers.Slots != _emptyWaferSlots)));
            }
        }

        /// <summary>
        /// Manage single selection of any PostProcess(PP) GroupBox results (ADC, Metro logy + acquisition) .
        /// </summary>
        private AutoRelayCommand<PPViewModel> _selectPostProcessCommand;

        public AutoRelayCommand<PPViewModel> SelectPostProcessCommand => _selectPostProcessCommand ?? (_selectPostProcessCommand = new AutoRelayCommand<PPViewModel>(SelectPostProcessCommandExecute, SelectPostProcessCommandCanExecute));

        private bool SelectPostProcessCommandCanExecute(PPViewModel arg)
        {
            return arg != null && !ReferenceEquals(arg, SelectedPostProcessModule);
        }

        private void SelectPostProcessCommandExecute(PPViewModel arg)
        {
            SelectedPostProcessModule = arg;
        }

        /// <summary>
        /// Manage single selection of any Process Module GroupBox results.
        /// </summary>
        private AutoRelayCommand<PMViewModel> _selectProcessModuleCommand;

        public AutoRelayCommand<PMViewModel> SelectProcessModuleCommand => _selectProcessModuleCommand ?? (_selectProcessModuleCommand = new AutoRelayCommand<PMViewModel>(SelectProcessModuleCommandExecute, SelectProcessModuleCommandCanExecute));

        private bool SelectProcessModuleCommandCanExecute(PMViewModel arg)
        {
            return arg != null && !ReferenceEquals(arg, SelectedProcessModule);
        }

        private void SelectProcessModuleCommandExecute(PMViewModel arg)
        {
            SelectedProcessModule = arg;
        }

        #endregion Commands

        /// <summary>
        /// This code is not called
        /// Messages registration has been done once in DisplayViewModel ctor.
        /// and not unregistered (VM life cycle is application life cycle).
        /// </summary>
        public override void Dispose()
        {
            Cleanup();
        }
    }
}
