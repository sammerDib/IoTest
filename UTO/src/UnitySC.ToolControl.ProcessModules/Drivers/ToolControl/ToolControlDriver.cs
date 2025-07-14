using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Windows.Threading;

using Agileo.Common.Logging;

using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl
{
    [ComVisible(true)]
    [Guid(Constants.UtoComInterfaceString)]
    [ComSourceInterfaces(typeof(ITcUtoComInterfaceEvents))]
    [ClassInterface(ClassInterfaceType.None)]
    public class ToolControlDriver : ITcUtoComInterface, IDisposable
    {
        #region Fields

        private int m_dwRegister;
        private readonly ILogger _logger;

        private Dispatcher _toolControlDispatcher;

        #endregion

        #region Properties

        private bool _isClientConnected;

        public bool IsClientConnected
        {
            get => _isClientConnected;
            private set
            {
                if (_isClientConnected != value)
                {
                    _isClientConnected = value;
                    ClientConnectionStateChanged?.Invoke(value);
                }
            }
        }

        public bool IsStarted { get; private set; }

        #endregion

        #region COM Events

        [ComVisible(false)]
        public delegate void StringEventHandler(string s);

        [ComVisible(false)]
        public delegate void ProcessModuleEventHandler(string moduleId);

        [ComVisible(false)]
        public delegate void GetModuleStateEventHandler(
            string moduleId,
            ref ModuleState moduleState);

        [ComVisible(false)]
        public delegate void ProcessJobEventHandler(IProcessJob processJob);

        [ComVisible(false)]
        public delegate void SelectRecipeEventHandler(
            IProcessJob processJob,
            string moduleId,
            string moduleRecipeId);

        [ComVisible(false)]
        public delegate void PrepareForSubstrateEventHandler(string moduleId, int eef);

        [ComVisible(false)]
        public delegate void ExecuteRecipeEventHandler(
            IProcessJob processJob,
            string moduleId,
            string moduleRecipeId,
            ISubstrate subtrate);

        [ComVisible(false)]
        public delegate void GetAllAvailableRecipeNamesHandler(IComStringList recipeNames);

        [ComVisible(false)]
        public delegate void GetAvailableModulesHandler(IProcessModuleCollection moduleIds);

        [ComVisible(false)]
        public delegate void EventHandler();

        [ComVisible(false)]
        public delegate void GetModuleAlignmentAngleHandler(string moduleId, ref double angle);

        [ComVisible(false)]
        public delegate void CreateProcessJobHandler(IProcessJob processJob);

        [ComVisible(false)]
        public delegate void UpdateProcessJobFlowRecipeHandler(
            IProcessJob processJob,
            ref bool success);

        [ComVisible(false)]
        public delegate void TableDataResponseEventHandler(ITableDataResponse response);

        [ComVisible(false)]
        public delegate void TableDataRequestEventHandler(ITableDataRequest request);

        [ComVisible(false)]
        public delegate void S7F3StreamEventHandler(
            string flowRecipeName,
            IStream stream,
            long streamLength,
            ref bool success,
            ref string errorMessage);

        [ComVisible(false)]
        public delegate void S7F5StreamEventHandler(
            string flowRecipeName,
            IStream stream,
            ref bool success,
            ref string errorMessage);

        [ComVisible(false)]
        public delegate void S7F17StreamHandler(
            IComStringList flowRecipeNames,
            ref bool success,
            ref string errorMessage);

        [ComVisible(false)]
        public delegate void GetEquipmentStateEventHandler(ref EquipmentState equipmentState);

        [ComVisible(false)]
        public delegate void ChangeOperationModeEventHandler(
            OperationMode operationMode,
            ref bool success,
            ref string errorMessage);

        [ComVisible(false)]
        public delegate void ModuleSubstrateEventHandler(string moduleId, ISubstrate substrate);

        public event GetModuleStateEventHandler OnGetModuleState;
        public event GetAllAvailableRecipeNamesHandler OnGetAllAvailableRecipeNames;
        public event GetAvailableModulesHandler OnGetAvailableModules;
        public event GetModuleAlignmentAngleHandler OnGetModuleAlignmentAngle;
        public event CreateProcessJobHandler OnCreateProcessJob;
        public event UpdateProcessJobFlowRecipeHandler OnUpdateProcessJobFlowRecipe;
        public event ProcessJobEventHandler OnDeleteProcessJob;
        public event ProcessJobEventHandler OnAbortProcessJob;
        public event SelectRecipeEventHandler OnSelectModuleRecipe;
        public event PrepareForSubstrateEventHandler OnPrepareForSubstrateLoad;
        public event PrepareForSubstrateEventHandler OnPrepareForSubstrateUnload;
        public event PrepareForSubstrateEventHandler OnSubstrateLoaded;
        public event PrepareForSubstrateEventHandler OnSubstrateUnloaded;
        public event ExecuteRecipeEventHandler OnExecuteModuleRecipe;
        public event EventHandler OnClose;
        public event TableDataResponseEventHandler OnSendDataSetAck_S13F14;
        public event TableDataRequestEventHandler OnDataSetRequest_S13F15;
        public event S7F3StreamEventHandler OnS7F3Changed;
        public event S7F5StreamEventHandler OnS7F5RequestPPIDChanged;
        public event GetEquipmentStateEventHandler OnGetEquipmentState;
        public event ChangeOperationModeEventHandler OnChangeOperationMode;
        public event ModuleSubstrateEventHandler OnModuleSubstratePresent;
        public event ModuleSubstrateEventHandler OnModuleSubstrateRemoved;
        public event TransportModuleStateEventHandler OnSetTransportModuleState;
        public event EquipmentStateEventHandler OnUTOEquipmentStateChanged;
        public event S7F17StreamHandler OnS7F17DeletePPID;

        #endregion

        #region Integration Events

        [ComVisible(false)]
        public delegate void RecipeNamesEventHandler(ComStringList recipeNames);

        [ComVisible(false)]
        public delegate void ModuleIdsEventHandler(ComStringList ids);

        [ComVisible(false)]
        public delegate void ModuleStateEventHandler(string moduleId, ModuleState state);

        [ComVisible(false)]
        public delegate void ModuleEventHandler(string moduleId, bool success);

        [ComVisible(false)]
        public delegate void FlowRecipeEventHandler(IProcessJob job, IFlowRecipeCollection items);

        [ComVisible(false)]
        public delegate void ClientConnectionStateChangedEventHandler(bool isConnected);

        [ComVisible(false)]
        public delegate void ModuleSubstratePresentHandler(string moduleId, bool value);

        [ComVisible(false)]
        public delegate void SendCollectionEventEventHandler(
            string collectionEventName,
            ISecsVariableList dataVariables);

        [ComVisible(false)]
        public delegate void TransportModuleStateEventHandler(ModuleState moduleState);

        [ComVisible(false)]
        public delegate void SendDataSet_S13F13EventHandler(ITableData_S13F13 tableData);

        [ComVisible(false)]
        public delegate void SendDataSet_S13F16EventHandler(ITableData_S13F16 tableData);

        [ComVisible(false)]
        public delegate void EquipmentStateEventHandler(EquipmentState equipmentState);

        public event Func<bool> AreJobsActiveReceived;
        public event ModuleStateEventHandler ModuleStateReceived;
        public event FlowRecipeEventHandler FlowRecipeReceived;
        public event ModuleEventHandler ReadyForSubstrateLoadRecieved;
        public event ModuleEventHandler ReadyForSubstrateUnloadRecieved;
        public event ModuleEventHandler TriggerSubstrateLoadedRecieved;
        public event ModuleEventHandler TriggerSubstrateUnloadedRecieved;
        public event Action<string, PPChangeState> ProcessProgramModificationNotifyReceived;
        public event SendCollectionEventEventHandler SendCollectionEventReceived;
        public event SendDataSet_S13F13EventHandler SendDataSet_S13F13Received;
        public event SendDataSet_S13F16EventHandler SendDataSet_S13F16Received;
        public event ClientConnectionStateChangedEventHandler ClientConnectionStateChanged;
        public event ChangeOperationModeEventHandler RequestChangeOperationModeReceived;
        public event EquipmentStateEventHandler EquipmentStateRecieved;
        public event TransportModuleStateEventHandler GetTransportModuleStateReceived;
        public event ModuleSubstratePresentHandler TriggerSubstratePresentReceived;
        public event GetEquipmentStateEventHandler GetUTOEquipmentStateReceived;

        #endregion

        #region Constructor/Destructor

        public ToolControlDriver(ILogger logger)
        {
            _logger = logger;
            _dataFactory = new SecsDataFactory();

            // create a manual reset event for crossthread signalling.
            var dispatcherReadyEvent = new ManualResetEvent(false);

            // create a new thread.
            var thread = new Thread(
                new ThreadStart(
                    () =>
                    {
                        // get the current dispatcher (if it didn't exists
                        // it will be created.
                        _toolControlDispatcher = Dispatcher.CurrentDispatcher;

                        // set the signal that the dispatcher is created.
                        dispatcherReadyEvent.Set();

                        // run the dispatcher.
                        Dispatcher.Run();
                    }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            // wait until the dispatcher is created on the thread.
            dispatcherReadyEvent.WaitOne();
        }

        ~ToolControlDriver()
        {
            Dispose();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (m_dwRegister != 0)
            {
                IsStarted = false;
                IsClientConnected = false;

                try
                {
                    RaiseOnClose();
                    Ole32.Check(Ole32.CreateBindCtx(0, out var bc));
                    bc.GetRunningObjectTable(out var rot);
                    rot.Revoke(m_dwRegister);
                    m_dwRegister = 0;
                }
                catch (Exception)
                {
                    // ignore any exceptions
                }
            }
        }

        #endregion

        #region Public Methods

        public bool Start()
        {
            var result = true;
            _toolControlDispatcher.Invoke(
                () =>
                {
                    if (!IsStarted)
                    {
                        IMoniker moniker = null;
                        IBindCtx bc = null;
                        IRunningObjectTable rot = null;

                        try
                        {
                            Ole32.Check(
                                Ole32.CreateItemMoniker(
                                    "!",
                                    "{" + Constants.UtoComInterfaceString + "}",
                                    out moniker));
                            Ole32.Check(Ole32.CreateBindCtx(0, out bc));
                            bc.GetRunningObjectTable(out rot);
                            m_dwRegister = rot.Register(
                                Ole32.ROTFLAGS_ALLOWANYCLIENT,
                                this,
                                moniker);
                        }
                        catch (Exception ex)
                        {
                            m_dwRegister = 0;
                            _logger.Error(ex);
                            _logger.Debug("UtoComInterface failed Started");
                            result = false;
                        }
                        finally
                        {
                            if (result)
                            {
                                if (moniker != null)
                                {
                                    Marshal.FinalReleaseComObject(moniker);
                                }

                                if (bc != null)
                                {
                                    Marshal.FinalReleaseComObject(bc);
                                }

                                if (rot != null)
                                {
                                    Marshal.FinalReleaseComObject(rot);
                                }
                            }
                        }

                        _logger.Debug("UtoComInterface Started");
                        IsStarted = true;
                    }
                });
            return result;
        }

        #endregion

        #region ITcUtoComInterface

        public void AreJobsActive(ref bool value)
        {
            if (AreJobsActiveReceived != null)
            {
                value = AreJobsActiveReceived.Invoke();
            }

            value = false;
        }

        public void Advise()
        {
            _logger.Debug($"Advised, thread = {Environment.CurrentManagedThreadId}");
            IsClientConnected = true;
        }

        public void SetModuleState(string processModuleId, ModuleState moduleState)
        {
            _logger.Debug(
                $"SetModuleState received with processModuleId = {processModuleId} and moduleState = {moduleState}, thread = {Environment.CurrentManagedThreadId}");
            ModuleStateReceived?.Invoke(processModuleId, moduleState);
        }

        public void SetProcessJobFlowRecipe(IProcessJob processJob, IFlowRecipeCollection items)
        {
            _logger.Debug(
                $"SetProcessJobFlowRecipe received with process job id = {processJob.Name}, thread = {Environment.CurrentManagedThreadId}");
            FlowRecipeReceived?.Invoke(processJob, items);
        }

        public void Unadvise()
        {
            _logger.Debug($"Unadvise, thread = {Environment.CurrentManagedThreadId}");
            IsClientConnected = false;
        }

        public void ReadyForSubstrateLoad(string moduleId, bool success)
        {
            _logger.Debug(
                $"{nameof(ReadyForSubstrateLoad)} received with module id = {moduleId} & success = {success}, thread = {Environment.CurrentManagedThreadId}");
            ReadyForSubstrateLoadRecieved?.Invoke(moduleId, success);
        }

        public void ReadyForSubstrateUnload(string moduleId, bool success)
        {
            _logger.Debug(
                $"{nameof(ReadyForSubstrateUnload)} received with module id = {moduleId} && success = {success}, thread = {Environment.CurrentManagedThreadId}");
            ReadyForSubstrateUnloadRecieved?.Invoke(moduleId, success);
        }

        public void TriggerSubstrateLoaded(string moduleId)
        {
            _logger.Debug(
                $"{nameof(TriggerSubstrateLoaded)} received with module id = {moduleId}, thread = {Environment.CurrentManagedThreadId}");
            TriggerSubstrateLoadedRecieved?.Invoke(moduleId, true);
        }

        public void TriggerSubstrateUnloaded(string moduleId)
        {
            _logger.Debug(
                $"{nameof(TriggerSubstrateUnloaded)} received with module id = {moduleId}, thread = {Environment.CurrentManagedThreadId}");
            TriggerSubstrateUnloadedRecieved?.Invoke(moduleId, true);
        }

        public void TriggerSubstratePresent(string moduleId, bool value)
        {
            _logger.Debug(
                $"{nameof(TriggerSubstratePresent)} received with module id = {moduleId} & value = {value}, thread = {Environment.CurrentManagedThreadId}");
            TriggerSubstratePresentReceived?.Invoke(moduleId, value);
        }

        public void ProcessProgramModificationNotify(string processProgramID, PPChangeState state)
        {
            _logger.Debug(
                $"{nameof(ProcessProgramModificationNotify)} received with ppId = {processProgramID} & state = {state}, thread = {Environment.CurrentManagedThreadId}");
            ProcessProgramModificationNotifyReceived?.Invoke(processProgramID, state);
        }

        public void SendCollectionEvent(string collectionEventName, ISecsVariableList dataVariables)
        {
            _logger.Debug(
                $"{nameof(SendCollectionEvent)} received with collection event name = {collectionEventName}, thread = {Environment.CurrentManagedThreadId}");
            SendCollectionEventReceived?.Invoke(collectionEventName, dataVariables);
        }

        public void SendDataSet_S13F13(ITableData_S13F13 tableData)
        {
            _logger.Debug(
                $"{nameof(SendDataSet_S13F13)} received, thread = {Environment.CurrentManagedThreadId}");
            SendDataSet_S13F13Received?.Invoke(tableData);
        }

        public void SendDataSet_S13F16(ITableData_S13F16 tableData)
        {
            _logger.Debug(
                $"{nameof(SendDataSet_S13F16)} received, thread = {Environment.CurrentManagedThreadId}");
            SendDataSet_S13F16Received?.Invoke(tableData);
        }

        public void SetEquipmentState(EquipmentState equipmentState)
        {
            _logger.Debug(
                $"{nameof(SetEquipmentState)} received with equipment state = {equipmentState}, thread = {Environment.CurrentManagedThreadId}");
            EquipmentStateRecieved?.Invoke(equipmentState);
        }

        public void RequestChangeOperationMode(
            OperationMode operationMode,
            ref bool success,
            ref string errorMessage)
        {
            _logger.Debug(
                $"{nameof(RequestChangeOperationMode)} received with operation mode = {operationMode}, thread = {Environment.CurrentManagedThreadId}");
            RequestChangeOperationModeReceived?.Invoke(
                operationMode,
                ref success,
                ref errorMessage);
            _logger.Debug(
                $"{nameof(RequestChangeOperationMode)} returned with success = {success} & error message = {errorMessage}, thread = {Environment.CurrentManagedThreadId}");
        }

        public void GetUTOEquipmentState(ref EquipmentState equipmentState)
        {
            _logger.Debug(
                $"{nameof(GetUTOEquipmentState)} received, thread = {Environment.CurrentManagedThreadId}");
            GetUTOEquipmentStateReceived?.Invoke(ref equipmentState);
            _logger.Debug(
                $"{nameof(GetUTOEquipmentState)} return with equipment state = {equipmentState}, thread = {Environment.CurrentManagedThreadId}");
        }

        private readonly SecsDataFactory _dataFactory;
        public ISecsDataFactory DataFactory => _dataFactory;

        #endregion

        #region ITcUtoComInterfaceEvents Raise Events

        public void RaiseOnGetProcessModuleState(string moduleId, ref ModuleState moduleState)
        {
            var refModuleState = moduleState;
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnGetModuleState)} sent for moduleId {moduleId}, thread = {Environment.CurrentManagedThreadId}");
                    OnGetModuleState?.Invoke(moduleId, ref refModuleState);
                    _logger.Debug(
                        $"{nameof(OnGetModuleState)} received with moduleState = {refModuleState}, thread = {Environment.CurrentManagedThreadId}");
                });
            moduleState = refModuleState;
        }

        public ComStringList RaiseOnGetAllAvailableRecipeNames()
        {
            var list = new ComStringList();
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnGetAllAvailableRecipeNames)}, thread = {Environment.CurrentManagedThreadId}");

                    OnGetAllAvailableRecipeNames?.Invoke(list);

                    var sb = new StringBuilder();
                    sb.AppendLine($"{nameof(OnGetAllAvailableRecipeNames)} values are :");
                    foreach (var item in list)
                    {
                        sb.AppendLine(item.ToString());
                    }

                    _logger.Debug(sb.ToString());
                });

            return list;
        }

        public ProcessModuleCollection RaiseOnGetAvailableModules()
        {
            var list = new ProcessModuleCollection();
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"Before lock {nameof(RaiseOnGetAvailableModules)}, thread = {Environment.CurrentManagedThreadId}");

                    //lock (_lockObject)
                    {
                        _logger.Debug($"{nameof(OnGetAvailableModules)}");

                        OnGetAvailableModules?.Invoke(list);

                        var sb = new StringBuilder();
                        sb.AppendLine($"{nameof(OnGetAvailableModules)} values are :");
                        foreach (var item in list)
                        {
                            sb.AppendLine(
                                $"Name = {item.Name}, Id = {item.Id}, State = {item.State}, IsSubstratePresent = {item.IsSubstratePresent}");
                        }

                        _logger.Debug(sb.ToString());

                        _logger.Debug(
                            $"End lock {nameof(RaiseOnGetAvailableModules)}, thread = {Environment.CurrentManagedThreadId}");
                    }
                });

            return list;
        }

        public double RaiseOnGetModuleAlignmentAngle(string moduleId)
        {
            var angle = -1.0;
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnGetModuleAlignmentAngle)} for module {moduleId}, thread = {Environment.CurrentManagedThreadId}");
                    OnGetModuleAlignmentAngle?.Invoke(moduleId, ref angle);
                    _logger.Debug($"{nameof(OnGetModuleAlignmentAngle)} received angle is {angle}");
                });
            return angle;
        }

        public bool RaiseOnCreateProcessJob(ProcessJob processJob)
        {
            var success = false;
            _toolControlDispatcher.Invoke(
                () =>
                {
                    RaiseOnUpdateProcessJobFlowRecipe(processJob, ref success);
                    if (success)
                    {
                        _logger.Debug(
                            $"{nameof(OnCreateProcessJob)} for PJ {processJob.Name}, thread = {Environment.CurrentManagedThreadId}");
                        OnCreateProcessJob?.Invoke(processJob);
                    }
                });
            return success;
        }

        public void RaiseOnUpdateProcessJobFlowRecipe(ProcessJob processJob, ref bool success)
        {
            var refSuccess = success;
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnUpdateProcessJobFlowRecipe)} for PJ {processJob.Name}, thread = {Environment.CurrentManagedThreadId}");
                    OnUpdateProcessJobFlowRecipe?.Invoke(processJob, ref refSuccess);
                    _logger.Debug(
                        $"{nameof(OnUpdateProcessJobFlowRecipe)} success = {refSuccess}, thread = {Environment.CurrentManagedThreadId}");
                });
            success = refSuccess;
        }

        public void RaiseOnDeleteProcessJob(ProcessJob processJob)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnDeleteProcessJob)} for PJ {processJob.Name}, thread = {Environment.CurrentManagedThreadId}");
                    OnDeleteProcessJob?.Invoke(processJob);
                });
        }

        public void RaiseOnAbortProcessJob(ProcessJob processJob)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnAbortProcessJob)} for PJ {processJob.Name}, thread = {Environment.CurrentManagedThreadId}");
                    OnAbortProcessJob?.Invoke(processJob);
                });
        }

        public void RaiseOnSelectModuleRecipe(
            ProcessJob processJob,
            string moduleId,
            string moduleRecipeId)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnSelectModuleRecipe)} for PJ {processJob.Name}, moduleId = {moduleId}, moduleRecipeId = {moduleRecipeId}, thread = {Environment.CurrentManagedThreadId}");
                    OnSelectModuleRecipe?.Invoke(processJob, moduleId, moduleRecipeId);
                });
        }

        public void RaiseOnPrepareForSubstrateLoad(string moduleId, int eef)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnPrepareForSubstrateLoad)} for moduleId = {moduleId}, eef = {eef}, thread = {Environment.CurrentManagedThreadId}");
                    OnPrepareForSubstrateLoad?.Invoke(moduleId, eef);
                });
        }

        public void RaiseOnPrepareForSubstrateUnload(string moduleId, int eef)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnPrepareForSubstrateUnload)} for moduleId = {moduleId}, eef = {eef}, thread = {Environment.CurrentManagedThreadId}");
                    OnPrepareForSubstrateUnload?.Invoke(moduleId, eef);
                });
        }

        public void RaiseOnSubstrateLoaded(string moduleId, int eef)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnSubstrateLoaded)} for moduleId = {moduleId}, eef = {eef}, thread = {Environment.CurrentManagedThreadId}");
                    OnSubstrateLoaded?.Invoke(moduleId, eef);
                });
        }

        public void RaiseOnSubstrateUnloaded(string moduleId, int eef)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnSubstrateUnloaded)} for moduleId = {moduleId}, eef = {eef}, thread = {Environment.CurrentManagedThreadId}");
                    OnSubstrateUnloaded?.Invoke(moduleId, eef);
                });
        }

        public void RaiseOnExecuteModuleRecipe(
            ProcessJob processJob,
            string moduleId,
            string moduleRecipeId,
            Substrate substrate)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnExecuteModuleRecipe)} for PJ {processJob.Name}, moduleId = {moduleId}, moduleRecipeId = {moduleRecipeId}, substrate = {substrate.Name}, thread = {Environment.CurrentManagedThreadId}");
                    OnExecuteModuleRecipe?.Invoke(processJob, moduleId, moduleRecipeId, substrate);
                });
        }

        public void RaiseOnClose()
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnClose)}, thread = {Environment.CurrentManagedThreadId}");
                    OnClose?.Invoke();
                });
        }

        public void RaiseOnSendDataSetAck_S13F14(ITableDataResponse response)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnSendDataSetAck_S13F14)}, thread = {Environment.CurrentManagedThreadId}");
                    OnSendDataSetAck_S13F14?.Invoke(response);
                });
        }

        public void RaiseOnDataSetRequest_S13F15(ITableDataRequest request)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnDataSetRequest_S13F15)}, thread = {Environment.CurrentManagedThreadId}");
                    OnDataSetRequest_S13F15?.Invoke(request);
                });
        }

        public void RaiseS7F3Changed(
            string flowRecipeName,
            Stream recipeStream,
            ref bool success,
            ref string errorMessage)
        {
            var refSuccess = success;
            var refErrorMessage = errorMessage;
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnS7F3Changed)} for flowRecipe = {flowRecipeName}, thread = {Environment.CurrentManagedThreadId}");
                    var w = new StreamWrapper(recipeStream);
                    OnS7F3Changed?.Invoke(
                        flowRecipeName,
                        w,
                        recipeStream.Length,
                        ref refSuccess,
                        ref refErrorMessage);
                    _logger.Debug(
                        $"{nameof(OnS7F3Changed)} with success = {refSuccess}, errorMessage = {refErrorMessage}, thread = {Environment.CurrentManagedThreadId}");
                });
            success = refSuccess;
            errorMessage = refErrorMessage;
        }

        public void RaiseS7F5RequestPPIDChanged(
            string flowRecipeName,
            MemoryStream recipeStream,
            ref bool success,
            ref string errorMessage)
        {
            var refSuccess = success;
            var refErrorMessage = errorMessage;
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnS7F5RequestPPIDChanged)} for flowRecipe = {flowRecipeName}, thread = {Environment.CurrentManagedThreadId}");
                    var w = new StreamWrapper(recipeStream);
                    OnS7F5RequestPPIDChanged?.Invoke(
                        flowRecipeName,
                        w,
                        ref refSuccess,
                        ref refErrorMessage);
                    _logger.Debug(
                        $"{nameof(OnS7F5RequestPPIDChanged)} with success = {refSuccess}, errorMessage = {refErrorMessage}, thread = {Environment.CurrentManagedThreadId}");
                });
            success = refSuccess;
            errorMessage = refErrorMessage;
        }

        public void RaiseChangeOperationMode(
            OperationMode operationMode,
            ref bool success,
            ref string errorMessage)
        {
            var refSuccess = success;
            var refErrorMessage = errorMessage;
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnChangeOperationMode)} with operationMode = {operationMode}, thread = {Environment.CurrentManagedThreadId}");
                    OnChangeOperationMode?.Invoke(
                        operationMode,
                        ref refSuccess,
                        ref refErrorMessage);
                    _logger.Debug(
                        $"{nameof(OnChangeOperationMode)} with success = {refSuccess}, errorMessage = {refErrorMessage}, thread = {Environment.CurrentManagedThreadId}");
                });
            success = refSuccess;
            errorMessage = refErrorMessage;
        }

        public EquipmentState RaiseOnGetEquipmentState()
        {
            var equipmentState = EquipmentState.Undefined;
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnGetEquipmentState)}, thread = {Environment.CurrentManagedThreadId}");
                    OnGetEquipmentState?.Invoke(ref equipmentState);
                    _logger.Debug(
                        $"{nameof(OnGetEquipmentState)} received with equipmentState = {equipmentState}, thread = {Environment.CurrentManagedThreadId}");
                });
            return equipmentState;
        }

        public void RaiseOnModuleSubstratePresent(string moduleId, Substrate substrate)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnModuleSubstratePresent)} with moduleId = {moduleId} and substrate = {substrate.Name}, thread = {Environment.CurrentManagedThreadId}");
                    OnModuleSubstratePresent?.Invoke(moduleId, substrate);
                });
        }

        public void RaiseOnModuleSubstrateRemoved(string moduleId, Substrate substrate)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnModuleSubstrateRemoved)} with moduleId = {moduleId} and substrate = {substrate.Name}, thread = {Environment.CurrentManagedThreadId}");
                    OnModuleSubstrateRemoved?.Invoke(moduleId, substrate);
                });
        }

        public void RaiseOnSetTransportModuleState(ModuleState moduleState)
        {
            //Commented because seems that it is not needed in ToolControl
            //_logger.Debug(
            //    $"{nameof(OnSetTransportModuleState)} with moduleState = {moduleState}");
            //OnSetTransportModuleState?.Invoke(moduleState);
        }

        public void RaiseOnUTOEquipmentStateChanged(EquipmentState equipmentState)
        {
            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnUTOEquipmentStateChanged)} with equipmentState = {equipmentState}, thread = {Environment.CurrentManagedThreadId}");
                    OnUTOEquipmentStateChanged?.Invoke(equipmentState);
                });
        }

        public void RaiseOnS7F17DeletePPID(
            string[] flowRecipeNames,
            ref bool success,
            ref string errorMessage)
        {
            var refSuccess = success;
            var refErrorMessage = errorMessage;

            _toolControlDispatcher.Invoke(
                () =>
                {
                    _logger.Debug(
                        $"{nameof(OnS7F17DeletePPID)}, thread = {Environment.CurrentManagedThreadId}");
                    OnS7F17DeletePPID?.Invoke(
                        new ComStringList(flowRecipeNames),
                        ref refSuccess,
                        ref refErrorMessage);
                });
            success = refSuccess;
            errorMessage = refErrorMessage;
        }

        #endregion
    }
}
