using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Data.PMProcessingState;
using UnitySC.Shared.TC.Shared.Data.TC_PMStates;
using UnitySC.Shared.TC.Shared.Data.Types.TransferValidation;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.PM.Operations.Implementation
{
    public class PMTransferManager : NotificationTemplate<IPMStateManagerCB>, IPMStateManager, ICommunicationOperationsCB
    {
        private BaseTCPMState _currentTCPMState;
        private BasePMProcessingState _currentPMProcessingState;
        private TransferStateBase _currenTransferState;
        private TransferValidationStateBase _currentTransferValidationState;
        private PMGlobalStates? _currentPMGlobalState = PMGlobalStates.NotInitialized;

        private Material _currentMaterial;
        private IUTOPMOperations _utoPMOperations;

        private IPMTCManager _pmTCService;

        private readonly ILogger _logger;
        private String _recipeNameExecuting = String.Empty;
        private IGlobalStatusServer _globalStatusServer;
        private bool _pmGrantedForMe = false;
        private bool _pmAccessRequestPending = false;
        private bool _startRecipeDetectedAfterTransfer = false;
        private readonly PMConfiguration _pmConfiguration;
        private CThroughtputStatistics _statistics;
        private ICommunicationOperations _communicationOperations;
        private bool _lastGrantedForMe = false;
        private List<Length> _materialDimensionsSupported;

        public CThroughtputStatistics Statistics { get => _statistics; set => _statistics = value; }

        public PMTransferManager()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();

            _pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
            _statistics = new CThroughtputStatistics(null, 10, _pmConfiguration.OutputThroughputDataFullFilePathName);
        }

        public void Init_Status()
        {
            _logger.Information("Initializing PMStateManager...");
            _utoPMOperations = ClassLocator.Default.GetInstance<IUTOPMOperations>();
            _utoPMOperations.SVOperations.InitAllSVIDsToDefaultvalues();
            Register(_utoPMOperations);
            _communicationOperations = ClassLocator.Default.GetInstance<ICommunicationOperations>();

            _globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();

            CurrentTCPMState = new TC_InitializingState();
            CurrentTransferState = new NotReady_State();
            CurrentTransferValidationState = new NotValidated_State();
            CurrentPMProcessingState = new PMP_NotReady();

            _communicationOperations.Init("Server UTO / Client PM", this);
            _communicationOperations.SwitchState = EnableState.Enabled;

            Task.Run(() => InvokeCallback(cb => cb.OnPMStateChanged(_currentTCPMState.State)));
            Task.Run(() => InvokeCallback(cb => cb.OnTransferStateChanged(_currenTransferState.State)));
            Task.Run(() => InvokeCallback(cb => cb.OnMaterialChanged_pmsm(_currentMaterial)));
        }



        public void Init_Services()
        {
            _pmTCService = ClassLocator.Default.GetInstance<IPMTCManager>();
        }


        //public TC_PMState PMStateValue { get => _pmStateObj.State; }
        public BaseTCPMState CurrentTCPMState
        {
            get => _currentTCPMState;
            set
            {
                if (value != null)
                {
                    if ((_currentTCPMState == null) || (_currentTCPMState.State != value.State) || (value.State == TC_PMState.Idle))
                    {
                        _logger.Information($"CurrentTCPMState {value.State}");
                        Task.Run(() => InvokeCallback(cb => cb.OnPMStateChanged(value.State)));
                    }
                }
                _currentTCPMState = value;
            }
        }

        public TC_PMState TCPMState
        {
            get { return CurrentTCPMState.State; }
        }

        public EnumPMTransferState TransferState
        {
            get { return CurrentTransferState.State; }
        }
        public bool CurrentTransferValidated { get => CurrentTransferValidationState.State; }

        public BasePMProcessingState CurrentPMProcessingState
        {
            get => _currentPMProcessingState;
            set
            {
                if ((value != null) && (_currentPMProcessingState != value) || (value.State == PMProcessingStates.Idle))
                {
                    _currentPMProcessingState = value;
                    _logger.Information($"CurrentPMGeneralState {_currentPMProcessingState.State}");
                    CurrentTransferState = CurrentTransferState.ChangeState_ProcessingStateChanged(_currentPMProcessingState.State == PMProcessingStates.Idle);
                    //Task.Run(()=>_utoRecipeOperations.OnPMProcessingStateChanged(_currentPMProcessingState));
                }
            }
        }
        private EnumPMTransferState _lastTansferState = EnumPMTransferState.ReadyToUnload_SlitDoorOpened;
        public TransferStateBase CurrentTransferState
        {
            get => _currenTransferState;
            set
            {
                if ((value != null) && (_currenTransferState != value))
                {
                    _currenTransferState = value;
                    if (_lastTansferState != _currenTransferState.State)
                    {
                        _lastTansferState = CurrentTransferState.State;
                        _logger.Information($"CurrenTransferState {_currenTransferState.State}");
                        Task.Run(() =>
                        {
                            InvokeCallback(cb => cb.OnTransferStateChanged(_currenTransferState.State));
                            if(CurrentTransferValidationState!=null)
                                CurrentTransferValidationState = CurrentTransferValidationState.ChangeState_PMTransferStateChanged(_currenTransferState.State);
                        });
                    };
                }
            }
        }

        private bool _lastTansferValidationState = false;
        public TransferValidationStateBase CurrentTransferValidationState
        {
            get => _currentTransferValidationState;
            set
            {
                if ((value != null) && (_currentTransferValidationState != value))
                {
                    _currentTransferValidationState = value;
                    if (_lastTansferValidationState != _currentTransferValidationState.State)
                    {
                        _lastTansferValidationState = _currentTransferValidationState.State;
                        _logger.Information($"CurrentTransferValidation = {_currentTransferValidationState.State}");
                        Task.Run(() => InvokeCallback(cb => cb.OnTransferValidationStateChanged(_currentTransferValidationState.State)));
                    }
                }
            }
        }

        public PMGlobalStates? CurrentPMGlobalState
        {
            get => _currentPMGlobalState;
            set
            {
                if (_currentPMGlobalState != value)
                {
                    _currentPMGlobalState = value;
                    _logger.Information($"CurrentPMGlobalState {_currentPMGlobalState}");

                    // If not my PM access request, then update PMState to Offline if PMGlobaleState is Busy
                    if (!PMAccessRequestPending && (_currentPMGlobalState == PMGlobalStates.Busy))
                        SetOnlineOffline(true);
                    if (!PMAccessRequestPending && (_currentPMGlobalState == PMGlobalStates.Free))
                        SetOnlineOffline(false);
                }
            }
        }

        public Material CurrentMaterial
        {
            get => _currentMaterial;
            set
            {
                if (_currentMaterial != value)
                {
                    _currentMaterial = value;
                    var materialName = (_currentMaterial == null) ? "None" : _currentMaterial.SubstrateID;
                    _logger.Debug($"CurrentMaterial : {materialName}");
                    Task.Run(() => InvokeCallback(cb => cb.OnMaterialChanged_pmsm(_currentMaterial)));
                }
            }
        }


        public IPMTCManager PMTCService { get => _pmTCService; set => _pmTCService = value; }

        public bool PMAccessRequestPending
        {
            get => _pmAccessRequestPending;
            set
            {
                if (_pmAccessRequestPending != value)
                {
                    _pmAccessRequestPending = value;
                    // If access rejected => offline
                    if (!PMAccessRequestPending && !_pmGrantedForMe && (_currentPMGlobalState == PMGlobalStates.Busy))
                        SetOnlineOffline(true);
                    // If access granted or free => Idle
                    if (!PMAccessRequestPending && _pmGrantedForMe && (_currentPMGlobalState == PMGlobalStates.Busy))
                        SetOnlineOffline(false);
                    if (!PMAccessRequestPending && !_pmGrantedForMe && (_currentPMGlobalState == PMGlobalStates.Free))
                        SetOnlineOffline(false);
                }
            }
        }

        public List<Length> MaterialDimensionsSupported { get => _materialDimensionsSupported; set => _materialDimensionsSupported = value; }

        public virtual void OnPMPreparing()
        {
            // State Management
            CurrentTCPMState = CurrentTCPMState.ChangeState_PrepareProcess();
        }
        public virtual void OnPMProcessing()
        {
            _statistics.SetCheckpoint_ProcessStart(_pmConfiguration.Actor.ToString());

            // State Management
            CurrentTCPMState = CurrentTCPMState.ChangeState_Processing();
            CurrentPMProcessingState = CurrentPMProcessingState.ChangeState_Processing();
        }


        public virtual void OnPMProcessFinished()
        {
            bool isPMGranted = IsPMAccessGranted();
            _statistics.SetCheckpoint_ProcessFinished(_pmConfiguration.Actor.ToString());

            // State Management
            CurrentTCPMState = CurrentTCPMState.ChangeState_ProcessFinisihed();
            CurrentPMProcessingState = CurrentPMProcessingState.ChangeState_ProcessFinisihed();
            if (isPMGranted)
            {
                MoveToLoadingUnloadingPosition_msc(CurrentMaterial);
                _utoPMOperations.NotifyMaterialNeedTransfer();
            }
        }

        public void SetError_GlobalStatus(ErrorID errorID, string msgError)
        {
            Message msg = new Message(errorID, MessageLevel.Error, msgError);
            _globalStatusServer.SetGlobalStatus(new GlobalStatus(msg));

        }

        public void SetGlobalStatus_ControlMode(PMControlMode controlMode)
        {
            

        }

        public void SetPMInCriticalErrorState(ErrorID errorID)
        {
            CurrentPMProcessingState = CurrentPMProcessingState.ChangeState_OnError(errorID); // Set ProcessingState before the TCPMState => because TCPMState is global
            CurrentTCPMState = CurrentTCPMState.ChangeState_OnError(errorID);
        }


        public virtual void SetOnlineOffline(bool offLine)
        {
            if (offLine)
                CurrentTCPMState = CurrentTCPMState.ChangeState_SetPMOffline();
            else
                CurrentTCPMState = CurrentTCPMState.ChangeState_SetPMOnline();
        }
        // Update material presence
        // Hypothesis : Only one material in a PM        
        public void OnMaterialPresenceStateChanged(MaterialPresence materialPresence)
        {
            CurrentTransferState = CurrentTransferState.ChangeState_MaterialPresenceChanged(materialPresence);
            _utoPMOperations.SVOperations.Update_MaterialState(CurrentTransferState.TransferStateData.MaterialPresence);
        }

        public void OnSlitDoorStateChanged(SlitDoorPosition slitDoorState)
        {
            CurrentTransferState = CurrentTransferState.ChangeState_DoorStateChanged(slitDoorState);
        }

        public void OnLoadingUnloadingPositionChanged(bool isInLoadingUnloadingPosition)
        {
            CurrentTransferState = CurrentTransferState.ChangeState_LoadingUnloadingPositionChanged(isInLoadingUnloadingPosition);
        }

        public void OnLocalHardwareGrantedStateChanged(bool granted)
        {
            CurrentTransferState = CurrentTransferState.ChangeState_LocalHardwareGrantedStateChanged(granted);
        }

        public void OnRequestMaterialTransfer_UpdateMaterialDimensionValidation(bool validated)
        {
            CurrentTransferValidationState = CurrentTransferValidationState.ChangeState_MaterialDimensionValidatedChanged(validated);
        }
        // Update Chuck Clamp state
        // Hypothesis : Only one material in a PM        
        public void OnChuckStateChanged_UpdateMaterialClampState( bool materialClamped)
        {
            CurrentTransferValidationState = CurrentTransferValidationState.ChangeState_MaterialClampChanged(materialClamped);
        }

        public void OnInitializationChanged(bool isInitialized)
        {
            if (isInitialized)
            {
                // State Management
                CurrentTCPMState = CurrentTCPMState.ChangeState_InitializationFinished();
                CurrentPMProcessingState = CurrentPMProcessingState.ChangeState_InitializationFinished();

            }
            else
            {
                CurrentTCPMState = CurrentTCPMState.ChangeState_SetNotInitialized();
                CurrentPMProcessingState = CurrentPMProcessingState.ChangeState_SetNotInitialized();
            }
        }

        public void ReInitializationStart()
        {
            OnInitializationChanged(false);
            Task.Run(() =>
            {
                PMTCService.PMInitialization_pmtcs();
                OnInitializationChanged(true);
            });
        }

        public bool ReleasePMReservation()
        {
            try
            {
                PMAccessRequestPending = true;
                bool released = _globalStatusServer.ReleaseLocalHardware();
                if (released)
                {
                    _logger.Information("[PMTransferManager] Hardware released");
                    _pmGrantedForMe = false;
                    _globalStatusServer.SetControlMode(PMControlMode.Engineering);
                    if (_lastGrantedForMe != _pmGrantedForMe)
                    {
                        _lastGrantedForMe = _pmGrantedForMe;
                        OnLocalHardwareGrantedStateChanged(_pmGrantedForMe);
                    }
                }

                return released;
            }
            finally
            {
                PMAccessRequestPending = false;
            }
        }
        public bool RequestPMReservation()
        {            
            try
            {
                PMAccessRequestPending = true;
                _pmGrantedForMe = _globalStatusServer.ReserveLocalHardware();
                if (_pmGrantedForMe)
                {
                    _logger.Information("[PMTransferManager] Hardware reservation");
                    _globalStatusServer.SetControlMode(PMControlMode.Production);
                }
                if (_lastGrantedForMe != _pmGrantedForMe)
                {
                    _lastGrantedForMe = _pmGrantedForMe;
                    OnLocalHardwareGrantedStateChanged(_pmGrantedForMe);
                }
                return _pmGrantedForMe;
            }
            finally
            {
                PMAccessRequestPending = false;
            }
        }

        public bool IsPMAccessGranted()
        {
            return _pmGrantedForMe;
        }

        #region ITCCommunicationControlCallback

        public void CommunicationEstablished()
        {
            CurrentTCPMState = CurrentTCPMState.ChangeState_TCCommunicating(true);
            InvokeCallback(cb => cb.CommunicationEstablished());
        }

        public void CommunicationInterrupted()
        {
            // State Management
            CurrentTCPMState = CurrentTCPMState.ChangeState_TCCommunicating(false);
            if (_pmGrantedForMe)
                ReleasePMReservation();
            InvokeCallback(cb => cb.CommunicationInterrupted());
        }

        public void CommunicationCheck()
        {
            try
            {
                bool isHere = _utoPMOperations.UTOService.AskAreYouThere();
                if (!isHere)
                    _communicationOperations.AttemptCommunicationFailedOrCommunicationLost();
                else
                    _communicationOperations.AttemptCommunicationSucceed();
            }
            catch (Exception)
            {
                _communicationOperations.AttemptCommunicationFailedOrCommunicationLost();
            }
        }

        #endregion ITCCommunicationControlCallback

        #region ITCMaterialServiceCallback

        public void MoveToLoadingUnloadingPosition_msc(MaterialTypeInfo materialTypeInfo)
        {
            PMTCService.MoveToLoadingUnloadingPosition_pmtcs(materialTypeInfo);
        }

        public void OnTransferMaterialStarted_msc()
        {
            _startRecipeDetectedAfterTransfer = false;
            PMTCService.OnTransferMaterialStarted_pmtcs();
        }

        public void OnTransferMaterialFinished_msc(String failedReason)
        {
            if (!string.IsNullOrEmpty(failedReason))
                _logger.Error($"Transfer failed reason = {failedReason}");
            PMTCService.OnTransferMaterialFinished_pmtcs(failedReason);
            _logger.Debug("OnTransferMaterialFinished_msc CurrentTransferState = " + CurrentTransferState.State.ToString());

            if ((TransferState == EnumPMTransferState.ReadyToLoad_SlitDoorClosed) || 
                (TransferState == EnumPMTransferState.ReadyToLoad_SlitDoorOpened))
            {
                // No Material - Release hardware
                ReleasePMReservation();
                _logger.Information("No material - Release local hardware");
            }

            if ((TransferState == EnumPMTransferState.ReadyToUnload_SlitDoorClosed) ||
                (TransferState == EnumPMTransferState.ReadyToUnload_SlitDoorOpened))
            {
                // Waiting for delay before releasing hardware if no start recipe received between
                Task.Run(() =>
                {
                    DateTime startTime = DateTime.Now;
                    bool timeout = false;
                    _logger.Debug("Waiting for delay before releasing hardware [timeout 3 sec]");
                    // Check
                    do
                    {
                        Thread.Sleep(50);
                        timeout = (DateTime.Now.Subtract(startTime).TotalSeconds > 3);
                    } while ((!_startRecipeDetectedAfterTransfer) && (!timeout));

                    if (timeout)
                    {
                        _logger.Debug("No StartRecipe detected in last 3sec - Release local hardware");
                        ReleasePMReservation();
                    }
                    else
                        _logger.Debug("Start recipe detected - Keep local hardware");
                    _startRecipeDetectedAfterTransfer = false;
                });
            }
        }

        public void OnMaterialChanged_msc(Material material)
        {
            CurrentMaterial = material;
            if (material != null)
                _statistics.SetCheckpoint_WaferPlaced(_pmConfiguration.Actor.ToString(), material.SubstrateID);
            else
                _statistics.SetCheckpoint_WaferRemoved(_pmConfiguration.Actor.ToString(), "");
        }

        public void StartRecipeRequest()
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.RecipeStartingError_PMRequestStart);

                _startRecipeDetectedAfterTransfer = true;

                if (CurrentMaterial != null)
                {
                    //TODO: Assume we will be not in laoding position after movement started - Because no event is available
                    OnLoadingUnloadingPositionChanged(false);
                    // Material present
                    _logger.Information("Material - Move chuck in process position");
                    PMTCService.MoveToProcessPosition_pmtcs();

                    if (CurrentPMProcessingState.State == PMProcessingStates.Idle)
                    {
                        Task.Run(() =>
                        {
                            PMTCService.StartRecipeRequest_pmtcs(CurrentMaterial);
                        });
                    }
                    else
                    {
                        string msgError = $"StartRecipe failed reason = PMProcessingState not IDLE";
                        throw new Exception(msgError);
                    }
                }
                else
                {
                    string msgError = $"StartRecipe failed reason = No Material";
                    throw new Exception(msgError);
                }
            }
            catch (Exception ex)
            {
                SetError_GlobalStatus(ErrorID.RecipeStartingError_PMRequestStart, ex.Message);
            }
        }
        // Abort from UTO to PM directly
        public void AbortRecipeRequest()
        {
            if ((CurrentPMProcessingState.State == PMProcessingStates.Processing) && (CurrentMaterial != null))
            {
                PMTCService.AbortRecipeExecution_pmtcs();
            }
        }


        public void UpdateChuckPositionState()
        {
            PMTCService.UpdateChuckPositionState_pmtcs();
        }

        public bool WaitPMAccessForInitialization_Timeout()
        {
            bool timeout_Failed = false;
            DateTime startTime = DateTime.Now;
            bool pmGranted = false;
            _logger.Information("[PMTransferManager] Wait PM acces for initialization");
            while (!timeout_Failed)
            {
                if (CurrentPMGlobalState == PMGlobalStates.ErrorHandling)
                {
                    return true;
                }
                bool statuReady = (_communicationOperations != null) &&
                                ((_communicationOperations.State & ECommunicationState.Communicating) == ECommunicationState.Communicating) &&
                                ((CurrentPMGlobalState == PMGlobalStates.Free) || (CurrentPMGlobalState == PMGlobalStates.Busy));
                if (statuReady)
                {
                    pmGranted = IsPMAccessGranted();
                    if (!pmGranted)
                        pmGranted = RequestPMReservation();
                }
                if (statuReady && pmGranted)
                    break;
                else
                    Thread.Sleep(1000);
                timeout_Failed = DateTime.Now.Subtract(startTime).TotalSeconds > 60;
            }
            if (timeout_Failed)
            {
                if ((_communicationOperations == null) || ((_communicationOperations.State & ECommunicationState.Communicating) != ECommunicationState.Communicating))
                    _logger.Information($"TIMEOUT !! Communication UTO state is not Communicating");
                if ((CurrentPMGlobalState != PMGlobalStates.Free) && (CurrentPMGlobalState != PMGlobalStates.Busy))
                    _logger.Information($"PMGlobalState :{CurrentPMGlobalState.ToString()}");

            }
            return !timeout_Failed;
        }

        public bool PMInitialization()
        {
            try
            {
                bool isok = WaitPMAccessForInitialization_Timeout();
                if (isok)
                {
                    _logger.Information("PM initialisation start");
                    PMTCService.PMInitialization_pmtcs();
                    ResetError();
                    ReleasePMReservation();

                    _logger.Information("PM initialisation complete");
                    return true;
                }
                else
                {
                    string errmsg = "Failure to reserve the PM for initialization.";
                    SetError_GlobalStatus(ErrorID.InitializationError, errmsg);
                    return false;
                }
            }
            catch (Exception ex)
            {
                string errmsg = $"UTO request initialization failed";
                _logger.Error($"{errmsg} Exception = {ex.Message} {ex.StackTrace}");
                SetError_GlobalStatus(ErrorID.InitializationError, errmsg);
                return false;
            }
            finally
            {
                ReleasePMReservation();
            }
        }

        public void UpdatePMProgressInfo(PMProgressInfo pmProgressInfo)
        {
            _utoPMOperations.SVOperations.Update_PMProgressInfo(pmProgressInfo);
        }


        public void ResetError()
        {
            CurrentTCPMState = CurrentTCPMState.ChangeState_OnErrorCleared();
            CurrentPMProcessingState = CurrentPMProcessingState.ChangeState_OnErrorCleared();
        }

        public void OnHandlingError(ErrorID errorId, String msg)
        {
            SetError_GlobalStatus(errorId, msg);
        }

        public void LoadMaterial(Material material)
        {
            if (material == null)
            {
                _logger.Debug("UTO tries to load a material null");
                return;
            }

            if (!material.IsValid)
                _logger.Debug($"UTO loads an invalid material. Material = {material.ToString()}");
            CurrentMaterial = material;

            if ((CurrentMaterial.WaferDimension == null) || (CurrentMaterial.WaferDimension.Value <= 0))
                CurrentMaterial.WaferDimension = new Length(300, LengthUnit.Millimeter);


            if (!PMTCService.Handling.IsSensorWaferPresenceOnChuckAvailable(material.WaferDimension))
                // If no sensor presence, set material present
                PMTCService.SetMaterialPresenceWithoutSensorPresence(material.WaferDimension, MaterialPresence.Present);
        }

        public Material UnloadMaterial()
        {
            if (CurrentMaterial == null) return null;
            Material material = CurrentMaterial.Clone();
            CurrentMaterial = null;

            if (!PMTCService.Handling.IsSensorWaferPresenceOnChuckAvailable(material.WaferDimension))
                // If no sensor presence, set no material anymore                
                PMTCService.SetMaterialPresenceWithoutSensorPresence(material.WaferDimension, MaterialPresence.NotPresent);
            return material;
        }

        #endregion ITCMaterialServiceCallback
    }
}
