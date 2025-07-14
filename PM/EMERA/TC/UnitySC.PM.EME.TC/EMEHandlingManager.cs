using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Enum;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.PM.Operations.Implementation;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.Shared;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.TC
{
    public class EMEHandlingManager : BaseHandlingManager<EmeHardwareManager>, IEMEHandling
    {
        private Dictionary<Length, PMSlot> _slots = new Dictionary<Length, PMSlot>(); // Several slots but only one material can be placed in a PM

        public Dictionary<Length, PMSlot> Slots { get => _slots; set => _slots = value; }
        public PMSlot LastSlotInLoadingPosition = null;

        public EMEHandlingManager()
        {
        }
                
        #region IHandling
        public override void Init()
        {
            base.Init();
            try
            {
                Slots.Clear();
                // Update number of slots to manage slot states
                foreach (var subSlot in HardwareManager.Chuck.Configuration.GetSubstrateSlotConfigs())
                {
                    var pmSlot = new PMSlot(subSlot.Diameter, MaterialPresence.NotPresent);                    
                    Slots.Add(subSlot.Diameter, pmSlot);
                    Slots.TryGetValue(subSlot.Diameter, out LastSlotInLoadingPosition);
                }
            }
            catch (Exception ex)
            {
                HandleError(ErrorID.InitializationError, ex, "Init");
                throw;
            }
        }
        public override void ResetHardware()
        {
            if (GlobalStatusServer?.GetGlobalState() == PMGlobalStates.Error)
            {
                HardwareManager.Reset();
            }
            else
            {
                HardwareManager.Axes?.ResetAxis();
            }
        }
        protected override void OnChuckStateChanged(ChuckState state)
        {
            // Wafer presence updated on OnWaferPresenceChanged
        }

        public void PMClampMaterial(Material material)
        {
            if(material != null)
                HardwareManager.ClampHandler.ClampWafer(material.WaferDimension);
        }
        public void PMUnclampMaterial(Material material)
        {
            if (material == null)
            {  
                // No wafer in PM, prepare any slot to be loaded => declamp all slots
                foreach (var pmSlot in Slots.Values)
                    HardwareManager.ClampHandler.ReleaseWafer(pmSlot.Size);
            }else
                HardwareManager.ClampHandler.ReleaseWafer(material.WaferDimension);
        }

        public override bool IsChuckInLoadingPosition
        {
            get
            {
                try
                {
                    bool loadingPosDetected = false;
                    if(Slots == null || Slots.Count <=0)
                    {
                        Logger.Error("IsChuckInProcessPosition - Unable to retrieve wafer diameter. Configuration may not be loaded or no slot configs available.");
                        return false;
                    }
                    // Check all Loading position per size
                    foreach (var size in Slots.Keys)
                    {                       
                        var slotConfig = GetSlotConfigByDiameter(size);
                        if (slotConfig?.PositionPark == null)
                        {
                            Logger.Error($"IsChuckInLoadingPosition - No position park defined for diameter {size}");
                            return false;
                        }
                        if (HardwareManager.MotionAxes.IsAtPosition(slotConfig.PositionPark))
                        {
                            loadingPosDetected = true;
                            break;
                        } 
                    }
                    return loadingPosDetected;
                }
                catch (Exception)
                {
                    return false;
                };
            }
        }

        public override bool IsChuckInProcessPosition
        {
            get
            {
                try
                {
                    var waferDiameter = GetDefaultWaferDiameter();
                    if (waferDiameter == null)
                    {
                        Logger.Error("IsChuckInProcessPosition - Unable to retrieve wafer diameter. Configuration may not be loaded or no slot configs available.");
                        return false;
                    }
                    var slotConfig = GetSlotConfigByDiameter(waferDiameter);
                    if (slotConfig?.PositionManualLoad == null)
                    {
                        Logger.Error($"IsChuckInProcessPosition - No process position defined for diameter {waferDiameter}");
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
        /// <summary>
        /// Move on loading position according to material size - ANALYSE = material info not used
        /// </summary>
        /// <param name="positionRequested"></param>
        /// <param name="materialTypeInfo"></param>
        public override void MoveChuck(ChuckPosition positionRequested, MaterialTypeInfo materialTypeInfo = null)
        {
            PredefinedPosition posRequested = PredefinedPosition.Park;
            PositionBase targetPosition;
            Logger.Information($"MoveChuck() - Start Goto {positionRequested}");
            SubstrateSlotConfig slotConfig = null;
            try
            {
                if (materialTypeInfo != null)
                {
                    slotConfig = HardwareManager?.Chuck?.Configuration.GetSubstrateSlotConfigByWafer(materialTypeInfo.WaferDimension);
                }
                switch (positionRequested)
                {
                    case ChuckPosition.ProcessPosition:
                        posRequested = PredefinedPosition.ManualLoading; // Process position
                        Logger.Debug("MoveChuck() - Call method to go to Process position");
                        if (slotConfig?.PositionManualLoad == null)
                        {
                            throw new Exception($"PositionManualLoad is not defined for the given wafer diameter: {materialTypeInfo.WaferDimension}.");
                        }
                        targetPosition = slotConfig.PositionManualLoad;
                        HardwareManager.MotionAxes.GoToPosition(targetPosition, AxisSpeed.Normal);
                        TestErrors.CheckTestError(ErrorID.StageError, 0);
                        break;

                    case ChuckPosition.LoadingUnloadingPosition:
                        posRequested = PredefinedPosition.Park;
                        Logger.Debug("MoveChuck() - Call method to go to Loading/Unloading position");
                        if (slotConfig?.PositionPark == null)
                        {
                            throw new Exception($"PositionPark is not defined for the given wafer diameter:{materialTypeInfo.WaferDimension}.");
                        }
                        targetPosition = slotConfig.PositionPark;
                        HardwareManager.MotionAxes.GoToPosition(targetPosition, AxisSpeed.Normal);
                        Slots.TryGetValue(materialTypeInfo.WaferDimension, out LastSlotInLoadingPosition);
                        TestErrors.CheckTestError(ErrorID.StageError, 1);
                        break;

                    default:
                    case ChuckPosition.UnknownPosition:
                        Logger.Error("MoveChuck() - UnknownPosition - Do nothing");
                        return;
                }

                HardwareManager.MotionAxes.WaitMotionEnd(60000);
                //TODO : Temporary until WaitMotionEnd fixed because suspicious currently
                Thread.Sleep(2000);
                Logger.Debug("Movechuck() Motion ended");

                bool positionOk = HardwareManager.MotionAxes.IsAtPosition(targetPosition);
                if (!positionOk)
                    throw new Exception("Chuck didn't reach the requesteposition requested [" + positionRequested.ToString() + "]");
                Logger.Debug("Movechuck() Position is OK");

                // Notify IsAtLoadingPosition Changed
                OnPositionStateChanged();
            }
            catch (Exception ex)
            {
                HandleError(ErrorID.StageError, ex, "MoveChuck(" + posRequested.ToString() + ")");
                throw ex;
            }
        }

        public override void CheckWaferPresenceAndClampOrRelease()
        {
            foreach (var slotSize in Slots.Keys)
            {
                MaterialPresence materialPresent = CheckWaferPresence(slotSize);
                if (materialPresent == MaterialPresence.Present)
                {
                    HardwareManager.ClampHandler.ClampWafer(slotSize); // clamp slot only that match to the slotSize 
                    Logger.Information($"Clamp material at slot {slotSize}");
                }
                else if (materialPresent == MaterialPresence.NotPresent)
                {
                    HardwareManager.ClampHandler.ReleaseWafer(slotSize); // unclamp slot only that match to the slotSize 
                    Logger.Information($"Unclamp material slot {slotSize}");
                }
            }
        }

        public override void Dispose()
        {
            Messenger.Unregister<DataAttributesChuckMessage>(this);
            Messenger.Unregister<AxesPositionChangedMessage>(this);
        }

        private WaferDimensionalCharacteristic GetDefaultWaferCharacterisitic()
        {
            var waferCharacteristic = new WaferDimensionalCharacteristic
            {
                WaferShape = WaferShape.Notch,
                Diameter = 300.Millimeters(),
                Category = "1.15",
                DiameterTolerance = null,
                Flats = null,
                Notch = new NotchDimentionalCharacteristic
                {
                    Depth = 1.Millimeters(),
                    Angle = 0.Degrees(),
                    DepthPositiveTolerance = 0.25.Millimeters(),
                    AngleNegativeTolerance = 1.Degrees(),
                    AnglePositiveTolerance = 5.Degrees()
                },
                SampleWidth = null,
                SampleHeight = null
            };
            return waferCharacteristic;
        }
        private Length GetDefaultWaferDiameter()
        {
            try
            {
                return HardwareManager.Chuck.Configuration.GetSubstrateSlotConfigs().FirstOrDefault()?.Diameter;
            }
            catch (Exception ex)
            {
                Logger.Error($"GetDefaultWaferDiameter - Exception occurred: {ex.Message}");
                return null;
            }
        }
        private SubstrateSlotConfig GetSlotConfigByDiameter(Length waferDiameter)
        {
            try
            {
                return HardwareManager.Chuck.Configuration.GetSubstrateSlotConfigByWafer(waferDiameter);
            }
            catch (Exception ex)
            {
                Logger.Error($"GetSlotConfigByDiameter - Exception occurred for diameter {waferDiameter}: {ex.Message}");
                return null;
            }
        }
        

        // Event method from Controller or sw itself (see above)
        protected override void ApplyWaferPresenceChanged(Length size, MaterialPresence waferPresence)
        {
            PMSlot slot = null;
            Slots.TryGetValue(size, out slot);
            if (slot == null)
                PmTransferManager.SetError_GlobalStatus(ErrorID.BadMaterialTypeTransferError, "PM does not support material size specified.");
            else
            {
                // apply sensor wafer presence
                slot.MaterialPresenceState = waferPresence;
                CurrentMaterialPresence = GetGlobaleChuckMaterialPresenceFromSlots(Slots);
               
                // Update PMStateManager
                if ((CurrentMaterialPresence != LastMaterialPresence) || ForceEventForWaferPresenceUpdate)
                {
                    ForceEventForWaferPresenceUpdate = false;
                    LastMaterialPresence = CurrentMaterialPresence;
                    Task.Run(() =>
                    {
                        HandlingManagerCB.InvokeCallback(cb => cb.OnMaterialPresenceStateChanged(CurrentMaterialPresence));
                    });
                }
            }
        }

        private MaterialPresence GetGlobaleChuckMaterialPresenceFromSlots(Dictionary<Length, PMSlot> slots)
        {
            MaterialPresence presenceState = MaterialPresence.NotPresent;
            if (slots.Any(x => x.Value.MaterialPresenceState == MaterialPresence.Present)) // One is present => Material Present
                presenceState = MaterialPresence.Present;
            if (slots.Any(x => x.Value.MaterialPresenceState == MaterialPresence.Unknown)) // One is unknown => Material presence state unknown
                presenceState = MaterialPresence.Unknown;
            return presenceState;
        }
        public override  void PMInitialization()
        {
            Logger.Information("Initializing handling...");
            // Init complete
            HandlingManagerCB.InvokeCallback(i => i.OnInitializationChanged(false));
            ResetHardware();

            // Trigger all PLC devices to re-send all states events = refresh our states from all plc devices
            TriggerAllPLCDevicesToReceiveAllStatesEvent();

            // Slit door closing
            MoveSlitDoor(SlitDoorPosition.ClosePosition);

            // Move chuck in starting position = loading position 
            MoveChuck(ChuckPosition.LoadingUnloadingPosition, new MaterialTypeInfo() { MaterialType = 1, WaferDimension = LastSlotInLoadingPosition.Size });

            Logger.Information("INIT PM STATUS: Chuck is in loading position.");


            // Check presense, update clamp
            CheckWaferPresenceAndClampOrRelease();

            // Check chamber state= door slit state, panels states
            CheckChamberStates();

            // Init complete
            HandlingManagerCB.InvokeCallback(i => i.OnInitializationChanged(true));
        }
        #endregion IHandling
    }
}

