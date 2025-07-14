using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Enum;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Hardware.USPChuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.Shared;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.PM.Operations.Implementation
{
    public abstract class BaseHandlingManager<THarwareManager> : IDisposable, IHandling
                                    where THarwareManager : HardwareManager
    {
        #region Private Members

        private UnitySC.Shared.Logger.ILogger _logger;
        private MaterialPresence _currentMaterialPresence = MaterialPresence.Unknown; // Memorize presence validated only in loading position (= physical position of Sensor)
        private MaterialPresence _lastMaterialPresence = MaterialPresence.Unknown;
        private bool _lastInLoadingPosition;
        private PMConfiguration _pmConfiguration;
        private IGlobalStatusServer _globalStatusServer;
        private bool _forceEventForPositionUpdate = true;
        private bool _forceEventForSlitDoorStateUpdate = true;
        private bool _forceEventForWaferPresenceUpdate = true;
        private SlitDoorPosition _lastSlitDoorState;
        private bool _hwSimulated = false;
        private IMessenger _messenger;
        private NotificationTemplate<IPMHandlingStatesChangedCB> _handlingManagerCB = new NotificationTemplate<IPMHandlingStatesChangedCB>();
        private THarwareManager _hardwareManager;
        private AutoResetEvent _waitingSlitDoorOpened = new AutoResetEvent(false);
        private AutoResetEvent _waitingSlitDoorNotOpened = new AutoResetEvent(false);
        private PMTransferManager _pmTransferManager;

        #endregion Private Members

        #region Properties

        protected IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                return _messenger;
            }
        }
        public PMTransferManager PmTransferManager { get => _pmTransferManager; set => _pmTransferManager = value; }
        protected ILogger Logger { get => _logger; set => _logger = value; }
        protected MaterialPresence CurrentMaterialPresence { get => _currentMaterialPresence; set => _currentMaterialPresence = value; }
        public MaterialPresence LastMaterialPresence { get => _lastMaterialPresence; set => _lastMaterialPresence = value; }
        protected PMConfiguration PmConfiguration { get => _pmConfiguration; set => _pmConfiguration = value; }

        protected IGlobalStatusServer GlobalStatusServer
        {
            get
            {
                if (_globalStatusServer == null)
                    _globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
                return _globalStatusServer;
            }
            set => _globalStatusServer = value;
        }

        protected bool ForceEventForPositionUpdate { get => _forceEventForPositionUpdate; set => _forceEventForPositionUpdate = value; }
        protected bool HwSimulated { get => _hwSimulated; set => _hwSimulated = value; }
        public NotificationTemplate<IPMHandlingStatesChangedCB> HandlingManagerCB { get => _handlingManagerCB; set => _handlingManagerCB = value; }

        public THarwareManager HardwareManager
        {
            get
            {
                if (_hardwareManager == null)
                    _hardwareManager = ClassLocator.Default.GetInstance<THarwareManager>();

                return _hardwareManager;
            }
            set
            {
                _hardwareManager = value;
            }
        }

        #endregion Properties

        #region Constructor

        public BaseHandlingManager()
        {
            PmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
            HwSimulated = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().HardwaresAreSimulated;
            PmTransferManager = ClassLocator.Default.GetInstance<PMTransferManager>();

            Logger = ClassLocator.Default.GetInstance<ILogger<THarwareManager>>();
            Messenger.Register<DataAttributesChuckMessage>(this, (r, m) => { OnChuckStateChanged(m.State); });
            Messenger.Register<AxesPositionChangedMessage>(this, (r, m) => { OnPositionStateChanged(); });
            Messenger.Register<SlitDoorPositionMessage>(this, (r, m) => { OnSlitDoorStatusChanged(m.SlitDoorPosition); });            
            Messenger.Register<WaferPresenceMessage>(this, (r, m) => { OnWaferPresenceChanged(m.Diameter, m.WaferPresence); });
        }

        #endregion Constructor

        #region IHandling

        /// <summary>
        /// Gets the current wafer presence memorized.
        /// Wafer sensor is present in chuck and can be read in loading position only.
        /// If chuck is in loading position, wafer presence is memorized and returned
        /// </summary>
        public MaterialPresence CheckWaferPresence(Length size)
        {
            MaterialPresence materialPresenceRead = MaterialPresence.Unknown;

            if (IsSensorWaferPresenceOnChuckAvailable(size))
            {
                // Read sensor wafer presence from controller
                materialPresenceRead = HardwareManager.MaterialPresenceHandler.CheckWaferPresence(size);
            }
            else
            {
                // No sensor presence
                // Check if material data is present or simulated
                PMTransferManager pmTransferManager = ClassLocator.Default.GetInstance<PMTransferManager>();
                if ((pmTransferManager.CurrentMaterial?.WaferDimension == size) || GetInitMaterialInfo(size) == MaterialPresence.Present)
                    materialPresenceRead = MaterialPresence.Present;
                else
                    materialPresenceRead = MaterialPresence.NotPresent;
            }
            // Call event method to update state according to chuck position
            ApplyWaferPresenceChanged(size, materialPresenceRead);

            return materialPresenceRead;
        }


        protected virtual void OnWaferPresenceChanged(Length size, MaterialPresence waferPresence)
        {
            // In case of sensor does not work and notify bad status - check Presence sensor validation by configuration
            if (IsSensorWaferPresenceOnChuckAvailable(size))
                ApplyWaferPresenceChanged(size, waferPresence);

        }

        // Event method called by controller or sw itself to update material presence on chuck according to chuck position
        protected virtual void ApplyWaferPresenceChanged(Length size, MaterialPresence waferPresence)
        {
            if (IsChuckInLoadingPosition) // Automatic loading position : perhaps need to rename "Park"
            {
                // Sensor wafer presence was read in position then need to be updated 
                CurrentMaterialPresence = waferPresence;
            }
            else
                // keep previous read
                Logger.Information("[" + Thread.CurrentThread.Name + "] Reading wafer presence status not in loading position : " + CurrentMaterialPresence.ToString());

            // Update PMStateManager
            if ((LastMaterialPresence != CurrentMaterialPresence) || _forceEventForWaferPresenceUpdate)
            {
                _forceEventForWaferPresenceUpdate = false;
                LastMaterialPresence = CurrentMaterialPresence;
                Task.Run(() =>
                {
                    HandlingManagerCB.InvokeCallback(cb => cb.OnMaterialPresenceStateChanged(CurrentMaterialPresence));
                });
            }
        }

        private MaterialPresence GetInitMaterialInfo(Length size)
        {
            List<InitMaterialInfo> initMaterialInfos = InitMaterialInfoManagementInFile.GetMaterialInfo();
            var matInfo = initMaterialInfos.FirstOrDefault(x => x.MaterialDimension == size);
            if (matInfo != null)
                return matInfo.MaterialPresence;
            else
                return MaterialPresence.Unknown;
        }

        // Called if SensorWaferPresence on Chuck not Available
        public virtual void UpdateBackupFileAndApplyWaferPresenceChanged(Length size, MaterialPresence presence)
        {
            if (presence == MaterialPresence.NotPresent)
            {
                var materialInfos = InitMaterialInfoManagementInFile.GetMaterialInfo();
                InitMaterialInfoManagementInFile.ResetPresence(materialInfos);
            }else
            {
                var materialInfos = InitMaterialInfoManagementInFile.GetMaterialInfo();
                InitMaterialInfoManagementInFile.SetPresence(size, materialInfos);
            }

            ApplyWaferPresenceChanged(size, presence);
        }
        public abstract void CheckWaferPresenceAndClampOrRelease();

        public abstract void ResetHardware();

        protected abstract void OnChuckStateChanged(ChuckState state);

        public abstract void MoveChuck(ChuckPosition positionRequested, MaterialTypeInfo materialTypeInfo);

        /// <summary>
        /// Initialize All objects used in class.
        /// </summary>
        public virtual void Init()
        {
            try
            {
                Logger.Information("Initializing handling...");
                HandlingManagerCB.InvokeCallback(i => i.OnInitializationChanged(false));
                HardwareManager = ClassLocator.Default.GetInstance<THarwareManager>();

                // Disable SlitDoor if chamber is simulated
                if (HardwareManager.Chamber.Configuration.IsSimulated)                
                    HardwareManager.Chamber.Configuration.SlitDoorConfig.Available = false;
               
                SlitDoorValidationState validationState = GetSlitDoorValidationState();
                switch (validationState)
                {
                    case SlitDoorValidationState.BadConfiguration:
                        Logger.Warning("Slit door is disabled in configuration. Normally Slit door is expected by PM software.");
                        break;
                    case SlitDoorValidationState.InterfaceNotImplemented:
                        Logger.Error("Slit door configuration is invalid. Slit door is enabled but not managed by PM software");
                        break;
                    case SlitDoorValidationState.Operational_DoorAvailable:
                    case SlitDoorValidationState.Operational_DoorDisabled:
                    default:
                        break;
                }                              
            }
            catch (Exception ex)
            {
                HandleError(ErrorID.InitializationError, ex, "Init");
                throw;
            }
        }

        /// <summary>
        /// gets if the system is in loading position
        /// </summary>
        public virtual bool IsChuckInLoadingPosition
        {
            get
            {
                try
                {
                    var slotConfig = HardwareManager.Chuck.Configuration.GetSubstrateSlotConfigByWafer(300.Millimeters());
                    if (slotConfig?.PositionPark == null)
                    {
                        Logger.Error($"IsChuckInLoadingPosition - No process position defined for diameter {300}");
                        return false;
                    }                 

                    return HardwareManager.MotionAxes.IsAtPosition(slotConfig.PositionPark);
                }
                catch (Exception)
                {
                    return false;
                };
            }
        }

        public virtual bool IsChuckInProcessPosition
        {
            get
            {
                try
                {
                    var slotConfig = HardwareManager.Chuck.Configuration.GetSubstrateSlotConfigByWafer(300.Millimeters());
                    if (slotConfig?.PositionManualLoad == null)
                    {
                        Logger.Error($"IsChuckInProcessPosition - No process position defined for diameter {300}");
                        return false;
                    }
                    return HardwareManager.MotionAxes.IsAtPosition(slotConfig.PositionManualLoad);
                }
                catch (Exception)
                {
                    return false;
                };
            }
        }

        public SlitDoorValidationState GetSlitDoorValidationState()
        {            
            SlitDoorValidationState validationState = _hardwareManager.Chamber.Configuration.SlitDoorConfig.Available && (_hardwareManager.Chamber is ISlitDoor) ? SlitDoorValidationState.Operational_DoorAvailable : SlitDoorValidationState.Operational_DoorDisabled;
            if (validationState == SlitDoorValidationState.Operational_DoorDisabled) // If door disabled, we check that it is not a config/implementation error
            {
                if ((!_hardwareManager.Chamber.Configuration.SlitDoorConfig.Available) && (_hardwareManager.Chamber is ISlitDoor))                
                    validationState = SlitDoorValidationState.BadConfiguration;
                if ((_hardwareManager.Chamber.Configuration.SlitDoorConfig.Available) && (!(_hardwareManager.Chamber is ISlitDoor)))
                    validationState = SlitDoorValidationState.InterfaceNotImplemented;
            }
            return validationState;
        }

        public bool IsSensorWaferPresenceOnChuckAvailable(Length size)
        {
            return HardwareManager.Chuck.IsSensorPresenceEnable(size);
        }
        public SlitDoorPosition GetSlitDoorState()
        {
            SlitDoorPosition doorState = SlitDoorPosition.UnknownPosition;
            SlitDoorValidationState doorValidation = GetSlitDoorValidationState();
            switch (doorValidation)
            {
                case SlitDoorValidationState.Operational_DoorAvailable:
                    if (_hardwareManager.Chamber is ISlitDoor chamberSlitDoor)
                        doorState = chamberSlitDoor.SlitDoorState;
                    else
                        doorState = SlitDoorPosition.UnknownPosition;  
                    Logger.Information($"Reading Slit door status - State : {doorState.ToString()}");
                    break;
                case SlitDoorValidationState.Operational_DoorDisabled:
                    doorState = SlitDoorPosition.OpenPosition; // No door in configuration + not implemented => opened
                    break;
                case SlitDoorValidationState.BadConfiguration:
                    _logger.Warning("Slit door configuration is DISABLED on this PM !!! ");
                    doorState = SlitDoorPosition.OpenPosition; // No door in configuration + implemented  = vonlontary disabled is allowed => opened
                    break;
                default:
                case SlitDoorValidationState.InterfaceNotImplemented:
                    _logger.Error("Slit door functionality is not implemented with this PM");
                    doorState = SlitDoorPosition.ClosePosition;
                    break;
            }
            return doorState;            
        }

        public void MoveSlitDoor(SlitDoorPosition requestedState)
        {
            try
            {
                if (GetSlitDoorValidationState() == SlitDoorValidationState.Operational_DoorAvailable)
                {
                    Logger.Verbose($"Start slitdoor action : {requestedState.ToString()}");
                    bool sucessT_TimeoutF = false;
                    _waitingSlitDoorOpened.Reset();
                    _waitingSlitDoorNotOpened.Reset();
                    ISlitDoor slitDoorHandler = (ISlitDoor)_hardwareManager.Chamber; //IsSlitDoorEnabled => Chamber support ISlitdoor

                    if (requestedState == SlitDoorPosition.OpenPosition) 
                    {
                        slitDoorHandler.OpenSlitDoor();
                        Thread.Sleep(200);
                        if (requestedState != GetSlitDoorState())
                            sucessT_TimeoutF = _waitingSlitDoorOpened.WaitOne(HardwareManager.Chamber.Configuration.SlitDoorConfig.OpeningTimeout_ms);
                        else
                            sucessT_TimeoutF = true;
                    }
                    if (requestedState == SlitDoorPosition.ClosePosition) 
                    {
                        slitDoorHandler.CloseSlitDoor();
                        Thread.Sleep(200);
                        if (requestedState != GetSlitDoorState())
                            sucessT_TimeoutF = _waitingSlitDoorNotOpened.WaitOne(HardwareManager.Chamber.Configuration.SlitDoorConfig.ClosingTimeout_ms);
                        else
                            sucessT_TimeoutF = true;
                    }
                    if (!sucessT_TimeoutF)
                        throw new Exception($"Command slit door [{requestedState.ToString()}] failed in TIMEOUT");
                    Logger.Verbose($"End slitdoor action : complete");
                }
            }
            catch (Exception ex)
            {
                string msgErr = $"Command move slit door in {requestedState.ToString()} failed";
                Logger.Debug(msgErr + $" - Exception: {ex.Message + ex.StackTrace}");
                HandleError(ErrorID.SlitDoorError, ex, msgErr);
                Logger.Verbose($"End slitdoor action : ERROR = {msgErr}");
            }
        }

        public bool ForceEventForSlitDoorStateUpdate { get => _forceEventForSlitDoorStateUpdate; set => _forceEventForSlitDoorStateUpdate = value; }
        public bool ForceEventForWaferPresenceUpdate { get => _forceEventForWaferPresenceUpdate; set => _forceEventForWaferPresenceUpdate = value; }

        public virtual void PMInitialization()
        {
            Logger.Information("Initializing handling...");
            // Init complete
            HandlingManagerCB.InvokeCallback(i => i.OnInitializationChanged(false));
            ResetHardware();

            // Trigger all PLC devices to re-send all states events = refresh our states from all plc devices
            TriggerAllPLCDevicesToReceiveAllStatesEvent();

            // check chuck size is correctly detected and selected
            Chuck_SizeVerification();

            // Slit door closing
            MoveSlitDoor(SlitDoorPosition.ClosePosition);

            // Move chuck in starting position = loading position
            MoveChuck(ChuckPosition.LoadingUnloadingPosition, new MaterialTypeInfo() { MaterialType = 1, WaferDimension = new Length(300, LengthUnit.Millimeter)});
            Logger.Information("INIT PM STATUS: Chuck is in loading position.");

            // Check presense, update clamp
            CheckWaferPresenceAndClampOrRelease();

            // Check chamber state= door slit state, panels states
            CheckChamberStates();

            // Init complete
            HandlingManagerCB.InvokeCallback(i => i.OnInitializationChanged(true));
        }

        private void Chuck_SizeVerification()
        {
            
            List<Length> sizes = GetMaterialDiametersSupported();
            if (sizes.Count > 0)
            {
                Length sizeFound = sizes[0]; // First size is default size
                if (HardwareManager.Chuck is IRemovableChuck removableChuck)
                {
                    if (sizes.Count == 1)
                        // One size and only one detected - select it
                        removableChuck.InitChuckSizeDetected(sizes[0]);
                    else
                        // other detection
                        throw new Exception("Several sizes available found for removable chuck.");
                }
            }
            else
                throw new Exception("No size found for chuck.");
            
        }

        protected void TriggerAllPLCDevicesToReceiveAllStatesEvent()
        {
            _hardwareManager.Chamber.TriggerUpdateEvent();
            if(_hardwareManager.Chuck is USPChuckBase chuck)
                chuck.TriggerUpdateEvent();
        }

        protected void CheckChamberStates()
        {
            // PM Slit door state
            CheckSlitDoorState();

            // PM Panels states
            if (_hardwareManager.Chamber is IChamberInterlocks chamberInterlocks)
                chamberInterlocks.CheckInterlocksClosedState();
        }
        /// <summary>
        /// return empty list if error
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private List<Length> GetChuckDimensionSupported(out string errorMessage)
        {
            errorMessage = String.Empty; // no error by default
            Length sizeSupportedFromReaderRFID = null;

            List<Length> sizesSupportedInConfig = HardwareManager.Chuck.GetMaterialDiametersSupported();
            if (HardwareManager.Rfids.FirstOrDefault().Value != null)
            {
                sizeSupportedFromReaderRFID = HardwareManager.Rfids.First().Value.GetTag().Size;

                if (sizeSupportedFromReaderRFID == 0.Millimeters())
                    errorMessage = "Chuck dimension unknown";                    
                else
                if (!sizesSupportedInConfig.Contains(sizeSupportedFromReaderRFID))
                    errorMessage = "Incompatible chuck dimension with current configuration";

                if (sizeSupportedFromReaderRFID != null)
                    return new List<Length>() { sizeSupportedFromReaderRFID };
                else
                    return new List<Length>();
            }
            else
                return sizesSupportedInConfig;
        }

        public virtual void RefreshChuckPositionState()
        {
            ForceEventForPositionUpdate = true;
            OnPositionStateChanged();
        }

        public void Shutdown()
        {
            HardwareManager.Shutdown();
        }

        public void ResetSmokeDetectorError()
        {
            HardwareManager.Plc.SmokeDetectorResetAlarm();
        }

        #endregion IHandling

        /// <summary>
        /// Logs the exception and associates it with the function name where it occurred
        /// </summary>
        /// <param name="ex">the exception</param>
        /// <param name="FunctionName">the function name</param>
        protected void HandleError(ErrorID errorid, Exception ex, String functionName)
        {
            String errorReason = "In Function " + functionName + " : " + ex.Message;
            HandlingManagerCB.InvokeCallback(i => i.OnHandlingError(errorid, errorReason));
        }

        protected void OnPositionStateChanged()
        {
            bool isloadingPos = IsChuckInLoadingPosition;
            Logger.Verbose($"OnPositionStateChanged isLoadingPos : {isloadingPos} , _lastInLoadingPosition : {_lastInLoadingPosition} ");
            if ((_lastInLoadingPosition != isloadingPos) || ForceEventForPositionUpdate)
            {
                ForceEventForPositionUpdate = false;
                _lastInLoadingPosition = isloadingPos;
                Logger.Information("Chuck in loading position: " + _lastInLoadingPosition.ToString());

                Task.Run(() =>
                {
                    if(_hardwareManager.Chuck is IChuckLoadingPosition chuckHandler)
                        chuckHandler.SetChuckInLoadingPosition(isloadingPos); // Signal to chuck that its position is in loading position (Park) or not (the goal is : signal to PLC => signal to SAFETY => signal to EFEM robot)
                    HandlingManagerCB.InvokeCallback(i => i.OnLoadingUnloadingPositionChanged(isloadingPos));
                });
            }
        }

        public virtual void Dispose()
        {
        }

        public virtual void CheckSlitDoorState()
        {
            SlitDoorPosition slitDoorState = SlitDoorPosition.ClosePosition; // If not determinated slit door presence => door is closed for safe
            try
            {
                var validationState = GetSlitDoorValidationState();
                switch (validationState)
                {
                    case SlitDoorValidationState.Operational_DoorAvailable:
                        slitDoorState = GetSlitDoorState();
                        break;
                    case SlitDoorValidationState.BadConfiguration: // Safe, robot hw check door sensor, just warning for configuration
                    case SlitDoorValidationState.Operational_DoorDisabled:
                        slitDoorState = SlitDoorPosition.OpenPosition; // If not enabled => Door slit always opened
                        break;
                    case SlitDoorValidationState.InterfaceNotImplemented:
                    default:
                        slitDoorState = SlitDoorPosition.ClosePosition;
                        break;
                }
            }
            catch 
            {
                Logger.Error("Slit door configuration is invalid.");
                slitDoorState = SlitDoorPosition.ClosePosition;
            }
            ForceEventForSlitDoorStateUpdate = true;
            OnSlitDoorStatusChanged(slitDoorState);
        }

        private void OnSlitDoorStatusChanged(SlitDoorPosition doorState)
        {
            if (doorState == SlitDoorPosition.OpenPosition)
                _waitingSlitDoorOpened.Set();
            else
                _waitingSlitDoorNotOpened.Set();

            if ((_lastSlitDoorState != doorState) || ForceEventForSlitDoorStateUpdate)
            {
                ForceEventForSlitDoorStateUpdate = false;
                _lastSlitDoorState = doorState;
                Task.Run(() =>
                {
                    if((GetSlitDoorValidationState() == SlitDoorValidationState.Operational_DoorAvailable) && (doorState == SlitDoorPosition.OpenPosition))
                        Thread.Sleep(500); // To wait UTO is updated with IO else it is too fast !!
                    HandlingManagerCB.InvokeCallback(i => i.OnSlitDoorStateChanged(doorState));
                });
                Logger.Information("Slit door state : " + doorState.ToString());
            }
        }

        public List<Length> GetMaterialDiametersSupported()
        {
            if (HardwareManager.Chuck != null)
            {
                String msgError = String.Empty;
                var sizesSupported = GetChuckDimensionSupported(out msgError);
                if (sizesSupported.IsNullOrEmpty())
                    HandlingManagerCB.InvokeCallback(i => i.OnHandlingError(ErrorID.SubstrateDimensionIdentificationError, msgError));               
                return sizesSupported;
            }
            else
            {// If no chuck => default Size = 300mm
                List<Length> result = new List<Length>();
                result.Add(new Length(300, LengthUnit.Millimeter));
                return result;
            }
        }
    }
}
