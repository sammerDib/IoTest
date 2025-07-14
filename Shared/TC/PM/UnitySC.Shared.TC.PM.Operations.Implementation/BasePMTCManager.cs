using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.Shared.Data;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.Proxy;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Shared.TC.PM.Operations.Implementation
{
    public abstract class BasePMTCManager<TPMRecipe, TPMHandling, TRecipeState> : IPMTCManager, IAlarmOperationsCB
                                where TPMRecipe : PmRecipe
                                where TPMHandling : IHandling
                                where TRecipeState : Enum

    {
        #region Private
        private ILogger _logger;
        private PMTransferManager _pmStateManager;
        private Identity _pmIdentity;
        private DFSupervisor _dfSupervisor;
        private TPMRecipe _currentRecipe { get; set; }
        private PMProgressInfo _currentPMProgressInfo = new PMProgressInfo(1,"");
        private IUTOPMOperations _utoPMOperations;
        private IGlobalStatusServer _globalStatusServer;
        private TRecipeState _currentRecipeState;
        private IMessenger _messenger;
        private string _lastInterlockMsg = String.Empty;
        #endregion Private

        #region Protected
        protected IUTOPMOperations UTOPMOperations { get => _utoPMOperations; set => _utoPMOperations = value; }
        protected DFSupervisor DfSupervisor { get => _dfSupervisor; set => _dfSupervisor = value; }
        protected IGlobalStatusServer GlobalStatusServer { get => _globalStatusServer; set => _globalStatusServer = value; }
        protected PMProgressInfo CurrentPMProgressInfo { get => _currentPMProgressInfo; set => _currentPMProgressInfo = value; }
        protected ILogger Logger { get => _logger; set => _logger = value; }
        protected PMTransferManager PMStateManager { get => _pmStateManager; set => _pmStateManager = value; }
        protected TRecipeState CurrentRecipeState { get => _currentRecipeState; set => _currentRecipeState = value; }
        protected IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                return _messenger;
            }
        }
        protected TPMHandling PMHandling;
        #endregion Protected

        #region IPMTCManager
        public abstract void AbortRecipeExecution_pmtcs();
        public Material CurrentMaterial => PMStateManager.CurrentMaterial;
        public Identity PMIdentity { get => _pmIdentity; set => _pmIdentity = value; }
        public IHandling Handling { get => PMHandling; }
        
        public TPMRecipe CurrentRecipe
        {
            get => _currentRecipe;
            set
            {
                _currentRecipe = value;
            }
        }
        public Guid? CurrentRecipeKey => CurrentRecipe.Key;

        public virtual void Init_Services()
        {
            UTOPMOperations = ClassLocator.Default.GetInstance<IUTOPMOperations>();
            // Finalization PM State manager in remote mode
            PMStateManager.Init_Services();

            // Connection to DF
            DfSupervisor = ClassLocator.Default.GetInstance<DFSupervisor>();
            DfSupervisor.DoInitiateConnection();
            GlobalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            GlobalStatusServer.GlobalStatusChanged += GlobalStatusServer_GlobalStatusChanged;
        }
        public virtual void Init_Status()
        {
            // initialization PM state manager in remote mode (= Using Tool Control as UTO)
            Logger = ClassLocator.Default.GetInstance<ILogger>();
            PMStateManager = ClassLocator.Default.GetInstance<PMTransferManager>();
            PMStateManager.Init_Status();
            Messenger.Register<SendFDCListMessage>(this, (r, m) => { UpdateFDCData(m.FDCsData); });
            Messenger.Register<DeviceErrorMessage>(this, (r, m) => { OnDeviceErrorMessage(m); });
        }

        private void OnDeviceErrorMessage(DeviceErrorMessage errorMessage)
        {
            Logger.Error($"Device error {errorMessage.ErrorID}: {errorMessage.Message}");            
            PMStateManager.SetError_GlobalStatus(errorMessage.ErrorID, errorMessage.Message);
        }

        public virtual void LoadMaterialOnChuck_pmtcs()
        {
            // => Load action finished, Doing material clamp if needed
            // No clamp action here
        }
        public abstract void MoveToLoadingUnloadingPosition_pmtcs(MaterialTypeInfo materialTypeInfo);
        public abstract void MoveToProcessPosition_pmtcs();
        public virtual void OnTransferMaterialStarted_pmtcs()
        {
            Logger.Information("-----------------------------------------------------------------------");
            Logger.Information("OnTransferMaterialStarted");
            UnloadMaterialOnChuck_pmtcs();
        }
        public virtual void OnTransferMaterialFinished_pmtcs(String failedReason)
        {   
            PMHandling.MoveSlitDoor(SlitDoorPosition.ClosePosition);
            
            // Check if material is present - Action
            //----------------------------------------------------------------------------
            // Load material from liftpins if needed
            LoadMaterialOnChuck_pmtcs();
        }

        public virtual void PMInitialization_pmtcs()
        {
            PMHandling.PMInitialization();
        }
        public virtual void RequestAllFDCsUpdate()
        {
            ClassLocator.Default.GetInstance<FDCManager>().RequestAllFDCsUpdate();
        }
        public abstract void StartRecipeExecution_pmtcs(Guid? pmRecipeKey, DataflowRecipeInfo dfRecipeInfo, Material material);
        public virtual void StartRecipeRequest_pmtcs(Material currentMaterial)
        {
            try
            {
                if (currentMaterial != null)
                {
                    _logger.Information($"Start recipe request for material: {CurrentMaterial.ToString()}");
                    if (CurrentMaterial.IsValid)
                        _dfSupervisor.StartRecipeRequest(PMIdentity, currentMaterial);
                    else
                        throw new Exception("Start recipe failed. Material information is Invalid.");
                }
                else
                    throw new Exception("Start recipe failed. Material information is Null.");
            }
            catch (Exception ex)
            {
                string _errorReason = "In Function StartRecipeRequest_pmtcs : " + ex.Message;
                _logger.Error(ex, _errorReason);
                _pmStateManager.SetError_GlobalStatus(ErrorID.RecipeStartingError_PMRequestStart, ex.Message);
                _dfSupervisor.RecipeExecutionComplete(PMIdentity, _pmStateManager.CurrentMaterial, CurrentRecipe?.Key, "Recipe " + CurrentRecipeState.ToString(), RecipeTerminationState.failed);
                _pmStateManager.OnPMProcessFinished();
            }
        }
        public virtual void UnloadMaterialOnChuck_pmtcs()
        {
            // => Pick/Place action, Do material unclamp if needed and clear Recipe selection
            // No Unclamp action here
            // Clear recipe selection
            CurrentRecipe = null;
        }
        public virtual void UpdateChuckPositionState_pmtcs()
        {
            PMHandling.RefreshChuckPositionState();
        }

        public List<Length> GetMaterialDiametersSupported()
        {
            _pmStateManager.MaterialDimensionsSupported = Handling.GetMaterialDiametersSupported(); // Dimension found in configuration and validated by hardware sensors (if available)
            return _pmStateManager.MaterialDimensionsSupported;
        }
        public virtual void SetMaterialPresenceWithoutSensorPresence(Length slotSize, MaterialPresence presence)
        {
            PMHandling.UpdateBackupFileAndApplyWaferPresenceChanged(slotSize, presence);
        }
        #endregion

        #region IAlarmOperationsCB
        public void OnErrorAcknowledged(Identity identity, ErrorID errorId)
        {
            // Acknowledgement for PM only
            // According to errorId, doing action in a switch case if needed            
            switch (errorId)
            {
                case ErrorID.SmokeDetected:
                    PMHandling.ResetSmokeDetectorError();
                    break;
                default:
                    break;
            }
        }
        public void OnErrorReset(Identity identity, ErrorID error)
        {
            PMStateManager.ResetError();
        }
        public void SetCriticalErrorState(Identity identity, ErrorID errorID)
        {
            Logger.Error($"Set PM in CRITICAL Error state - Error = {errorID}");
            PMStateManager.SetPMInCriticalErrorState(errorID);
        }
        # endregion IAlarmOperationsCB

        #region private methodes
        private void GlobalStatusServer_GlobalStatusChanged(GlobalStatus globalStatus)
        {
            PMStateManager.CurrentPMGlobalState = globalStatus.CurrentState;

            if (globalStatus.Messages != null)
            {
                foreach (var message in globalStatus.Messages)
                {
                    // ATTENTION: Uniquement les operations d'urgences peuvent passer directement PM->UTO
                    // SINON privilegier le Dataflow en utilisant DFSupervisor.NotifyError() dans le "Else"
                    if (IsSmokeDetected(message))
                    {
                        UTOPMOperations.AlarmOperations.SetAlarm(PMIdentity, ErrorID.SmokeDetected);
                    }
                    else
                        if (message.Error != ErrorID.Undefined)
                    {
                        DfSupervisor.NotifyError(PMIdentity, message);
                    }

                }
            }
        }
        private bool IsSmokeDetected(Message message)
        {
            return message.Level == MessageLevel.Error && message.UserContent.Contains("Smoke detected");
        }

        private void UpdateFDCData(List<FDCData> fdcsData)
        {
            if (PMIdentity != null)
                _dfSupervisor.NotifyFDCCollectionChanged(PMIdentity, fdcsData);
        }

        #endregion
    }
}
