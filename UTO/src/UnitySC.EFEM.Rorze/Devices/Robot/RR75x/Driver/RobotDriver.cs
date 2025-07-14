using System;
using System.Globalization;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.EventArgs;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.EFEM.Rorze.Drivers.EventArgs;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Robot.Converters;
using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Drivers.Common.PostmanCommands;

using MappingEventArgs = UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.EventArgs.MappingEventArgs;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver
{
    /// <summary>
    /// Class responsible to communicate with the RORZE robot model RR754 through RR757.
    /// </summary>
    public class RobotDriver : DriverBase
    {
        #region Fields

        private readonly IStoppingPositionConverter _stoppingPositionConverter;

        private readonly IMacroCommandSubscriber _commandsSubscriber;

        private readonly IMacroCommandSubscriber _stateChangedSubscriber;
        private readonly IMacroCommandSubscriber _gpioReceivedSubscriber;
        private readonly IMacroCommandSubscriber _waferPresenceReceivedSubscriber;
        private readonly IMacroCommandSubscriber _gposReceivedSubscriber;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RobotDriver"/> class.
        /// </summary>
        /// <param name="logger">The logger to use to trace any information</param>
        /// <param name="connectionMode">Indicates which connection mode the driver must have (client or server).</param>
        /// <param name="stoppingPositionConverter">Used to convert EFEM Controller in robot HW formatted data and reverse.</param>
        /// <param name="port">Port's number of the device.</param>
        /// <param name="aliveBitPeriod">Contains alive bit request period</param>
        public RobotDriver(
            ILogger logger,
            ConnectionMode connectionMode,
            IStoppingPositionConverter stoppingPositionConverter,
            byte port = 1,
            double aliveBitPeriod = 1000)
            : base(logger, nameof(Equipment.Abstractions.Devices.Robot.Robot), connectionMode, port, RorzeConstants.DeviceTypeAbb.Robot, aliveBitPeriod)
        {
            _stoppingPositionConverter = stoppingPositionConverter;

            _commandsSubscriber              = AddReplySubscriber(SubscriberType.SenderAndListener);
            _stateChangedSubscriber          = AddReplySubscriber(SubscriberType.ListenForEverything);
            _gpioReceivedSubscriber          = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _waferPresenceReceivedSubscriber = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
            _gposReceivedSubscriber          = AddReplySubscriber(SubscriberType.ListenForParticularMessage);
        }

        #endregion Constructors

        #region Commands to Hardware

        /// <summary>
        /// Enable robot events and reset error.
        /// </summary>
        public void InitializeCommunication()
        {
            var macroCommand = BuildInitMacroCommand((int)EFEMEvents.InitializeCommunicationCompleted);

            // Send the command.
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Picks a substrate from designated position.
        /// </summary>
        /// <param name="location">The substrate source location.</param>
        /// <param name="slotNumber">The substrate source slot number.</param>
        /// <param name="arm">Designates an arm which is used for loading.</param>
        /// <param name="isToReturnArmInOriginPosition">Designate the position of movement completion:
        /// if set to <c>true</c> the arm is retracted to the origin position and stops after substrate is picked up;
        /// otherwise the arm stops after substrate is picked up.</param>
        /// <remarks>Send LOAD Rorze message</remarks>
        public void Pick(
            TransferLocation location,
            byte slotNumber,
            RobotArm arm                       = RobotArm.Arm1,
            bool isToReturnArmInOriginPosition = true)
        {
            // Update Robot's Data
            RobotAction action = new RobotAction
            {
                Command               = RobotCommands.Pick,
                ArmLoad               = arm,
                SourceLocation        = location,
                SourceSlotNumber      = slotNumber,
                DestinationLocation   = TransferLocation.Robot,
                DestinationSlotNumber = 1
            };

            // Retrieve command parameters
            ArmOrBoth_Interpolated commandArm;
            switch (arm)
            {
                case RobotArm.Arm1:
                    commandArm = ArmOrBoth_Interpolated.UpperArm;
                    break;

                case RobotArm.Arm2:
                    commandArm = ArmOrBoth_Interpolated.LowerArm;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(arm), arm, string.Empty);
            }

            var stg = (uint)_stoppingPositionConverter.ToStoppingPosition(location, arm, false);

            // Create the command
            var loadCommand = LoadWaferCommand.NewOrder(
                Port,
                commandArm,
                stg,
                slotNumber,
                Sender,
                this,
                isToReturnArmInOriginPosition ? MotionType.NormalMotion : MotionType.StopOnWaferLocationChanged);

            // Create the macro-command
            var macroCommand = new RobotMacroCommand(this, (int)EFEMEvents.LoadWaferCompleted, action);
            macroCommand.AddMacroCommand(loadCommand);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Places a substrate to designated position.
        /// </summary>
        /// <param name="location">The substrate destination location.</param>
        /// <param name="slotNumber">The substrate destination slot number.</param>
        /// <param name="arm">Designates an arm which is used for unloading.</param>
        /// <param name="isToReturnArmInOriginPosition">Designate the position of movement completion:
        /// if set to <c>true</c> the arm is retracted to the origin position and stops after substrate is placed;
        /// otherwise the arm stops after substrate is placed.</param>
        public void Place(
            TransferLocation location,
            byte slotNumber,
            RobotArm arm                       = RobotArm.Arm1,
            bool isToReturnArmInOriginPosition = true)
        {
            // Update Robot's Data
            RobotAction action = new RobotAction
            {
                Command               = RobotCommands.Place,
                ArmUnLoad             = arm,
                SourceLocation        = TransferLocation.Robot,
                SourceSlotNumber      = 1,
                DestinationLocation   = location,
                DestinationSlotNumber = slotNumber
            };

            // Retrieve command parameters
            ArmOrBoth_Interpolated commandArm;
            switch (arm)
            {
                case RobotArm.Arm1:
                    commandArm = ArmOrBoth_Interpolated.UpperArm;
                    break;

                case RobotArm.Arm2:
                    commandArm = ArmOrBoth_Interpolated.LowerArm;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(arm), arm, string.Empty);
            }

            var stg = (uint)_stoppingPositionConverter.ToStoppingPosition(location, arm, false);

            // Create the command
            var unloadCommand = UnloadWaferCommand.NewOrder(
                Port,
                commandArm,
                stg,
                slotNumber,
                Sender,
                this,
                isToReturnArmInOriginPosition ? MotionType.NormalMotion : MotionType.StopOnWaferLocationChanged);

            // Create the macro-command
            var macroCommand = new RobotMacroCommand(this, (int)EFEMEvents.UnloadWaferCompleted, action);
            macroCommand.AddMacroCommand(unloadCommand);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        public void SetDateAndTime()
        {
            // Create the command
            var setTimeCmd = SetDateAndTimeCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                Sender,
                this,
                true);

            // Send the command
            _commandsSubscriber.AddMacro(setTimeCmd);
        }

        public void SetMotionSpeed(Ratio percentage)
        {
            // Create the command
            var setSpeedCmd = SetMotionSpeedCommand.NewOrder(
                Port,
                Sender,
                this,
                true,
                true,
                (uint)percentage.Percent);

            // Send the command
            _commandsSubscriber.AddMacro(setSpeedCmd);
        }

        /// <summary>
        /// Gets the robot statuses.
        /// </summary>
        public void GetStatuses()
        {
            // Create commands
            var statCmd = StatusAcquisitionCommand.NewOrder(RorzeConstants.DeviceTypeAbb.Robot, Port, Sender, this);
            var gpioCmd = GpioCommand.NewOrder(RorzeConstants.DeviceTypeAbb.Robot, Port, Sender, this);
            var gwidCmd = GetWaferPresenceAndHistoryCommand.NewOrder(Port, _stoppingPositionConverter, Sender, this);
            var gposCmd = GposCommand.NewOrder(RorzeConstants.DeviceTypeAbb.Robot, Port, Sender, this);

            // Create the Macro Command
            var macroCmd = new BaseMacroCommand(this, (int)EFEMEvents.GetStatusesCompleted);
            macroCmd.AddMacroCommand(statCmd);
            macroCmd.AddMacroCommand(gpioCmd);
            macroCmd.AddMacroCommand(gwidCmd);
            macroCmd.AddMacroCommand(gposCmd);

            // Send the command.
            _commandsSubscriber.AddMacro(macroCmd);
        }

        /// <summary>
        /// Sends robot to home position.
        /// </summary>
        /// <remarks>Send HOME Rorze message</remarks>
        public void GoToHome()
        {
            // Update Robot's Data
            RobotAction action = new RobotAction { Command = RobotCommands.Home };

            // Create the command
            var homeCmd = GoToPosVisitingHomeCommand.NewOrder(Port, Sender, this);

            // Create the macro-command
            var macroCommand = new RobotMacroCommand(this, (int)EFEMEvents.GoToPosVisitingHomeCompleted, action);
            macroCommand.AddMacroCommand(homeCmd);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Commands the robot to move in front of the specified location.
        /// </summary>
        /// <param name="location">The destination location.</param>
        /// <param name="slotNumber">The destination slot number.</param>
        /// <param name="arm">The arm used.</param>
        /// <param name="isPickUpPosition">if set to <c>true</c> robot moves to substrate pick up position; otherwise robot moves to substrate insert position.</param>
        /// <remarks>Send GOTO Rorze message</remarks>
        public void GoToLocation(
            TransferLocation location,
            byte slotNumber       = 1,
            RobotArm arm          = RobotArm.Arm1,
            bool isPickUpPosition = true)
        {
            // Update Robot's Data
            RobotAction action = new RobotAction
            {
                Command               = RobotCommands.GoTo,
                ArmLoad               = arm,
                ArmUnLoad             = arm,
                DestinationLocation   = location,
                DestinationSlotNumber = slotNumber
            };

            // Retrieve command parameters
            var moveId     = isPickUpPosition ? MoveId.WaferBottomSurface : MoveId.PositionWhereWaferIsLifted;
            var commandArm = arm == RobotArm.Arm1 ? Arm_Interpolated.UpperArm : Arm_Interpolated.LowerArm;
            var stg        = (uint)_stoppingPositionConverter.ToStoppingPosition(location, arm, false);

            // Create the command
            var goToCmd = GoToPosVisitingHomeCommand.NewOrder(Port, moveId, commandArm, stg, slotNumber, Sender, this);

            // Create the macro-command
            var macroCommand = new RobotMacroCommand(this, (int)EFEMEvents.GoToPosVisitingHomeCompleted, action);
            macroCommand.AddMacroCommand(goToCmd);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Sets chucking state (switch ON or OFF the vacuum).
        /// </summary>
        /// <param name="arm">The arm to be operated. Value <see cref="RobotArm.Undefined"/> is not allowed. </param>
        /// <param name="state">The vacuum state (ON, OFF).</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="arm"/> value is <see cref="RobotArm.Undefined"/>.</exception>
        public void SetVacuum(RobotArm arm, IOState state)
        {
            if (state != IOState.On && state != IOState.Off)
            {
                throw new ArgumentOutOfRangeException($"State '{state}' invalid.");
            }

            // Create the command
            RorzeCommand setVacuumCmd;
            if (state == IOState.On)
            {
                switch (arm)
                {
                    case RobotArm.Arm1:
                        setVacuumCmd = RetainWaferCommand.NewOrder(
                            Port,
                            RetainingOption.PerformWaferRetainingTestWithUpperArm,
                            Sender,
                            this,
                            true);
                        break;

                    case RobotArm.Arm2:
                        setVacuumCmd = RetainWaferCommand.NewOrder(
                            Port,
                            RetainingOption.PerformWaferRetainingTestWithLowerArm,
                            Sender,
                            this,
                            true);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(arm), arm, null);
                }
            }
            else
            {
                switch (arm)
                {
                    case RobotArm.Arm1:
                        setVacuumCmd = ReleaseWaferRetentionCommand.NewOrder(
                            Port,
                            ReleaseWaferRetentionParameter.ReleaseWaferRetentionOnUpperArm,
                            Sender,
                            this,
                            true);
                        break;

                    case RobotArm.Arm2:
                        setVacuumCmd = ReleaseWaferRetentionCommand.NewOrder(
                            Port,
                            ReleaseWaferRetentionParameter.ReleaseWaferRetentionOnLowerArm,
                            Sender,
                            this,
                            true);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(arm), arm, null);
                }
            }

            // Send the command
            _commandsSubscriber.AddMacro(setVacuumCmd);
        }

        /// <summary>
        /// Sets the device in a safe known state and makes it ready for production.
        /// </summary>
        /// <remarks>Send INIT Rorze message</remarks>
        /// <exception cref="NotImplementedException"></exception>
        public void QuickInit()
        {
            // Create each individual command
            var homeCmd = GoToPosVisitingHomeCommand.NewOrder(Port, Sender, this);

            var testUpperArmRetainingTestCmd = RetainWaferCommand.NewOrder(
                Port,
                RetainingOption.PerformWaferRetainingTestWithUpperArm,
                Sender,
                this,
                false);

            var testLowerArmRetainingTestCmd = RetainWaferCommand.NewOrder(
                Port,
                RetainingOption.PerformWaferRetainingTestWithLowerArm,
                Sender,
                this,
                false);


            // Create the macro-command
            var macroCommand = new BaseMacroCommand(this, (int)EFEMEvents.InitCompleted);

            macroCommand.AddMacroCommand(homeCmd);
            macroCommand.AddMacroCommand(testLowerArmRetainingTestCmd);
            macroCommand.AddMacroCommand(testUpperArmRetainingTestCmd);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Sets the device in a safe known state and makes it ready for production.
        /// </summary>
        /// <remarks>Send INIT Rorze message</remarks>
        /// <exception cref="NotImplementedException"></exception>
        public override void Initialization()
        {
            // Create each individual command
            var resetErrorsCmd = ResetErrorCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                ResetErrorParameter.ResetAndStop,
                false,
                Sender,
                this);

            var initCmd = InitializeStatusCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                Sender,
                this);

            var testUpperArmRetainingTestCmd = RetainWaferCommand.NewOrder(
                Port,
                RetainingOption.PerformWaferRetainingTestWithUpperArm,
                Sender,
                this,
                false);

            var testLowerArmRetainingTestCmd = RetainWaferCommand.NewOrder(
                Port,
                RetainingOption.PerformWaferRetainingTestWithLowerArm,
                Sender,
                this,
                false);

            var originSearchCmd = OriginSearchCommand.NewRobotOrder(
                Port,
                Sender,
                this,
                RobotOriginSearchParameterN.CheckAndRetainWafer);

            // Create the macro-command
            var macroCommand = BuildInitMacroCommand((int)EFEMEvents.InitCompleted);

            macroCommand.AddMacroCommand(resetErrorsCmd);
            macroCommand.AddMacroCommand(initCmd);
            macroCommand.AddMacroCommand(testLowerArmRetainingTestCmd);
            macroCommand.AddMacroCommand(testUpperArmRetainingTestCmd);
            macroCommand.AddMacroCommand(originSearchCmd);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        public void Extend(
            TransferLocation location,
            byte slotNumber,
            RobotArm arm = RobotArm.Arm1)
        {
            // Update Robot's Data
            RobotAction action = new RobotAction
            {
                Command = RobotCommands.Pick,
                ArmLoad = arm,
                SourceLocation = location,
                SourceSlotNumber = slotNumber,
                DestinationLocation = TransferLocation.Robot,
                DestinationSlotNumber = 1
            };

            // Retrieve command parameters
            ArmOrBoth_Interpolated commandArm;
            switch (arm)
            {
                case RobotArm.Arm1:
                    commandArm = ArmOrBoth_Interpolated.UpperArm;
                    break;

                case RobotArm.Arm2:
                    commandArm = ArmOrBoth_Interpolated.LowerArm;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(arm), arm, string.Empty);
            }

            var stg = (uint)_stoppingPositionConverter.ToStoppingPosition(location, arm, false);

            // Create the command
            var extendCommand = ExtendRobotArmCommand.NewOrder(
                Port,
                ExtendId.WaferBottomSurfaceAtUnloadPosition,
                commandArm,
                stg,
                slotNumber,
                Sender,
                this);

            // Create the macro-command
            var macroCommand = new RobotMacroCommand(this, (int)EFEMEvents.ExtendArmCompleted, action);
            macroCommand.AddMacroCommand(extendCommand);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        public void Swap(
            TransferLocation location,
            byte slotNumber,
            RobotArm pickArm = RobotArm.Arm1)
        {
            var placeArm = RobotArm.Arm1;
            if (pickArm == RobotArm.Arm1)
            {
                placeArm = RobotArm.Arm2;
            }

            // Update Robot's Data
            RobotAction action = new RobotAction
            {
                Command = RobotCommands.Swap,
                ArmLoad = pickArm,
                ArmUnLoad = placeArm,
                SourceLocation = location,
                SourceSlotNumber = slotNumber,
                DestinationLocation = location,
                DestinationSlotNumber = slotNumber
            };

            // Retrieve command parameters
            ArmOrBoth_Interpolated commandArm;
            switch (pickArm)
            {
                case RobotArm.Arm1:
                    commandArm = ArmOrBoth_Interpolated.UpperArm;
                    break;

                case RobotArm.Arm2:
                    commandArm = ArmOrBoth_Interpolated.LowerArm;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(pickArm), pickArm, string.Empty);
            }

            var stg = (uint)_stoppingPositionConverter.ToStoppingPosition(location, pickArm, false);

            var swapCommand = ExchangeWaferCommand.NewOrder(
                Port,
                commandArm,
                stg,
                slotNumber,
                ExchangeMotionType.NormalMotion,
                Sender,
                this);

            // Create the macro-command
            var macroCommand = new RobotMacroCommand(this, (int)EFEMEvents.SwapCompleted, action);
            macroCommand.AddMacroCommand(swapCommand);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        public void Transfer(
            TransferLocation sourceLocation,
            byte sourceSlotNumber,
            RobotArm pickArm,
            TransferLocation destinationLocation,
            byte destinationSlotNumber,
            RobotArm placeArm)
        {
            // Update Robot's Data
            RobotAction action = new RobotAction
            {
                Command = RobotCommands.Swap,
                ArmLoad = pickArm,
                ArmUnLoad = placeArm,
                SourceLocation = sourceLocation,
                SourceSlotNumber = sourceSlotNumber,
                DestinationLocation = destinationLocation,
                DestinationSlotNumber = destinationSlotNumber
            };

            // Retrieve command parameters
            ArmOrBoth_Interpolated commandPickArm;
            switch (pickArm)
            {
                case RobotArm.Arm1:
                    commandPickArm = ArmOrBoth_Interpolated.UpperArm;
                    break;

                case RobotArm.Arm2:
                    commandPickArm = ArmOrBoth_Interpolated.LowerArm;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(pickArm), pickArm, string.Empty);
            }

            var sourceStg = (uint)_stoppingPositionConverter.ToStoppingPosition(sourceLocation, pickArm, false);

            // Retrieve command parameters
            ArmOrBoth_Interpolated commandPlaceArm;
            switch (placeArm)
            {
                case RobotArm.Arm1:
                    commandPlaceArm = ArmOrBoth_Interpolated.UpperArm;
                    break;

                case RobotArm.Arm2:
                    commandPlaceArm = ArmOrBoth_Interpolated.LowerArm;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(placeArm), placeArm, string.Empty);
            }

            var destinationStg = (uint)_stoppingPositionConverter.ToStoppingPosition(sourceLocation, placeArm, false);

            var transferCommand = TransferWaferCommand.NewOrder(
                Port,
                commandPickArm,
                sourceStg,
                sourceSlotNumber,
                commandPlaceArm,
                destinationStg,
                destinationSlotNumber,
                Sender,
                this);

            // Create the macro-command
            var macroCommand = new RobotMacroCommand(this, (int)EFEMEvents.TransferCompleted, action);
            macroCommand.AddMacroCommand(transferCommand);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>Perform mapping of the carrier.</summary>
        /// <remarks>Carrier requires to be opened first.</remarks>
        public void Map(TransferLocation location, uint stg)
        {
            // Update Robot's Data
            RobotAction action = new RobotAction
            {
                Command = "Map",
                SourceLocation = location
            };

            // Create command
            var enableMappingSensor = SetMappingSensorCommand.NewOrder(Port, true, Sender, this);
            var performWaferMappingCommand = PerformWaferMappingCommand.NewOrder(Port, stg, Sender, this);
            var disableMappingSensor = SetMappingSensorCommand.NewOrder(Port, false, Sender, this);

            // Create the macro-command
            var macroCommand = new RobotMacroCommand(this, (int)EFEMEvents.PerformWaferMappingCompleted, action);
            macroCommand.AddMacroCommand(enableMappingSensor);
            macroCommand.AddMacroCommand(performWaferMappingCommand);
            macroCommand.AddMacroCommand(disableMappingSensor);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>Gets last mapping results.</summary>
        public void GetLastMapping(uint stg)
        {
            // Create command
            var mappingPatternAcquisitionCommand =
                MappingPatternAcquisitionCommand.NewOrder(Port, stg, Sender, this);

            // Send the command
            _commandsSubscriber.AddMacro(mappingPatternAcquisitionCommand);
        }

        public void GetVersion()
        {
            // Create commands
            var acquisitionCommand = VersionAcquisitionCommand.NewOrder(RorzeConstants.DeviceTypeAbb.Robot, Port, Sender, this);

            // Send the command.
            _commandsSubscriber.AddMacro(acquisitionCommand);
        }

        #endregion Commands to Hardware

        #region Events

        /// <summary>
        /// Occurs when robot movement ended.
        /// </summary>
        public event EventHandler<RobotMovementEventArgs> RobotMoved;

        /// <summary>
        /// Raises the <see cref="E:RobotMoved" /> event.
        /// </summary>
        /// <param name="args">The <see cref="RobotMovementEventArgs" /> to be attached with the event.</param>
        protected virtual void OnRobotMoved(RobotMovementEventArgs args)
        {
            try
            {
                RobotMoved?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        /// Occurs when status received from Robot.
        /// </summary>
        public event EventHandler<StatusEventArgs<RobotStatus>> StatusReceived;

        /// <summary>
        /// Raises the <see cref="StatusReceived"/> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{RobotStatus}"/> to be attached with the event.</param>
        protected virtual void OnStatusReceived(StatusEventArgs<RobotStatus> args)
        {
            try
            {
                StatusReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        /// Occurs when GPIO status received from Robot.
        /// </summary>
        public event EventHandler<StatusEventArgs<RobotGpioStatus>> GpioReceived;

        /// <summary>
        /// Raises the <see cref="GpioReceived"/> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{GpioReceived}"/> to be attached with the event.</param>
        protected virtual void OnGpioReceived(StatusEventArgs<RobotGpioStatus> args)
        {
            try
            {
                GpioReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        /// Occurs when GPOS status received from Robot.
        /// </summary>
        public event EventHandler<StatusEventArgs<RobotGposStatus>> GposReceived;

        /// <summary>
        /// Raises the <see cref="GposReceived"/> event.
        /// </summary>
        /// <param name="args">The <see cref="StatusEventArgs{GposReceived}"/> to be attached with the event.</param>
        protected virtual void OnGposReceived(StatusEventArgs<RobotGposStatus> args)
        {
            try
            {
                GposReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        /// Occurs when substrate presence changed on arm.
        /// </summary>
        public event EventHandler<SubstratePresenceEventArgs> SubstratePresenceChanged;

        /// <summary>
        /// Raises the <see cref="SubstratePresenceChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="SubstratePresenceEventArgs" /> to be attached with the event.</param>
        protected virtual void OnSubstratePresenceReceived(SubstratePresenceAndHistoryEventArgs args)
        {
            try
            {
                SubstratePresenceChanged?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>
        /// Notifies that the mapping of the carrier present on LoadPort is completed.
        /// </summary>
        /// <remarks>
        /// This event may be "command-initiated" (sent at completion of map command), or "self-initiated" (in
        /// case mapping is automatically performed when opening the carrier).
        /// </remarks>
        public event EventHandler<MappingEventArgs> CarrierMapped;

        /// <summary>Sends the <see cref="CarrierMapped" /> event.</summary>
        /// <param name="args">The <see cref="MappingEventArgs" /> to be attached with the event.</param>
        protected virtual void OnCarrierMapped(MappingEventArgs args)
        {
            try
            {
                CarrierMapped?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        /// <summary>Occurs when firmware version received.</summary>
        public event EventHandler<VersionAcquisitionEventArgs> VersionReceived;

        /// <summary>Sends the <see cref="VersionReceived" /> event.</summary>
        /// <param name="args">
        /// The <see cref="VersionAcquisitionEventArgs" /> to be attached with the event.
        /// </param>
        protected virtual void OnVersionReceived(VersionAcquisitionEventArgs args)
        {
            try
            {
                VersionReceived?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
        #endregion Events

        #region Overrides

        /// <summary>
        /// Suspends EFEM operation axes.
        /// If a wafer is being transferred, the operation is stopped with vacuum chucking the wafer on the arm and retracting the arm.
        /// </summary>
        /// <remarks>
        /// This api will not imply a behavior similar to an EMO switch.
        /// Sends HOLD then ABORT messages
        /// </remarks>
        public override void EmergencyStop()
        {
            ClearCommandsQueue();

            var pauseCommand = PauseCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                Sender,
                this);

            var stopCommand = StopCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                Sender,
                this);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(
                this,
                (int)EFEMEvents.StopMotionCommandCompleted);
            macroCommand.AddMacroCommand(pauseCommand);
            macroCommand.AddMacroCommand(stopCommand);

            // Send the command
            _commandsSubscriber.AddMacro(macroCommand);
        }

        /// <summary>
        /// Called when a command is completed by the hardware.
        /// </summary>
        /// <param name="evtId">Identifies the command that ended.</param>
        /// <param name="evtResults">Contains the results of the command. To be cast in the appropriate type if command results are expected.</param>
        protected override void CommandEndedCallback(int evtId, System.EventArgs evtResults)
        {
            if (!Enum.IsDefined(typeof(EFEMEvents), evtId))
            {
                Logger.Warning($"{nameof(CommandEndedCallback)} - Unexpected event ID received: {evtId}");
                return;
            }

            switch ((EFEMEvents)evtId)
            {
                // Command completion
                case EFEMEvents.InitCompleted:
                    OnCommandDone(new CommandEventArgs(RobotCommands.Initialization));
                    break;

                case EFEMEvents.InitializeCommunicationCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(InitializeCommunication)));
                    break;

                case EFEMEvents.GetStatusesCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(GetStatuses)));
                    break;

                case EFEMEvents.GoToPosVisitingHomeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(GoToPosVisitingHomeCommand)));
                    break;

                case EFEMEvents.LoadWaferCompleted:
                    if (evtResults is RobotMovementEventArgs loadEventArgs)
                    {
                        OnRobotMoved(loadEventArgs);
                    }

                    OnCommandDone(new CommandEventArgs(nameof(LoadWaferCommand)));
                    break;

                case EFEMEvents.UnloadWaferCompleted:
                    if (evtResults is RobotMovementEventArgs unloadEventArgs)
                    {
                        OnRobotMoved(unloadEventArgs);
                    }

                    OnCommandDone(new CommandEventArgs(nameof(UnloadWaferCommand)));
                    break;

                case EFEMEvents.ExtendArmCompleted:
                    if (evtResults is RobotMovementEventArgs extendEventArgs)
                    {
                        OnRobotMoved(extendEventArgs);
                    }

                    OnCommandDone(new CommandEventArgs(nameof(ExtendRobotArmCommand)));
                    break;

                case EFEMEvents.SwapCompleted:
                    if (evtResults is RobotMovementEventArgs swapEventArgs)
                    {
                        OnRobotMoved(swapEventArgs);
                    }

                    OnCommandDone(new CommandEventArgs(nameof(ExchangeWaferCommand)));
                    break;

                case EFEMEvents.TransferCompleted:
                    if (evtResults is RobotMovementEventArgs transferEventArgs)
                    {
                        OnRobotMoved(transferEventArgs);
                    }

                    OnCommandDone(new CommandEventArgs(nameof(TransferWaferCommand)));
                    break;

                case EFEMEvents.RetainWaferCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(RetainWaferCommand)));
                    break;

                case EFEMEvents.ReleaseWaferRetentionCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(ReleaseWaferRetentionCommand)));
                    break;

                case EFEMEvents.ResetErrorCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(ResetErrorCommand)));
                    break;

                case EFEMEvents.SetMotionSpeedCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(SetMotionSpeedCommand)));
                    break;

                case EFEMEvents.WaferPresenceAndHistoryCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(GetWaferPresenceAndHistoryCommand)));
                    break;

                case EFEMEvents.SetDateTimeCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(SetDateAndTimeCommand)));
                    break;

                case EFEMEvents.StopMotionCommandCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(StopCommand)));
                    break;

                case EFEMEvents.GetVersionCompleted:
                    if (evtResults is not VersionAcquisitionEventArgs version)
                    {
                        break;
                    }

                    OnVersionReceived(version);
                    OnCommandDone(new CommandEventArgs(nameof(VersionAcquisitionCommand)));
                    break;

                case EFEMEvents.PerformWaferMappingCompleted:
                    OnCommandDone(new CommandEventArgs(nameof(PerformWaferMappingCommand)));
                    break;

                // Event received
                case EFEMEvents.StatusReceived:
                    OnStatusReceived(evtResults as StatusEventArgs<RobotStatus>);
                    break;

                case EFEMEvents.GpioEventReceived:
                    OnGpioReceived(evtResults as StatusEventArgs<RobotGpioStatus>);
                    break;

                case EFEMEvents.GposEventReceived:
                    OnGposReceived(evtResults as StatusEventArgs<RobotGposStatus>);
                    break;

                case EFEMEvents.WaferPresenceAndHistoryReceived:
                    OnSubstratePresenceReceived(evtResults as SubstratePresenceAndHistoryEventArgs);
                    break;

                case EFEMEvents.GetLastMappingCompleted:
                    OnCarrierMapped(evtResults as MappingEventArgs);
                    OnCommandDone(new CommandEventArgs(nameof(MappingPatternAcquisitionCommand)));
                    break;

                // Not managed by this driver
                default:
                    Logger.Warning($"{nameof(CommandEndedCallback)} - Unexpected event ID received: {evtId}");
                    break;
            }
        }

        /// <summary>
        /// Enables Robot listeners.
        /// </summary>
        protected override void EnableListeners()
        {
            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Enabling", Name));

            var stateEvt = StatusAcquisitionCommand.NewEvent(RorzeConstants.DeviceTypeAbb.Robot, Port, Sender, this);
            _stateChangedSubscriber.AddMacro(stateEvt);

            var gpioEvt = GpioCommand.NewEvent(RorzeConstants.DeviceTypeAbb.Robot, Port, Sender, this);
            _gpioReceivedSubscriber.AddMacro(gpioEvt);

            var waferPresenceEvt = GetWaferPresenceAndHistoryCommand.NewEvent(Port, _stoppingPositionConverter, Sender, this);
            _waferPresenceReceivedSubscriber.AddMacro(waferPresenceEvt);

            var gposEvt = GposCommand.NewEvent(RorzeConstants.DeviceTypeAbb.Robot, Port, Sender, this);
            _gposReceivedSubscriber.AddMacro(gposEvt);

            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Enabled", Name));
        }

        /// <summary>
        /// Flush Listeners.
        /// </summary>
        protected override void DisableListeners()
        {
            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Disabling", Name));

            DiscardOpenTransactions(_stateChangedSubscriber);
            DiscardOpenTransactions(_gpioReceivedSubscriber);
            DiscardOpenTransactions(_waferPresenceReceivedSubscriber);
            DiscardOpenTransactions(_gposReceivedSubscriber);

            Logger.Debug(string.Format(CultureInfo.InvariantCulture, "{0} Listeners are Disabled", Name));
        }

        /// <summary>
        /// Flush the queue holding commands to be sent to the device.
        /// </summary>
        /// <remarks>
        /// In case a command is in progress when this method is called, the command's completion will NOT be notified.
        /// </remarks>
        public override void ClearCommandsQueue()
        {
            base.ClearCommandsQueue();
            DiscardOpenTransactions(_commandsSubscriber);
            OnCommandInterrupted();
        }

        protected override void AliveBitRequest()
        {
            GetVersion();
        }
        #endregion Overrides

        #region Helpers

        private BaseMacroCommand BuildInitMacroCommand(int eventToFacade)
        {
            // Create each individual command
            var setTimeCmd = SetDateAndTimeCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                Sender,
                this,
                false);
            var disableEventCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                EventTargetParameter.AllEvents,
                EventEnableParameter.Disable,
                Sender,
                this);
            var enableStatusCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                EventTargetParameter.StatusEvent,
                EventEnableParameter.Enable,
                Sender,
                this);
            var enableGpioCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                EventTargetParameter.PioEvent,
                EventEnableParameter.Enable,
                Sender,
                this);
            var enableGposCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                EventTargetParameter.StoppingPositionEvent,
                EventEnableParameter.Enable,
                Sender,
                this);
            var enableGwidCmd = EventCommand.NewOrder(
                RorzeConstants.DeviceTypeAbb.Robot,
                Port,
                EventTargetParameter.SubstrateIdEvent,
                EventEnableParameter.Enable,
                Sender,
                this);

            // Create the Macro Command
            var macroCommand = new BaseMacroCommand(this, eventToFacade);
            macroCommand.AddMacroCommand(setTimeCmd);
            macroCommand.AddMacroCommand(disableEventCmd);
            macroCommand.AddMacroCommand(enableStatusCmd);
            macroCommand.AddMacroCommand(enableGpioCmd);
            macroCommand.AddMacroCommand(enableGposCmd);
            macroCommand.AddMacroCommand(enableGwidCmd);

            return macroCommand;
        }

        #endregion Helpers

        #region IDisposable

        /// <summary>
        /// Performs the actual cleanup actions on managed/unmanaged resources.
        /// </summary>
        /// <param name="disposing">When <see Langword="true" />, managed resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveReplySubscriber(_commandsSubscriber);
                RemoveReplySubscriber(_stateChangedSubscriber);
                RemoveReplySubscriber(_gpioReceivedSubscriber);
                RemoveReplySubscriber(_waferPresenceReceivedSubscriber);
                RemoveReplySubscriber(_gposReceivedSubscriber);
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
