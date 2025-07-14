using System;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.PM.Operations.Implementation
{
    public class MaterialOperations : IMaterialOperations
    {
        private IPMStateManager _pmStatesManager;
        private ILogger _logger;
        private string _lastFailedReason;
        private TC_Transferstatus _materialTransferState = TC_Transferstatus.Complete;
        private IUTOPMServiceCB _utoService;
        private bool _hwSimulated;
        private bool _requestMovingChuckAlreadyPending = false;

        public MaterialOperations()
        {
        }

        public void Init()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            _pmStatesManager = ClassLocator.Default.GetInstance<IPMStateManager>();
            _utoService = ClassLocator.Default.GetInstance<IUTOPMServiceCB>();

            _hwSimulated = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().HardwaresAreSimulated;
        }

        public bool PrepareForTransfer(TransferType transferType, MaterialTypeInfo materialTypeInfo)
        {
            CheckReadyState currentState = CheckReadyState.eFailedError;
            if (materialTypeInfo == null)
            {
                _logger.Debug("MaterialTypeInfo is null !!");
            }
            bool isMaterialTypeMatching = _pmStatesManager.MaterialDimensionsSupported.Contains(materialTypeInfo.WaferDimension);
            _pmStatesManager.OnRequestMaterialTransfer_UpdateMaterialDimensionValidation(isMaterialTypeMatching);

            if (!isMaterialTypeMatching)
            {
                currentState = CheckReadyState.eFailedError;
                _logger.Debug("PM acess is not granted");
                if(!isMaterialTypeMatching)
                    _pmStatesManager.SetError_GlobalStatus(ErrorID.BadMaterialTypeTransferError, "PM is not granted. PM does not support Material type specified from EFEM transfer request.");
            }
            else
            {
                _logger.Debug("New transfer transaction requested");
                _materialTransferState = TC_Transferstatus.Requested;

                _lastFailedReason = String.Empty;
                DateTime startTime = DateTime.Now;
                bool timeout = false;
                bool pmGranted = false;
                while (!timeout)
                {
                    pmGranted = _pmStatesManager.IsPMAccessGranted();
                    if (!pmGranted)
                        pmGranted = _pmStatesManager.RequestPMReservation();
                    else
                        break;
                    Thread.Sleep(1000);
                    timeout = (DateTime.Now.Subtract(startTime).TotalSeconds > API_Consts.TRANSFER_TIMEOUT);
                }
                if (pmGranted)
                {
                    currentState = CheckPMLoadingUnloadingState(transferType, out _lastFailedReason);
                    if (currentState != CheckReadyState.eReady)
                    {
                        // Request chuck in Loading/Unloading position
                        StartRequest_MoveChuckInLoadingPosition(transferType, materialTypeInfo);
                        currentState = CheckPMLoadingUnloadingState(transferType, out _lastFailedReason);
                    }
                    else
                    {
                        _logger.Debug("State ready confirmed to TC.");
                        _materialTransferState = TC_Transferstatus.Complete;
                    }
                }
                else
                {
                    _materialTransferState = TC_Transferstatus.Canceled;
                    if (String.IsNullOrEmpty(_lastFailedReason))
                        _lastFailedReason = "PM acess is not granted";
                }
            }
            if (currentState == CheckReadyState.eReady)
            {
                _pmStatesManager.OnTransferMaterialStarted_msc();
            }
            return (currentState == CheckReadyState.eReady);
        }

        private void StartRequest_MoveChuckInLoadingPosition(TransferType transferType, MaterialTypeInfo materialTypeInfo)
        {
            if (!_requestMovingChuckAlreadyPending)
            {
                _requestMovingChuckAlreadyPending = true;
                //Task.Run(() =>
                //{
                    try
                    {
                        _logger.Debug("Start Request MoveChuckInLoadingPosition");
                        bool bStop = false;
                        bool requestAlreadyPending = false;
                        DateTime startTime = DateTime.Now;
                        bool timeout = false;
                        CheckReadyState checkState = CheckReadyState.eFailedError;
                        while (!timeout && !bStop)
                        {
                            checkState = CheckPMLoadingUnloadingState(transferType, out _lastFailedReason);
                            _logger.Debug($"CheckState ={checkState.ToString()}");
                            // Check PM state before sending ReadyForTransfer OR Request to have state needed OR Simply NotReady
                            switch (checkState)
                            {
                                case CheckReadyState.eReady:
                                    _logger.Debug("Chuck in loading/unloading position. State ready for transfer");
                                    break;

                                case CheckReadyState.eRequestNeeded:
                                    if (!requestAlreadyPending)
                                    {
                                        requestAlreadyPending = true;
                                        // Check PM state before sending ReadyForTransfer or NotReady
                                        _logger.Debug("Goto to Loading/Unloading position");
                                        // Request PM Loading/Unloading position
                                        try
                                        {
                                            _pmStatesManager.MoveToLoadingUnloadingPosition_msc(materialTypeInfo);
                                        }
                                        catch (Exception)
                                        {
                                            _pmStatesManager.SetError_GlobalStatus(ErrorID.MaterialTransferError, "Chuck movement failed");
                                        }
                                    }
                                    break;

                                default:
                                case CheckReadyState.eFailedError:
                                    _logger.Debug("Request loading/unloading position for transfer failed. " + _lastFailedReason);
                                    break;
                            }
                            bStop = (checkState == CheckReadyState.eReady) || (checkState == CheckReadyState.eFailedError) || (_pmStatesManager.TCPMState == TC_PMState.Error);
                            timeout = (DateTime.Now.Subtract(startTime).TotalSeconds > API_Consts.LOADING_POSITION_REQUEST_TIMEOUT);
                            Thread.Sleep(200);
                        }
                        if (timeout)
                        {
                            _logger.Debug("TIMEOUT Request loading/unloading position for transfer " + _lastFailedReason);
                        }
                    }
                    finally
                    {
                        _requestMovingChuckAlreadyPending = false;

                        _logger.Debug($"End Request MoveChuckInLoadingPosition");
                }
                //});
            }
        }

        public void LoadMaterial(Material material)
        {
            _pmStatesManager.LoadMaterial(material);            
        }

        public Material UnloadMaterial()
        {
            return _pmStatesManager.UnloadMaterial();
        }

        public void PostTransfer()
        {
            _logger.Debug("[Recv] MaterialService - Post transfer");

            Thread.CurrentThread.Name = "Request_PostTransfer #" + Task.CurrentId.ToString();
            _pmStatesManager.OnTransferMaterialFinished_msc(_lastFailedReason);
        }

        public void StartRecipe()
        {
            _logger.Debug("[Recv] MaterialService - Start Recipe");
            _pmStatesManager.StartRecipeRequest();
        }

        public void AbortRecipe()
        {
            _logger.Debug("[Recv] MaterialService - Abort Recipe");
            _pmStatesManager.AbortRecipeRequest();
        }

        public void StartTransfer()
        {
        }

        private CheckReadyState CheckPMLoadingUnloadingState(TransferType transfertType, out String failedReason)
        {
            CheckReadyState result = CheckReadyState.eFailedError;
            failedReason = "";
            _pmStatesManager.UpdateChuckPositionState();
            Thread.Sleep(200);

            if (_pmStatesManager.TCPMState != TC_PMState.Offline)
            {
                switch (_pmStatesManager.TransferState)
                {
                    case EnumPMTransferState.ReadyToLoad_SlitDoorOpened:
                        if (transfertType == TransferType.Place)
                            result = CheckReadyState.eReady;
                        else
                        { result = CheckReadyState.eFailedError; failedReason = "no Material is present"; }
                        break;

                    case EnumPMTransferState.ReadyToUnload_SlitDoorOpened:
                        if (transfertType == TransferType.Pick)
                            result = CheckReadyState.eReady;
                        else
                        { result = CheckReadyState.eFailedError; failedReason = "Material is already present"; }
                        break;

                    case EnumPMTransferState.ReadyToLoad_SlitDoorClosed:
                        failedReason = "PM door is closed";
                        goto default;
                    case EnumPMTransferState.ReadyToUnload_SlitDoorClosed:
                        failedReason = "PM door is closed";
                        goto default;
                    case EnumPMTransferState.NotReady:
                        failedReason = "PM is not ready";
                        goto default;
                    default:
                        result = CheckReadyState.eRequestNeeded;
                        break;
                }
            }
            else
            { result = CheckReadyState.eFailedError; failedReason = "Process module is offline"; }
            return result;
        }

        public void ResetTransferInProgress()
        {
            if ((_materialTransferState != TC_Transferstatus.Canceled) && (_materialTransferState != TC_Transferstatus.Complete))
            {
                _logger.Debug("Reset transfer in progress");
                _materialTransferState = TC_Transferstatus.Canceled;
            }
        }

        public bool PMInitialization()
        {
            return _pmStatesManager.PMInitialization();
        }
    }
}
