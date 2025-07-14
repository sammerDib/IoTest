using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Robot.Configuration;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Abstractions.Devices.Robot
{
    public partial class Robot : IExtendedConfigurableDevice
    {
        #region Public Methods

        /// <summary>Adds a new entry to <see cref="RegisteredLocations" />.</summary>
        /// <param name="deviceName">Name of the device (<see cref="ExtendedObject.Name" />).</param>
        /// <param name="position">Enumerate value that should be associated to the device.</param>
        public void RegisterLocation(string deviceName, TransferLocation position)
        {
            RegisteredLocations.Add(deviceName, position);
        }

        public virtual void CheckSubstrateDetectionError(RobotArm arm, bool reset = false)
        {
            switch (arm)
            {
                case RobotArm.Arm1:
                    if ((UpperArmWaferPresence == WaferPresence.Present
                         && UpperArmLocation.Wafer != null)
                        || (UpperArmWaferPresence == WaferPresence.Absent
                            && UpperArmLocation.Wafer == null)
                        || reset)
                    {
                        UpperArmSubstrateDetectionError = false;
                    }
                    else
                    {
                        UpperArmSubstrateDetectionError = true;
                    }

                    break;
                case RobotArm.Arm2:
                    if ((LowerArmWaferPresence == WaferPresence.Present
                         && LowerArmLocation.Wafer != null)
                        || (LowerArmWaferPresence == WaferPresence.Absent
                            && LowerArmLocation.Wafer == null)
                        || reset)
                    {
                        LowerArmSubstrateDetectionError = false;
                    }
                    else
                    {
                        LowerArmSubstrateDetectionError = true;
                    }

                    break;
            }
        }

        #endregion Public Methods

        #region Properties

        public OneToManyComposition<MaterialLocation> MaterialLocations { get; protected set; }
        public WaferLocation UpperArmLocation { get; protected set; }
        public WaferLocation LowerArmLocation { get; protected set; }
        public ArmHistoryItem LowerArmHistory { get; } = new();
        public ArmHistoryItem UpperArmHistory { get; } = new();
        public RobotArm LastArmBeingMoved { get; set; }
        public CommandContext CurrentCommandContext { get; private set; }

        /// <summary>
        /// Mapping table between device's name and <see cref="TransferLocation" /> enumerate.
        /// </summary>
        /// <remarks>
        /// This is used by robot commands, to know where to move the robot. Most command takes a
        /// <see cref="IMaterialLocationContainer" /> which only gives a name. We use this table to map this
        /// name to a strongly-typed enumerate representing the positions where robot can move.
        /// </remarks>
        public Dictionary<string, TransferLocation> RegisteredLocations { get; protected set; } =
            new();

        #endregion Properties

        #region Command Confirmation

        /// <summary>
        /// Occurs when a movement command is received and external confirmation is required.
        /// </summary>
        public event EventHandler<CommandConfirmationRequestedEventArgs>
            CommandConfirmationRequested;

        protected void OnCommandConfirmationRequested(CommandConfirmationRequestedEventArgs args)
        {
            try
            {
                Logger.Debug(
                    FormattableString.Invariant(
                        $"{nameof(CommandConfirmationRequested)} event sent for Guid {args.Uuid}."));
                CommandConfirmationRequested?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    FormattableString.Invariant(
                        $"Exception occurred when sending event {nameof(CommandConfirmationRequested)}."),
                    ex);
            }
        }

        /// <summary>
        /// Completes a <see cref="CommandConfirmationRequested" /> by allowing the command to run.
        /// </summary>
        public virtual void CommandGranted(Guid commandUuid)
        {
        }

        /// <summary>
        /// Completes a <see cref="CommandConfirmationRequested" /> by aborting the command execution.
        /// </summary>
        public virtual void CommandDenied(Guid commandUuid)
        {
        }

        #endregion Command Confirmation

        #region Robot Commands

        protected abstract void InternalClamp(RobotArm arm);

        protected abstract void InternalUnclamp(RobotArm arm);

        protected abstract void InternalGoToHome();

        protected abstract void InternalGoToLocation(IMaterialLocationContainer destinationDevice);

        protected abstract void InternalGoToTransferLocation(
            TransferLocation location,
            RobotArm arm,
            byte slot);

        protected abstract void InternalGoToSpecifiedLocation(
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot,
            RobotArm arm,
            bool isPickUpPosition);

        protected abstract void InternalPick(
            RobotArm arm,
            IMaterialLocationContainer sourceDevice,
            byte sourceSlot);

        protected abstract void InternalPlace(
            RobotArm arm,
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot);

        protected abstract void InternalTransfer(
            RobotArm pickArm,
            IMaterialLocationContainer sourceDevice,
            byte sourceSlot,
            RobotArm placeArm,
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot);

        protected abstract void InternalSwap(
            RobotArm pickArm,
            IMaterialLocationContainer sourceDevice,
            byte sourceSlot);

        protected abstract void InternalExtendArm(
            RobotArm arm,
            TransferLocation location,
            byte slot);

        protected abstract void InternalSetMotionSpeed(Ratio percentage);

        protected abstract void InternalSetDateAndTime();

        #endregion Robot Commands

        #region Setup

        private void InstanceInitialization()
        {
            UpperArmLocation = new WaferLocation($"{Name} upper arm substrate location");
            LowerArmLocation = new WaferLocation($"{Name} lower arm substrate location");
            MaterialLocations =
                ReferenceFactory.OneToManyComposition<MaterialLocation>(
                    nameof(MaterialLocations),
                    this);
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    LoadConfiguration();
                    break;
                case SetupPhase.SettingUp:
                    MaterialLocations.Add(UpperArmLocation);
                    MaterialLocations.Add(LowerArmLocation);
                    CommandExecutionStateChanged += Robot_CommandExecutionStateChanged;
                    UpperArmLocation.PropertyChanged += UpperArmLocation_PropertyChanged;
                    LowerArmLocation.PropertyChanged += LowerArmLocation_PropertyChanged;
                    DeviceType.AllCommands().First(x => x.Name == nameof(Initialize)).Timeout =
                        Duration.FromSeconds(Configuration.InitializationTimeout);
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region IConfigurableDevice

        public IConfigManager ConfigManager { get; protected set; }

        /// <summary>
        /// Gets the device current configuration (<see cref="IConfigManager.Current" />).
        /// </summary>
        public RobotConfiguration Configuration => ConfigManager.Current.Cast<RobotConfiguration>();

        /// <inheritdoc />
        public abstract string RelativeConfigurationDir { get; }

        /// <inheritdoc />
        public abstract void LoadConfiguration(string deviceConfigRootPath = "");

        /// <inheritdoc />
        public void SetExecutionMode(ExecutionMode executionMode)
        {
            ExecutionMode = executionMode;
        }

        #endregion IConfigurableDevice

        #region Event Handlers

        private void Robot_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            CurrentCommandContext = e.Execution.Context;
        }

        #endregion Event Handlers

        #region Private

        private void LowerArmLocation_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LowerArmLocation.Material))
            {
                if (LowerArmLocation.Substrate != null)
                {
                    LowerArmWaferDimension = LowerArmLocation.Substrate.MaterialDimension;
                    LowerArmSimplifiedWaferId = LowerArmLocation.Substrate.SimplifiedName;
                    LowerArmWaferStatus = LowerArmLocation.Substrate.Status;
                    LowerArmLocation.Substrate.PropertyChanged += LowerArmSubstrate_PropertyChanged;
                }
                else
                {
                    LowerArmWaferDimension = SampleDimension.NoDimension;
                    LowerArmSimplifiedWaferId = string.Empty;
                    LowerArmWaferStatus = WaferStatus.None;
                }
            }
        }

        private void UpperArmLocation_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UpperArmLocation.Material))
            {
                if (UpperArmLocation.Substrate != null)
                {
                    UpperArmWaferDimension = UpperArmLocation.Substrate.MaterialDimension;
                    UpperArmSimplifiedWaferId = UpperArmLocation.Substrate.SimplifiedName;
                    UpperArmWaferStatus = UpperArmLocation.Substrate.Status;
                    UpperArmLocation.Substrate.PropertyChanged += UpperArmSubstrate_PropertyChanged;
                }
                else
                {
                    UpperArmWaferDimension = SampleDimension.NoDimension;
                    UpperArmSimplifiedWaferId = string.Empty;
                    UpperArmWaferStatus = WaferStatus.None;
                }
            }
        }

        private void LowerArmSubstrate_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Substrate.Status))
            {
                LowerArmWaferStatus = LowerArmLocation.Substrate?.Status ?? WaferStatus.None;
            }
        }

        private void UpperArmSubstrate_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Substrate.Status))
            {
                UpperArmWaferStatus = UpperArmLocation.Substrate?.Status ?? WaferStatus.None;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CommandExecutionStateChanged -= Robot_CommandExecutionStateChanged;
                UpperArmLocation.PropertyChanged -= UpperArmLocation_PropertyChanged;
                LowerArmLocation.PropertyChanged -= LowerArmLocation_PropertyChanged;
                if (UpperArmLocation.Substrate != null)
                {
                    UpperArmLocation.Substrate.PropertyChanged -= UpperArmSubstrate_PropertyChanged;
                }

                if (LowerArmLocation.Substrate != null)
                {
                    LowerArmLocation.Substrate.PropertyChanged -= LowerArmSubstrate_PropertyChanged;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
