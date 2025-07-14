using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using BAI.Systems.Common;
using BAI.Systems.Devices.WaferEngine;
using BAI.Systems.Modules.EFEM;

using UnitsNet;

using UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot.Configuration;
using UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot.Resources;
using UnitySC.Equipment.Abstractions.Devices.Robot.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

namespace UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot
{
    public partial class BrooksRobot : IConfigurableDevice<BrooksRobotConfiguration>
    {
        #region Fields

        private WaferEngineRemoteProxy _robotProxy;
        private EfemProxy _efemProxy;

        #endregion

        #region Properties

        public List<string> MotionProfiles { get; set; }

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            BAI.CTC.ClientInit.ClientLibLoader.InitializeLoader();
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        _efemProxy = Helpers.Helper.GetEfemProxy(this);

                        var robot = _efemProxy.GetDevice(Configuration.BrooksRobotName);
                        if (robot is not WaferEngineRemoteProxy robotProxy)
                        {
                            throw new InvalidOperationException(
                                Messages.RobotNotPresentInEfemConfig);
                        }

                        _robotProxy = robotProxy;
                        _robotProxy.WaferPresenceChanged += RobotProxy_WaferPresenceChanged;
                        _robotProxy.RobotPositionChanged += RobotProxy_RobotPositionChanged;
                        _robotProxy.AlarmGenerated += RobotProxy_AlarmGenerated;
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region IGenericDevice Commands

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            base.InternalInterrupt(interruption, interruptedExecution);
            if (ExecutionMode != ExecutionMode.Real)
            {
                return;
            }

            _robotProxy.Ems();
            if (!_robotProxy.IsOperable())
            {
                HasBeenInitialized = false;
            }
        }

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                //Base init
                base.InternalInitialize(mustForceInit);

                //Device init
                _robotProxy.Initialize();
                _efemProxy.ClearAlarm();
                _robotProxy.SetMotionServo(true);
                var oldProfile = _robotProxy.GetActiveRobotMotionProfile();
                _robotProxy.SetActiveRobotMotionProfile(Configuration.RobotHomeMotionProfile);
                if (!_robotProxy.AllAxesHaveBeenHomed() || mustForceInit)
                {
                    _robotProxy.HomeAllAxes();
                }

                if (!string.IsNullOrEmpty(oldProfile))
                {
                    _robotProxy.SetActiveRobotMotionProfile(oldProfile);
                }

                //Status update
                GetWaferPresence();
                HasBeenInitialized = _robotProxy.IsOperable();
                MotionProfiles = _robotProxy.GetRobotMotionProfiles().ToList();

                //Check device ready
                if (!_robotProxy.IsOperable())
                {
                    throw new InvalidOperationException(Messages.RobotNotOperable);
                }

                if (_robotProxy.IsInMaintenance())
                {
                    throw new InvalidOperationException(Messages.RobotInMaintenance);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            try
            {
                if (!_robotProxy.Connected)
                {
                    _robotProxy.Connect();
                }

                IsCommunicationStarted = true;
                IsCommunicating = true;

                CheckSubstrateDetectionError(RobotArm.Arm1);
                CheckSubstrateDetectionError(RobotArm.Arm2);

                InternalGetMotionProfiles();
                MotionProfile = _robotProxy.GetActiveRobotMotionProfile();

                UpdatePosition(_robotProxy.GetRobotPosition());
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalStopCommunication()
        {
            try
            {
                if (_robotProxy.Connected)
                {
                    _robotProxy.Disconnect();
                }

                IsCommunicationStarted = false;
                IsCommunicating = false;
                HasBeenInitialized = false;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region IRobot Commands

        protected override void InternalClamp(RobotArm arm)
        {
            try
            {
                var endEffector = arm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                _robotProxy.ApplyWaferRestraint(endEffector);
                UpdateClampState(arm);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalUnclamp(RobotArm arm)
        {
            try
            {
                var endEffector = arm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                _robotProxy.ReleaseWaferRestraint(endEffector);
                UpdateClampState(arm);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalGoToHome()
        {
            try
            {
                _robotProxy.HomeAllAxes();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalGoToLocation(IMaterialLocationContainer destinationDevice)
        {
            try
            {
                var destinationLocation = Helpers.Helper.GetBrooksLocation(this,
                    RegisteredLocations[destinationDevice.Name],
                    1);
                if (Configuration.UpperArm.IsEnabled)
                {
                    _robotProxy.MoveToReadyGet(Configuration.UpperArmName, destinationLocation);
                }
                else
                {
                    _robotProxy.MoveToReadyGet(Configuration.LowerArmName, destinationLocation);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalGoToTransferLocation(
            TransferLocation location,
            RobotArm arm,
            byte slot)
        {
            try
            {
                var endEffector = arm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                var fromLocation = Helpers.Helper.GetBrooksLocation(this, location, slot);
                _robotProxy.MoveToReadyGet(endEffector, fromLocation);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalGoToSpecifiedLocation(
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot,
            RobotArm arm,
            bool isPickUpPosition)
        {
            try
            {
                var endEffector = arm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                var fromLocation = Helpers.Helper.GetBrooksLocation(this,
                    RegisteredLocations[destinationDevice.Name],
                    destinationSlot);
                if (isPickUpPosition)
                {
                    _robotProxy.MoveToReadyGet(endEffector, fromLocation);
                }
                else
                {
                    _robotProxy.MoveToReadyPut(endEffector, fromLocation);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalPick(
            RobotArm arm,
            IMaterialLocationContainer sourceDevice,
            byte sourceSlot)
        {
            try
            {
                var endEffector = arm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                var sourceLocation = Helpers.Helper.GetBrooksLocation(this,
                    RegisteredLocations[sourceDevice.Name],
                    sourceSlot);
                _robotProxy.GetWafer(endEffector, sourceLocation);
                GetWaferPresence();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalPlace(
            RobotArm arm,
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot)
        {
            try
            {
                var endEffector = arm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                var destinationLocation = Helpers.Helper.GetBrooksLocation(this,
                    RegisteredLocations[destinationDevice.Name],
                    destinationSlot);
                _robotProxy.PutWafer(endEffector, destinationLocation);
                GetWaferPresence();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalTransfer(
            RobotArm pickArm,
            IMaterialLocationContainer sourceDevice,
            byte sourceSlot,
            RobotArm placeArm,
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot)
        {
            try
            {
                var pickEndEffector = pickArm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                var placeEndEffector = placeArm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                var pickLocation = Helpers.Helper.GetBrooksLocation(this,
                    RegisteredLocations[sourceDevice.Name],
                    sourceSlot);
                var placeLocation = Helpers.Helper.GetBrooksLocation(this,
                    RegisteredLocations[destinationDevice.Name],
                    destinationSlot);
                _robotProxy.GetPutSubstrates(
                    pickEndEffector,
                    pickLocation,
                    placeEndEffector,
                    placeLocation);
                GetWaferPresence();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSwap(
            RobotArm pickArm,
            IMaterialLocationContainer sourceDevice,
            byte sourceSlot)
        {
            try
            {
                var pickEndEffector = pickArm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                var placeEndEffector = pickArm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                var swapLocation = Helpers.Helper.GetBrooksLocation(this,
                    RegisteredLocations[sourceDevice.Name],
                    sourceSlot);
                _robotProxy.GetPutSubstrates(
                    pickEndEffector,
                    swapLocation,
                    placeEndEffector,
                    swapLocation);
                GetWaferPresence();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalExtendArm(
            RobotArm arm,
            TransferLocation location,
            byte slot)
        {
            try
            {
                var endEffector = arm == RobotArm.Arm1
                    ? Configuration.UpperArmName
                    : Configuration.LowerArmName;
                var fromLocation = Helpers.Helper.GetBrooksLocation(this, location, slot);
                _robotProxy.ExtendEndEffecter(
                    endEffector,
                    fromLocation,
                    VerticalOffsetFromWafer.Wafer);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetMotionSpeed(Ratio percentage)
        {
            throw new InvalidOperationException("Not supported");
        }

        protected override void InternalSetDateAndTime()
        {
            try
            {
                _efemProxy.SetControllerLocalTime(DateTime.Now);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        public override void CheckSubstrateDetectionError(RobotArm arm, bool reset = false)
        {
            GetWaferPresence();
            base.CheckSubstrateDetectionError(arm, reset);
        }

        protected virtual void InternalGetMotionProfiles()
        {
            try
            {
                MotionProfiles = _robotProxy.GetRobotMotionProfiles().ToList();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected virtual void InternalSetMotionProfile(string motionProfile)
        {
            try
            {
                _robotProxy.SetActiveRobotMotionProfile(motionProfile);
                MotionProfile = _robotProxy.GetActiveRobotMotionProfile();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region IConfigurableDevice

        public new BrooksRobotConfiguration Configuration
            => ConfigurationExtension.Cast<BrooksRobotConfiguration>(base.Configuration);

        public BrooksRobotConfiguration CreateDefaultConfiguration()
        {
            return new BrooksRobotConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Robot)}/{nameof(BrooksRobot)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(deviceConfigRootPath, Logger);
        }

        #endregion

        #region Event Handlers

        private void RobotProxy_WaferPresenceChanged(
            string module,
            string waferHost,
            WaferPresenceState waferPresence)
        {
            GetWaferPresence();
        }

        private void RobotProxy_RobotPositionChanged(
            [BAI.Internal.DeviceName] string source,
            BAI.Systems.Common.Controls.RoboticPosition position)
        {
            UpdatePosition(position);
        }

        private void RobotProxy_AlarmGenerated(
            [BAI.Internal.DeviceName] string source,
            AlarmLevel level,
            string message)
        {
            if (_robotProxy.IsInMaintenance())
            {
                Interrupt(InterruptionKind.Abort);
            }
        }

        #endregion

        #region Private Methods

        private void UpdateClampState(RobotArm arm)
        {
            switch (arm)
            {
                case RobotArm.Arm1:
                    UpperArmClamped = _robotProxy.GetWaferRestraintState(Configuration.UpperArmName) == LockState.Locked;
                    break;
                case RobotArm.Arm2:
                    LowerArmClamped = _robotProxy.GetWaferRestraintState(Configuration.LowerArmName) == LockState.Locked;
                    break;
                default:
                    UpperArmClamped = _robotProxy.GetWaferRestraintState(Configuration.UpperArmName) == LockState.Locked;
                    LowerArmClamped = _robotProxy.GetWaferRestraintState(Configuration.LowerArmName) == LockState.Locked;
                    break;
            }
        }

        private void UpdatePosition(BAI.Systems.Common.Controls.RoboticPosition position)
        {
            if (Configuration.UpperArm.IsEnabled)
            {
                UpperArmState = Math.Abs(position.Coordinates[Configuration.UpperEndEffectorName].Number) < 1
                    ? ArmState.Retracted
                    : ArmState.Extended;
            }

            if (Configuration.LowerArm.IsEnabled)
            {
                LowerArmState = Math.Abs(position.Coordinates[Configuration.LowerEndEffectorName].Number) < 1
                    ? ArmState.Retracted
                    : ArmState.Extended;
            }

            Position = ToTransferLocation(
                position.Coordinates["X"].Number,
                position.Coordinates["Theta"].Number,
                false);
        }

        private void GetWaferPresence()
        {
            if (Configuration.UpperArm.IsEnabled)
            {
                var upperPresence = _robotProxy.MapWaferPresenceOnHost(Configuration.UpperArmName);
                UpperArmWaferPresence =
                    Helpers.Helper.ConvertPresenceStateToWaferPresence(upperPresence);
            }
            else
            {
                UpperArmWaferPresence = Equipment.Abstractions.Vendor.Devices.Enums.WaferPresence.Absent;
            }

            if(Configuration.LowerArm.IsEnabled)
            {
                var lowerPresence = _robotProxy.MapWaferPresenceOnHost(Configuration.LowerArmName);
                LowerArmWaferPresence =
                    Helpers.Helper.ConvertPresenceStateToWaferPresence(lowerPresence);
            }
            else
            {
                LowerArmWaferPresence = Equipment.Abstractions.Vendor.Devices.Enums.WaferPresence.Absent;
            }
        }

        private TransferLocation ToTransferLocation(double xPosition, double thetaPosition, bool is0BasedIndexing)
        {
            foreach (var moduleStoppingPositions in Configuration.StoppingPositionPerSampleSize.Values)
            {
                foreach (var tupleModuleStoppingPositions in moduleStoppingPositions.StoppingPositionsPerModule)
                {
                    if (Math.Abs(tupleModuleStoppingPositions.Value.XPosition - xPosition) < 1
                        && Math.Abs(tupleModuleStoppingPositions.Value.ThetaPosition - thetaPosition) < 1)
                    {
                        return tupleModuleStoppingPositions.Key;
                    }
                }
            }

            return TransferLocation.Robot;
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                if (_robotProxy != null)
                {
                    _robotProxy.WaferPresenceChanged -= RobotProxy_WaferPresenceChanged;
                    _robotProxy.RobotPositionChanged -= RobotProxy_RobotPositionChanged;
                    _robotProxy.AlarmGenerated -= RobotProxy_AlarmGenerated;
                    _robotProxy.Dispose();
                }

                if (_efemProxy != null)
                {
                    _efemProxy.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
