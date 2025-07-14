using System;
using System.Threading;
using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Enum;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Hardware.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.PM.Operations.Implementation;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.TC
{
    public enum PositionChuck { Loading_mm =1, process_mm = 2}
    /// <summary>
    /// Interface implemented by devices that will report wafer statuses
    /// </summary>
    public interface IWaferPositionerInterface
    {
        bool IsInLoadingPosition { get; }
        WaferPositionerStatus Status { get; }
        bool IsClampedWithWafer { get; }
        bool IsUnclampedWithoutWafer { get; }
    }

    public class DMTHandlingManager : BaseHandlingManager<DMTHardwareManager>, IDMTHandling
    {
        private bool _overrideEFEMArmExtendStatus;
        private const int MATERIAL_PRESENCE_TIMEOUT = 5;

        public DMTHandlingManager()
        {
        }

        #region IHandling

        public override void ResetHardware()
        {
            // Nothing to do
        }

        public override void CheckWaferPresenceAndClampOrRelease()
        {
            Length size = HardwareManager.ChuckSizeDetectedHandler.ChuckSizeDetected;

            // Waiting PLCDevices event sent
            WaitPLCDevicesEvent_MaterialPresence();

            MaterialPresence presence = CheckWaferPresence(size); // This check is needed because it allows to update the current state
            Logger.Information($"Wafer {size} is " + presence.ToString());                
         
            // No clamp/Release with PSD chuck
        }

        protected override void OnWaferPresenceChanged(Length size, MaterialPresence waferPresence)
        {
            if ((HardwareManager.ChuckSizeDetectedHandler.ChuckSizeDetected == size) && // accept event only for size detected during initialization => filter other bad status from PLC that chuck is not concerned with
                IsSensorWaferPresenceOnChuckAvailable(size))                            
            {
                ApplyWaferPresenceChanged(size, waferPresence);
            }
        }

        private void WaitPLCDevicesEvent_MaterialPresence()
        {
            DateTime startTime = DateTime.Now;
            bool timeout = false;
            bool refreshed = false;
            // Waiting PLCDevices event sent
            do
            {
                if (HardwareManager.Chuck is USPChuckBase chuck)
                    refreshed = chuck.IsMaterialPresenceRefreshed;
                else
                    refreshed = true;
                if (refreshed) break;
                timeout = DateTime.Now.Subtract(startTime).TotalSeconds >= MATERIAL_PRESENCE_TIMEOUT;
                Thread.Sleep(500);
            } while (!timeout && !refreshed);
        }

        /// <summary>
        /// Move on loading position according to material size DEMETER = no material info used
        /// </summary>
        /// <param name="positionRequested"></param>
        public override void MoveChuck(ChuckPosition positionRequested, MaterialTypeInfo materialTypeInfo)
        {
            PredefinedPosition posRequested = PredefinedPosition.Park;
            Length positionDestination = new Length((double)PositionChuck.Loading_mm, LengthUnit.Millimeter);
            try
            {
                switch (positionRequested)
                {
                    case ChuckPosition.ProcessPosition:
                        posRequested = PredefinedPosition.ManualLoading; // Process position
                        positionDestination = new Length((double)PositionChuck.process_mm, LengthUnit.Millimeter);
                        break;

                    case ChuckPosition.LoadingUnloadingPosition:
                        posRequested = PredefinedPosition.Park;
                        positionDestination = new Length((double)PositionChuck.Loading_mm, LengthUnit.Millimeter);
                        break;

                    default:
                    case ChuckPosition.UnknownPosition:
                        return;
                }

                HardwareManager.MotionAxes.Move(new PMAxisMove("Linear", positionDestination));

                DateTime startTime = DateTime.Now;
                XTPosition currentPosition = (XTPosition)HardwareManager.MotionAxes.GetPosition();
                XTPosition pos = currentPosition;
                bool timeout = false;
                while ((pos.X.Millimeters() != positionDestination) && (!timeout))
                {
                    pos = (XTPosition)HardwareManager.MotionAxes.GetPosition();
                    if (pos.X.Millimeters() != currentPosition.X.Millimeters())
                    {
                        currentPosition = pos;
                        OnPositionStateChanged();
                    }
                    Thread.Sleep(1000);
                    timeout = (DateTime.Now.Subtract(startTime).TotalSeconds > 20);
                }
                if (timeout) throw new Exception("Chuck move is in timeout !");


                OnPositionStateChanged();
            }
            catch (Exception ex)
            {
                HandleError(ErrorID.StageError, ex, "MoveChuck(" + posRequested.ToString() + ")");
            }
        }


        protected override void OnChuckStateChanged(ChuckState state)
        {
            //throw new NotImplementedException();
        }

        public override bool IsChuckInLoadingPosition
        {
            get
            {
                try
                {
                    XTPosition pos = (XTPosition)HardwareManager.MotionAxes.GetPosition();
                    Length loadingPos = new Length((double)PositionChuck.Loading_mm,  LengthUnit.Millimeter);
                    return (pos.X.Millimeters() == loadingPos);  
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
                    XTPosition pos = (XTPosition)HardwareManager.MotionAxes.GetPosition();
                    Length processPos = new Length((double)PositionChuck.process_mm, LengthUnit.Millimeter);
                    return (pos.X.Millimeters() == processPos);
                }
                catch (Exception)
                {
                    return false;
                };
            }
        }

        #endregion
    }
}
