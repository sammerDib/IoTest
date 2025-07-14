using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Hardware;
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
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.TC
{
    public class AnaHandlingManager : BaseHandlingManager<AnaHardwareManager>, IANAHandling
    {
        public AnaHandlingManager()
        {
        }

        #region IHandling

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
            Task.Run(() =>
            {
                // Get default analyse material
                PMTransferManager pmTransferManager = ClassLocator.Default.GetInstance<PMTransferManager>();
                WaferDimensionalCharacteristic wc = GetDefaultWaferCharacterisitic();

                // Chuck Clamp state
                Logger.Information($"ChuckState state changed : Clamp[{wc.Diameter}] = {state.WaferClampStates[wc.Diameter]}");    
                pmTransferManager.OnChuckStateChanged_UpdateMaterialClampState(state.WaferClampStates[wc.Diameter]);

                // Material presence 
                MaterialPresence materialPresence = CheckWaferPresence(wc.Diameter); // Read waferPresence and Update Subscribers if changed
                Logger.Information($"Chuck State Changed : Presence[{wc.Diameter}] = {materialPresence}");
            });
        }

        public void PMClampMaterial(Material material)
        {
            WaferDimensionalCharacteristic wc = GetDefaultWaferCharacterisitic();
            HardwareManager.ClampHandler.ClampWafer(wc.Diameter);
        }

        public void PMUnclampMaterial(Material material)
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
            WaferDimensionalCharacteristic wafer = new WaferDimensionalCharacteristic();
            wafer.Diameter = new Length(300, LengthUnit.Millimeter);

            HardwareManager.ClampHandler.ReleaseWafer(waferCharacteristic.Diameter);
        }

        public override bool IsChuckInLoadingPosition
        {
            get
            {
                try
                {
                    var waferDiameter = GetDefaultWaferDiameter();
                    if(waferDiameter == null)
                    {                        
                        Logger.Error("IsChuckInProcessPosition - Unable to retrieve wafer diameter. Configuration may not be loaded or no slot configs available.");
                        return false;
                    }
                    var slotConfig = GetSlotConfigByDiameter(waferDiameter);
                    if(slotConfig?.PositionPark == null)
                    {
                        Logger.Error($"IsChuckInLoadingPosition - No position park defined for diameter {waferDiameter}");
                        return false;
                    }
                    return HardwareManager.Axes.IsAtPosition(slotConfig.PositionPark);
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
                    return HardwareManager.Axes.IsAtPosition(slotConfig.PositionManualLoad);
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
                        HardwareManager.Axes.GotoPosition(targetPosition, AxisSpeed.Normal);                        
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
                        HardwareManager.Axes.GotoPosition(targetPosition, AxisSpeed.Normal);                            
                        TestErrors.CheckTestError(ErrorID.StageError, 1);
                        break;

                    default:
                    case ChuckPosition.UnknownPosition:
                        Logger.Error("MoveChuck() - UnknownPosition - Do nothing");
                        return;
                }

                HardwareManager.Axes.WaitMotionEnd(60000);
                //TODO : Temporary until WaitMotionEnd fixed because suspicious currently
                Thread.Sleep(2000);
                Logger.Debug("Movechuck() Motion ended");

                bool positionOk = HardwareManager.Axes.IsAtPosition(targetPosition);
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
            WaferDimensionalCharacteristic wc = GetDefaultWaferCharacterisitic();
            MaterialPresence materialPresent = CheckWaferPresence(wc.Diameter);
            if (materialPresent == MaterialPresence.Present)
            {
                HardwareManager.ClampHandler.ClampWafer(wc.Diameter);
                Logger.Information("Clamp material");
            }
            else if (materialPresent == MaterialPresence.NotPresent)
            {
                HardwareManager.ClampHandler.ReleaseWafer(wc.Diameter);
                Logger.Information("Unclamp material");
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
            WaferDimensionalCharacteristic wafer = new WaferDimensionalCharacteristic();
            wafer.Diameter = new Length(300, LengthUnit.Millimeter);
            return wafer;
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
        #endregion IHandling
    }
}
