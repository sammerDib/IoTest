using System;
using System.Threading.Tasks;

using Agileo.Drivers;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Configuration;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Converters;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.EventArgs;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.EFEM.Rorze.Drivers.EventArgs;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.Robot.Converters;
using UnitySC.Equipment.Abstractions.Devices.Robot.Enums;
using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Drivers.Common.Exceptions;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

using OperationMode = UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x
{
    public partial class RR75x : IConfigurableDevice<RR75xConfiguration>, IVersionedDevice
    {
        #region Fields

        private const int NbSpeedLevels = 20;

        #endregion Fields

        #region IGenericDevice Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                base.InternalInitialize(mustForceInit);

                if (OriginReturnCompletion == OriginReturnCompletion.Completed && !mustForceInit)
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.QuickInit(); },
                        RobotCommand.Initialization);
                    return;
                }

                DriverWrapper.RunCommand(
                    delegate { Driver.Initialization(); },
                    RobotCommand.Initialization);
                DriverWrapper.RunCommand(
                    delegate
                    {
                        Driver.SetMotionSpeed(Ratio.FromPercent(Configuration.LastMemorizedSpeed));
                    },
                    RobotCommand.SetMotionSpeed);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion IGenericDevice Commands

        #region IRR75x Commands

        protected virtual void InternalGetStatuses()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.GetStatuses(); },
                    RobotCommand.GetStatuses);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion IRR75x Commands

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DriverWrapper != null)
                {
                    DriverWrapper.Dispose();
                }

                if (Driver != null)
                {
                    Driver.RobotMoved -= Driver_RobotMoved;
                    Driver.StatusReceived -= Driver_StatusReceived;
                    Driver.GpioReceived -= Driver_GpioReceived;
                    Driver.GposReceived -= Driver_GposReceived;
                    Driver.SubstratePresenceChanged -= Driver_SubstratePresenceChanged;
                    Driver.CommunicationEstablished -= Driver_CommunicationEstablished;
                    Driver.CommunicationClosed -= Driver_CommunicationClosed;
                    Driver.CommunicationStarted -= Driver_CommunicationStarted;
                    Driver.CommunicationStopped -= Driver_CommunicationStopped;
                    Driver.VersionReceived -= Driver_VersionReceived;
                    Driver.Dispose();
                }

                CommandExecutionStateChanged -= Robot_CommandExecutionStateChanged;
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable

        #region Properties

        protected RobotDriver Driver { get; set; }
        protected DriverWrapper DriverWrapper { get; set; }
        private SynchronizedRobotWrapper SynchronizedRobotWrapper { get; set; }
        protected IStoppingPositionConverter RobotStoppingPositionConverter { get; set; }

        #endregion Properties

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
            OperationMode = OperationMode.Maintenance;
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
                        RobotStoppingPositionConverter =
                            new RobotStoppingPositionConverter(this, Logger);
                        Driver = new RobotDriver(
                            Logger,
                            Configuration.CommunicationConfig.ConnectionMode,
                            RobotStoppingPositionConverter,
                            aliveBitPeriod: Configuration.CommunicationConfig.AliveBitPeriod);
                        Driver.Setup(
                            Configuration.CommunicationConfig.IpAddress,
                            Configuration.CommunicationConfig.TcpPort,
                            Configuration.CommunicationConfig.AnswerTimeout,
                            Configuration.CommunicationConfig.ConfirmationTimeout,
                            Configuration.CommunicationConfig.InitTimeout,
                            maxNbRetry: Configuration.CommunicationConfig.MaxNbRetry,
                            connectionRetryDelay: Configuration.CommunicationConfig
                                .ConnectionRetryDelay);
                        Driver.RobotMoved += Driver_RobotMoved;
                        Driver.StatusReceived += Driver_StatusReceived;
                        Driver.GpioReceived += Driver_GpioReceived;
                        Driver.GposReceived += Driver_GposReceived;
                        Driver.SubstratePresenceChanged += Driver_SubstratePresenceChanged;
                        Driver.CommunicationEstablished += Driver_CommunicationEstablished;
                        Driver.CommunicationClosed += Driver_CommunicationClosed;
                        Driver.CommunicationStarted += Driver_CommunicationStarted;
                        Driver.CommunicationStopped += Driver_CommunicationStopped;
                        Driver.VersionReceived += Driver_VersionReceived;
                        CommandExecutionStateChanged += Robot_CommandExecutionStateChanged;
                        DriverWrapper = new DriverWrapper(Driver, Logger);
                    }
                    else if (ExecutionMode == ExecutionMode.Simulated)
                    {
                        SetUpSimulatedMode();
                    }

                    SynchronizedRobotWrapper = new SynchronizedRobotWrapper(this);
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication() => Driver.EnableCommunications();

        protected override void InternalStopCommunication() => Driver.Disconnect();

        #endregion ICommunicatingDevice Commands

        #region IRobot Commands

        protected override void InternalClamp(RobotArm arm)
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.SetVacuum(arm, IOState.On); },
                    RobotCommand.Clamp);
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
                DriverWrapper.RunCommand(
                    delegate { Driver.SetVacuum(arm, IOState.Off); },
                    RobotCommand.UnClamp);
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
                InternalClamp(RobotArm.Arm1);
                InternalClamp(RobotArm.Arm2);
                DriverWrapper.RunCommand(delegate { Driver.GoToHome(); }, RobotCommand.GoToHome);
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
                var destinationId = RegisteredLocations[destinationDevice.Name];
                DriverWrapper.RunCommand(
                    delegate { Driver.GoToLocation(destinationId); },
                    RobotCommand.GoToLocation);
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
                DriverWrapper.RunCommand(
                    delegate { Driver.GoToLocation(location, slot, arm); },
                    RobotCommand.GoToLocation);
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
                var destinationId = RegisteredLocations[destinationDevice.Name];

                //Particular case where robot should not move its Z Axis
                if (destinationSlot == byte.MaxValue)
                {
                    destinationSlot = 1;
                }

                DriverWrapper.RunCommand(
                    delegate
                    {
                        Driver.GoToLocation(destinationId, destinationSlot, arm, isPickUpPosition);
                    },
                    RobotCommand.GoToLocation);
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
                var sourceId = RegisteredLocations[sourceDevice.Name];

                //Particular case where robot should not move its Z Axis
                if (sourceSlot == byte.MaxValue)
                {
                    sourceSlot = 1;
                }

                // Prepare pick
                DriverWrapper.RunCommand(
                    delegate { Driver.GoToLocation(sourceId, sourceSlot, arm); },
                    RobotCommand.GoToLocation);

                // Ask confirmation to extend the arm
                var guid = Guid.NewGuid();
                SynchronizedRobotWrapper.Set(
                    guid,
                    delegate
                    {
                        var action = new RobotAction
                        {
                            Command = RobotCommands.Pick,
                            SourceLocation = sourceId,
                            SourceSlotNumber = sourceSlot,
                            DestinationLocation = TransferLocation.Robot,
                            DestinationSlotNumber = 1,
                            ArmLoad = arm
                        };
                        OnCommandConfirmationRequested(
                            new CommandConfirmationRequestedEventArgs(guid, action));
                    });

                // Pick (if no error, extend arm denied...)
                DriverWrapper.RunCommand(
                    delegate { Driver.Pick(sourceId, sourceSlot, arm); },
                    RobotCommand.Pick);
            }
            catch (RobotCommandDeniedException)
            {
                //Special case when the confirmation has been rejected
                //To make the command fail without switching to maintenance
                throw;
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
                var destinationId = RegisteredLocations[destinationDevice.Name];

                //Particular case where robot should not move its Z Axis
                if (destinationSlot == byte.MaxValue)
                {
                    destinationSlot = 1;
                }

                // Prepare place
                DriverWrapper.RunCommand(
                    delegate { Driver.GoToLocation(destinationId, destinationSlot, arm, false); },
                    RobotCommand.GoToLocation);

                // Ask confirmation to extend arm
                var guid = Guid.NewGuid();
                SynchronizedRobotWrapper.Set(
                    guid,
                    delegate
                    {
                        var action = new RobotAction
                        {
                            Command = RobotCommands.Place,
                            SourceLocation = TransferLocation.Robot,
                            SourceSlotNumber = 1,
                            DestinationLocation = destinationId,
                            DestinationSlotNumber = destinationSlot,
                            ArmUnLoad = arm
                        };
                        OnCommandConfirmationRequested(
                            new CommandConfirmationRequestedEventArgs(guid, action));
                    });

                // Place (if no errors, arm extension denied...)
                DriverWrapper.RunCommand(
                    delegate { Driver.Place(destinationId, destinationSlot, arm); },
                    RobotCommand.Place);
            }
            catch (RobotCommandDeniedException)
            {
                //Special case when the confirmation has been rejected
                //To make the command fail without switching to maintenance
                throw;
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
                var sourceId = RegisteredLocations[sourceDevice.Name];

                //Particular case where robot should not move its Z Axis
                if (sourceSlot == byte.MaxValue)
                {
                    sourceSlot = 1;
                }

                var destinationId = RegisteredLocations[destinationDevice.Name];

                //Particular case where robot should not move its Z Axis
                if (destinationSlot == byte.MaxValue)
                {
                    destinationSlot = 1;
                }

                // Swap 
                DriverWrapper.RunCommand(
                    delegate
                    {
                        Driver.Transfer(
                            sourceId,
                            sourceSlot,
                            pickArm,
                            destinationId,
                            destinationSlot,
                            placeArm);
                    },
                    RobotCommand.Transfer);
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
                var sourceId = RegisteredLocations[sourceDevice.Name];

                //Particular case where robot should not move its Z Axis
                if (sourceSlot == byte.MaxValue)
                {
                    sourceSlot = 1;
                }

                // Swap 
                DriverWrapper.RunCommand(
                    delegate { Driver.Swap(sourceId, sourceSlot, pickArm); },
                    RobotCommand.Swap);
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
                //Particular case where robot should not move its Z Axis
                if (slot == byte.MaxValue)
                {
                    slot = 1;
                }

                // Prepare place
                DriverWrapper.RunCommand(
                    delegate { Driver.GoToLocation(location, slot, arm, false); },
                    RobotCommand.GoToLocation);

                // Ask confirmation to extend arm
                var guid = Guid.NewGuid();
                SynchronizedRobotWrapper.Set(
                    guid,
                    delegate
                    {
                        var action = new RobotAction
                        {
                            Command = RobotCommands.Place,
                            SourceLocation = TransferLocation.Robot,
                            SourceSlotNumber = 1,
                            DestinationLocation = location,
                            DestinationSlotNumber = slot,
                            ArmUnLoad = arm
                        };
                        OnCommandConfirmationRequested(
                            new CommandConfirmationRequestedEventArgs(guid, action));
                    });

                // Place (if no errors, arm extension denied...)
                DriverWrapper.RunCommand(
                    delegate { Driver.Extend(location, slot, arm); },
                    RobotCommand.Extend);
            }
            catch (RobotCommandDeniedException)
            {
                //Special case when the confirmation has been rejected
                //To make the command fail without switching to maintenance
                throw;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetMotionSpeed(Ratio percentage)
        {
            try
            {
                ConfigManager.Modified.Cast<RR75xConfiguration>().LastMemorizedSpeed =
                    percentage.Percent;
                ConfigManager.Apply(true);
                DriverWrapper.RunCommand(
                    delegate { Driver.SetMotionSpeed(percentage); },
                    RobotCommand.SetMotionSpeed);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalSetDateAndTime()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.SetDateAndTime(); },
                    RobotCommand.SetDateAndTime);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion IRobot Commands

        #region Command Confirmation

        public override void CommandGranted(Guid commandUuid)
        {
            base.CommandGranted(commandUuid);
            SynchronizedRobotWrapper.CommandGranted(commandUuid);
        }

        public override void CommandDenied(Guid commandUuid)
        {
            base.CommandDenied(commandUuid);
            SynchronizedRobotWrapper.CommandDenied(commandUuid);
        }

        #endregion Command Confirmation

        #region Configuration

        public new RR75xConfiguration Configuration
            => base.Configuration.Cast<RR75xConfiguration>();

        public RR75xConfiguration CreateDefaultConfiguration() => new();

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Equipment.Abstractions.Devices.Robot.Robot)}/{nameof(RR75x)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
            => ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                ConfigurationFileName,
                Logger);

        public string ConfigurationFileName { get; set; } = "Configuration.xml";

        #endregion Configuration

        #region Event Handlers

        private void Driver_RobotMoved(object sender, RobotMovementEventArgs e)
        {
            switch (e.Action.Command)
            {
                case RobotCommands.Pick:
                    LastArmBeingMoved = e.Action.ArmLoad;
                    switch (e.Action.ArmLoad)
                    {
                        case RobotArm.Arm1:
                            UpperArmHistory.Command = RobotCommands.Pick;
                            UpperArmHistory.Location = e.Action.SourceLocation;
                            UpperArmHistory.Slot = e.Action.SourceSlotNumber;
                            break;
                        case RobotArm.Arm2:
                            LowerArmHistory.Command = RobotCommands.Pick;
                            LowerArmHistory.Location = e.Action.SourceLocation;
                            LowerArmHistory.Slot = e.Action.SourceSlotNumber;
                            break;
                    }

                    break;
                case RobotCommands.Place:
                    LastArmBeingMoved = e.Action.ArmUnLoad;
                    switch (e.Action.ArmUnLoad)
                    {
                        case RobotArm.Arm1:
                            UpperArmHistory.Command = RobotCommands.Place;
                            UpperArmHistory.Location = e.Action.DestinationLocation;
                            UpperArmHistory.Slot = e.Action.DestinationSlotNumber;
                            break;
                        case RobotArm.Arm2:
                            LowerArmHistory.Command = RobotCommands.Place;
                            LowerArmHistory.Location = e.Action.DestinationLocation;
                            LowerArmHistory.Slot = e.Action.DestinationSlotNumber;
                            break;
                    }

                    break;
                case RobotCommands.Swap:
                case RobotCommands.Transfer:
                    LastArmBeingMoved = e.Action.ArmUnLoad;
                    switch (e.Action.ArmLoad)
                    {
                        case RobotArm.Arm1:
                            UpperArmHistory.Command = RobotCommands.Pick;
                            UpperArmHistory.Location = e.Action.SourceLocation;
                            UpperArmHistory.Slot = e.Action.SourceSlotNumber;
                            break;
                        case RobotArm.Arm2:
                            LowerArmHistory.Command = RobotCommands.Pick;
                            LowerArmHistory.Location = e.Action.SourceLocation;
                            LowerArmHistory.Slot = e.Action.SourceSlotNumber;
                            break;
                    }

                    switch (e.Action.ArmUnLoad)
                    {
                        case RobotArm.Arm1:
                            UpperArmHistory.Command = RobotCommands.Place;
                            UpperArmHistory.Location = e.Action.DestinationLocation;
                            UpperArmHistory.Slot = e.Action.DestinationSlotNumber;
                            break;
                        case RobotArm.Arm2:
                            LowerArmHistory.Command = RobotCommands.Place;
                            LowerArmHistory.Location = e.Action.DestinationLocation;
                            LowerArmHistory.Slot = e.Action.DestinationSlotNumber;
                            break;
                    }

                    break;
                default:
                    LastArmBeingMoved = RobotArm.Undefined;
                    UpperArmHistory.Command = RobotCommands.Undefined;
                    LowerArmHistory.Command = RobotCommands.Undefined;
                    Logger.Warning(
                        $"{nameof(Driver_RobotMoved)} "
                        + $"- Unexpected command achieved. Could not guarantee the behavior of GWID command. "
                        + $"Achieved command = {e.Action.Command}.");
                    break;
            }
        }

        private void Driver_StatusReceived(object sender, StatusEventArgs<RobotStatus> e)
        {
            Logger.Debug(
                $"Driver's {nameof(Driver.StatusReceived)} event received. Source={e.SourceName}, Status={e.Status}.");
            OperationMode = e.Status.OperationMode;
            OriginReturnCompletion = e.Status.OriginReturnCompletion;
            CommandProcessing = e.Status.CommandProcessing;
            OperationStatus = e.Status.OperationStatus;
            if (e.Status.MotionSpeed == 0)
            {
                IsNormalSpeed = true;
                MotionSpeedPercentage = "100%";
            }
            else
            {
                // When using maintenance mode, be careful about the speed range going up to 200% by 20 steps of 10% instead of 20 steps of 5%.
                IsNormalSpeed = false;
                MotionSpeedPercentage = $"{100 / NbSpeedLevels * e.Status.MotionSpeed}%";
            }

            ErrorControllerCode = ((int)e.Status.ErrorControllerId).ToString("X2");
            ErrorControllerName = e.Status.ErrorControllerId;
            if (ErrorCode != ((int)e.Status.ErrorCode).ToString("X2"))
            {
                if (e.Status.ErrorCode != Robot.RR75x.Driver.Enums.ErrorCode.None)
                {
                    //New alarm detected
                    SetAlarmById(((int)e.Status.ErrorCode + 1000).ToString());
                }
                else
                {
                    //Clear the previously set alarm
                    ClearAlarmById(((int)ErrorDescription + 1000).ToString());
                }
            }

            ErrorCode = ((int)e.Status.ErrorCode).ToString("X2");
            ErrorDescription = e.Status.ErrorCode;

            // Update abstraction statuses
            Speed = Ratio.Parse(MotionSpeedPercentage);
            HasBeenInitialized = OriginReturnCompletion == OriginReturnCompletion.Completed;
            /* Update Generic Statuses from HW data */
            // While a command or a macro-command is executing, we just want to know that it is executing
            // We do not want the detail of knowing each time a part of the command ended
            // (e.g.: Device Initialize = Driver STIM+4*EVNT+... = 1 command)
            if (State != OperatingModes.Executing
                || (OperationStatus == OperationStatus.Stop
                    && CommandProcessing == CommandProcessing.Stop))
            {
                UpdateDeviceState();
            }
        }

        private void Driver_GposReceived(object sender, StatusEventArgs<RobotGposStatus> e)
        {
            Logger.Debug(
                $"Driver's {nameof(Driver.GposReceived)} event received. Source={e.SourceName}, Status={e.Status}.");
            SetRobotPosition(e.Status);
        }

        private void Driver_GpioReceived(object sender, StatusEventArgs<RobotGpioStatus> e)
        {
            Logger.Debug(
                $"Driver's {nameof(Driver.GpioReceived)} event received. Source ={e.SourceName}, Status={e.Status}.");

            // Inputs
            I_EmergencyStop_SignalNotConnected = e.Status.I_EmergencyStop_SignalNotConnected;
            I_Pause_SignalNotConnected = e.Status.I_Pause_SignalNotConnected;
            I_VacuumSourcePressure_SignalNotConnected =
                e.Status.I_VacuumSourcePressure_SignalNotConnected;
            I_AirSourcePressure_SignalNotConnected =
                e.Status.I_AirSourcePressure_SignalNotConnected;
            I_ExhaustFan = e.Status.I_ExhaustFan;
            I_ExhaustFan_ForUpperArm = e.Status.I_ExhaustFan_ForUpperArm;
            I_ExhaustFan_ForLowerArm = e.Status.I_ExhaustFan_ForLowerArm;
            I_UpperArm_Finger1_WaferPresence1 = e.Status.I_UpperArm_Finger1_WaferPresence1;
            I_UpperArm_Finger1_WaferPresence2 = e.Status.I_UpperArm_Finger1_WaferPresence2;
            I_UpperArm_Finger2_WaferPresence1 = e.Status.I_UpperArm_Finger2_WaferPresence1;
            I_UpperArm_Finger2_WaferPresence2 = e.Status.I_UpperArm_Finger2_WaferPresence2;
            I_UpperArm_Finger3_WaferPresence1 = e.Status.I_UpperArm_Finger3_WaferPresence1;
            I_UpperArm_Finger3_WaferPresence2 = e.Status.I_UpperArm_Finger3_WaferPresence2;
            I_UpperArm_Finger4_WaferPresence1 = e.Status.I_UpperArm_Finger4_WaferPresence1;
            I_UpperArm_Finger4_WaferPresence2 = e.Status.I_UpperArm_Finger4_WaferPresence2;
            I_UpperArm_Finger5_WaferPresence1 = e.Status.I_UpperArm_Finger5_WaferPresence1;
            I_UpperArm_Finger5_WaferPresence2 = e.Status.I_UpperArm_Finger5_WaferPresence2;
            I_LowerArm_WaferPresence1 = e.Status.I_LowerArm_WaferPresence1;
            I_LowerArm_WaferPresence2 = e.Status.I_LowerArm_WaferPresence2;
            I_EmergencyStop_TeachingPendant = e.Status.I_EmergencyStop_TeachingPendant;
            I_DeadManSwitch = e.Status.I_DeadManSwitch;
            I_ModeKey = e.Status.I_ModeKey;
            I_InterlockInput00 = e.Status.I_InterlockInput00;
            I_InterlockInput01 = e.Status.I_InterlockInput01;
            I_InterlockInput02 = e.Status.I_InterlockInput02;
            I_InterlockInput03 = e.Status.I_InterlockInput03;
            I_Sensor1ForTeaching = e.Status.I_Sensor1ForTeaching;
            I_Sensor2ForTeaching = e.Status.I_Sensor2ForTeaching;
            I_ExternalInput1 = e.Status.I_ExternalInput1;
            I_ExternalInput2 = e.Status.I_ExternalInput2;
            I_ExternalInput3 = e.Status.I_ExternalInput3;
            I_ExternalInput4 = e.Status.I_ExternalInput4;
            I_ExternalInput5 = e.Status.I_ExternalInput5;
            I_ExternalInput6 = e.Status.I_ExternalInput6;
            I_ExternalInput7 = e.Status.I_ExternalInput7;
            I_ExternalInput8 = e.Status.I_ExternalInput8;
            I_ExternalInput9 = e.Status.I_ExternalInput9;
            I_ExternalInput10 = e.Status.I_ExternalInput10;
            I_ExternalInput11 = e.Status.I_ExternalInput11;
            I_ExternalInput12 = e.Status.I_ExternalInput12;
            I_ExternalInput13 = e.Status.I_ExternalInput13;
            I_ExternalInput14 = e.Status.I_ExternalInput14;
            I_ExternalInput15 = e.Status.I_ExternalInput15;
            I_ExternalInput16 = e.Status.I_ExternalInput16;
            I_ExternalInput17 = e.Status.I_ExternalInput17;
            I_ExternalInput18 = e.Status.I_ExternalInput18;
            I_Sensor1ForTeaching_Ext = e.Status.I_Sensor1ForTeaching_Ext;
            I_Sensor2ForTeaching_Ext = e.Status.I_Sensor2ForTeaching_Ext;

            // Update wafer presence on arm when known presence differs from presence detected by HW
            var isWaferDetectedOnArm1 =
                I_UpperArm_Finger1_WaferPresence1 && I_UpperArm_Finger1_WaferPresence2;
            var isWaferDetectedOnArm2 = I_LowerArm_WaferPresence1 && I_LowerArm_WaferPresence2;
            UpperArmWaferPresence = isWaferDetectedOnArm1
                ? WaferPresence.Present
                : WaferPresence.Absent;
            LowerArmWaferPresence = isWaferDetectedOnArm2
                ? WaferPresence.Present
                : WaferPresence.Absent;

            // Outputs
            O_PreparationComplete_SignalNotConnected =
                e.Status.O_PreparationComplete_SignalNotConnected;
            O_Pause_SignalNotConnected = e.Status.O_Pause_SignalNotConnected;
            O_FatalError_SignalNotConnected = e.Status.O_FatalError_SignalNotConnected;
            O_LightError_SignalNotConnected = e.Status.O_LightError_SignalNotConnected;
            O_ZAxisBrakeOFF_SignalNotConnected = e.Status.O_ZAxisBrakeOFF_SignalNotConnected;
            O_BatteryVoltageTooLow_SignalNotConnected =
                e.Status.O_BatteryVoltageTooLow_SignalNotConnected;
            O_DrivePower_SignalNotConnected = e.Status.O_DrivePower_SignalNotConnected;
            O_TorqueLimitation_SignalNotConnected = e.Status.O_TorqueLimitation_SignalNotConnected;
            O_UpperArm_Finger1_SolenoidValveOn = e.Status.O_UpperArm_Finger1_SolenoidValveOn;
            O_UpperArm_Finger1_SolenoidValveOff = e.Status.O_UpperArm_Finger1_SolenoidValveOff;
            O_UpperArm_Finger2_SolenoidValveOn = e.Status.O_UpperArm_Finger2_SolenoidValveOn;
            O_UpperArm_Finger2_SolenoidValveOff = e.Status.O_UpperArm_Finger2_SolenoidValveOff;
            O_UpperArm_Finger3_SolenoidValveOn = e.Status.O_UpperArm_Finger3_SolenoidValveOn;
            O_UpperArm_Finger3_SolenoidValveOff = e.Status.O_UpperArm_Finger3_SolenoidValveOff;
            O_UpperArm_Finger4_SolenoidValveOn = e.Status.O_UpperArm_Finger4_SolenoidValveOn;
            O_UpperArm_Finger4_SolenoidValveOff = e.Status.O_UpperArm_Finger4_SolenoidValveOff;
            O_UpperArm_Finger5_SolenoidValveOn = e.Status.O_UpperArm_Finger5_SolenoidValveOn;
            O_UpperArm_Finger5_SolenoidValveOff = e.Status.O_UpperArm_Finger5_SolenoidValveOff;
            O_LowerArm_SolenoidValveOn = e.Status.O_LowerArm_SolenoidValveOn;
            O_LowerArm_SolenoidValveOff = e.Status.O_LowerArm_SolenoidValveOff;
            O_XAxis_ExcitationOnOff_LogicSignal = e.Status.O_XAxis_ExcitationOnOff_LogicSignal;
            O_ZAxis_ExcitationOnOff_LogicSignal = e.Status.O_ZAxis_ExcitationOnOff_LogicSignal;
            O_RotationAxisExcitationOnOff_LogicSignal =
                e.Status.O_RotationAxisExcitationOnOff_LogicSignal;
            O_UpperArmExcitationOnOff_LogicSignal = e.Status.O_UpperArmExcitationOnOff_LogicSignal;
            O_LowerArmExcitationOnOff_LogicSignal = e.Status.O_LowerArmExcitationOnOff_LogicSignal;
            O_UpperArmOrigin_LogicSignal = e.Status.O_UpperArmOrigin_LogicSignal;
            O_LowerArmOrigin_LogicSignal = e.Status.O_LowerArmOrigin_LogicSignal;
            O_ExternalOutput1 = e.Status.O_ExternalOutput1;
            O_ExternalOutput2 = e.Status.O_ExternalOutput2;
            O_ExternalOutput3 = e.Status.O_ExternalOutput3;
            O_ExternalOutput4 = e.Status.O_ExternalOutput4;
            O_ExternalOutput5 = e.Status.O_ExternalOutput5;
            O_ExternalOutput6 = e.Status.O_ExternalOutput6;
            O_ExternalOutput7 = e.Status.O_ExternalOutput7;
            O_ExternalOutput8 = e.Status.O_ExternalOutput8;
            O_ExternalOutput9 = e.Status.O_ExternalOutput9;
            O_ExternalOutput10 = e.Status.O_ExternalOutput10;
            O_ExternalOutput11 = e.Status.O_ExternalOutput11;
            O_ExternalOutput12 = e.Status.O_ExternalOutput12;
            O_ExternalOutput13 = e.Status.O_ExternalOutput13;
            O_ExternalOutput14 = e.Status.O_ExternalOutput14;
            O_ExternalOutput15 = e.Status.O_ExternalOutput15;
            O_ExternalOutput16 = e.Status.O_ExternalOutput16;
            O_ExternalOutput17 = e.Status.O_ExternalOutput17;
            O_ExternalOutput18 = e.Status.O_ExternalOutput18;

            UpperArmClamped = O_UpperArm_Finger1_SolenoidValveOn;
            LowerArmClamped = O_LowerArm_SolenoidValveOn;

            UpdateArmState();
        }

        private void Driver_SubstratePresenceChanged(object sender, SubstratePresenceEventArgs args)
        {
            if (args is not SubstratePresenceAndHistoryEventArgs presenceAndHistoryEventArgs
                || presenceAndHistoryEventArgs.Location != TransferLocation.Robot)
            {
                return;
            }

            // If the previous location is DummyPortA and presence is correct, we do not know history for current wafer on arm.
            // However, we have to set minimal data to allow manual wafer recovery.
            if (presenceAndHistoryEventArgs.PreviousLocation == TransferLocation.DummyPortA
                && presenceAndHistoryEventArgs.Presence == SlotState.Correct)
            {
                Logger.Warning(
                    $"Wafer detected on arm{presenceAndHistoryEventArgs.Slot}. History has been lost. Default size of 300mm has been applied to it.");
            }

            var presence = presenceAndHistoryEventArgs.Presence == SlotState.Correct;
            switch (presenceAndHistoryEventArgs.Slot)
            {
                case 1:
                    UpperArmHistory.HasMaterial = presence;

                    // When wafer is present, take previous wafer location from HW instead of from software history
                    if (presence)
                    {
                        UpperArmWaferPresence = WaferPresence.Present;
                        UpperArmHistory.Location = presenceAndHistoryEventArgs.PreviousLocation;
                        UpperArmHistory.Slot = presenceAndHistoryEventArgs.PreviousSlot;
                    }
                    else
                    {
                        UpperArmWaferPresence = WaferPresence.Absent;
                    }

                    break;
                case 2:
                    LowerArmHistory.HasMaterial = presence;

                    // When wafer is present, take previous wafer location from HW instead of from software history
                    if (presence)
                    {
                        LowerArmWaferPresence = WaferPresence.Present;
                        LowerArmHistory.Location = presenceAndHistoryEventArgs.PreviousLocation;
                        LowerArmHistory.Slot = presenceAndHistoryEventArgs.PreviousSlot;
                    }
                    else
                    {
                        LowerArmWaferPresence = WaferPresence.Absent;
                    }

                    break;
            }
        }

        private void Driver_CommunicationEstablished(object sender, EventArgs e)
        {
            IsCommunicationStarted = IsCommunicating = true;
            Task.Factory.StartNew(
                () =>
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.InitializeCommunication(); },
                        RobotCommand.InitializeCommunication);
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetStatuses(); },
                        RobotCommand.GetStatuses);
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetVersion(); },
                        RobotCommand.GetVersion);
                });
        }

        private void Driver_CommunicationClosed(object sender, EventArgs e)
        {
            IsCommunicating = HasBeenInitialized = false;
            SetState(OperatingModes.Maintenance);
        }

        private void Driver_CommunicationStopped(object sender, EventArgs e)
        {
            IsCommunicationStarted = Driver.IsCommunicationStarted;
            SetState(OperatingModes.Maintenance);
        }

        private void Driver_CommunicationStarted(object sender, EventArgs e)
            => IsCommunicationStarted = Driver.IsCommunicationStarted;

        private void Robot_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            // Update device state on command ended
            if (e.PreviousState != ExecutionState.Running)
            {
                return;
            }

            UpdateDeviceState();
        }

        private void Driver_VersionReceived(object sender, VersionAcquisitionEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            Version = e.Version;
        }

        #endregion Event Handlers

        #region Other Methods

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            base.InternalInterrupt(interruption, interruptedExecution);
            if (ExecutionMode != ExecutionMode.Real)
            {
                return;
            }

            DriverWrapper?.InterruptTask();
            Driver.EmergencyStop();
        }

        private void UpdateDeviceState()
        {
            if (!IsCommunicating)
            {
                SetState(OperatingModes.Maintenance);
            }
            else if (ErrorDescription != Robot.RR75x.Driver.Enums.ErrorCode.None)
            {
                SetState(OperatingModes.Maintenance);
            }
            else if (OperationMode == OperationMode.Initializing)
            {
                SetState(OperatingModes.Initialization);
            }
            else if (OperationStatus == OperationStatus.Moving
                     || CommandProcessing == CommandProcessing.Processing)
            {
                SetState(OperatingModes.Executing);
            }
            else if (OperationStatus == OperationStatus.Stop
                     && CommandProcessing == CommandProcessing.Stop)
            {
                SetState(
                    OriginReturnCompletion != OriginReturnCompletion.Completed
                        ? OperatingModes.Maintenance
                        : OperatingModes.Idle);
            }
            else if (OperationStatus == OperationStatus.Pause)
            {
                SetState(OperatingModes.Executing); // Maybe we'll need an additional 'Pause' status
            }
            else
            {
                SetState(
                    OriginReturnCompletion == OriginReturnCompletion.Completed
                        ? OperatingModes.Idle
                        : OperatingModes.Maintenance);
            }
        }

        protected virtual void SetRobotPosition(RobotGposStatus status)
        {
            // Robot is at origin
            if (status.XAxis == 0)
            {
                Position = TransferLocation.DummyPortA;
            }

            // Robot is moving
            else if (status.XAxis == 999 || status.XAxis != status.RotationAxis)
            {
                Position = TransferLocation.Robot;
            }
            else
            {
                Position = RobotStoppingPositionConverter.ToTransferLocation(status.XAxis, false);
            }
        }

        protected virtual void UpdateArmState()
        {
            // Update abstraction statuses
            UpperArmState = O_UpperArmOrigin_LogicSignal
                ? ArmState.Retracted
                : ArmState.Extended;
            LowerArmState = O_LowerArmOrigin_LogicSignal
                ? ArmState.Retracted
                : ArmState.Extended;
        }

        #endregion Other Methods
    }
}
