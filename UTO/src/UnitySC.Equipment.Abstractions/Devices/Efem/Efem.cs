using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Agileo.AlarmModeling;
using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Efem.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.Robot.Enums;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Enums;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.Equipment.Abstractions.Vendor.Material;

using Path = System.IO.Path;

namespace UnitySC.Equipment.Abstractions.Devices.Efem
{
    public partial class Efem : IConfigurableDevice<EfemConfiguration>
    {
        #region Fields

        private Aligner.Aligner _aligner;
        private Robot.Robot _robot;
        private List<ProcessModule.ProcessModule> _processModules;
        private PersistentLocationInformations _persistentLocationInformations;
        private string _persistentLocationFilePath;

        #endregion Fields

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    foreach (var device in this.AllDevices())
                    {
                        device.StatusValueChanged += Device_StatusValueChanged;
                    }

                    _aligner = this.TryGetDevice<Aligner.Aligner>();
                    if (_aligner != null)
                    {
                        _aligner.Location.PropertyChanged += AlignerLocation_PropertyChanged;
                        _aligner.CommandExecutionStateChanged +=
                            Aligner_CommandExecutionStateChanged;
                    }

                    _robot = this.TryGetDevice<Robot.Robot>();
                    if (_robot != null)
                    {
                        _robot.CommandExecutionStateChanged += Robot_CommandExecutionStateChanged;
                        _robot.UpperArmLocation.PropertyChanged += UpperArmLocation_PropertyChanged;
                        _robot.LowerArmLocation.PropertyChanged += LowerArmLocation_PropertyChanged;
                    }

                    _processModules = this.GetTopDeviceContainer()
                        .AllDevices<ProcessModule.ProcessModule>()
                        .ToList();

                    // Register process modules
                    foreach (var processModule in _processModules)
                    {
                        processModule.Location.PropertyChanged += ProcessModule_PropertyChanged;
                    }

                    RegisterRobotLocations();
                    _persistentLocationInformations =
                        PersistentLocationInformations.Deserialize(_persistentLocationFilePath);
                    if (_persistentLocationInformations.LocationInformations == null
                        && _aligner != null
                        && _robot != null)
                    {
                        _persistentLocationInformations.LocationInformations =
                            new Dictionary<string, SerializableSubstrate>
                            {
                                { _aligner.Location.Name, null },
                                { _robot.UpperArmLocation.Name, null },
                                { _robot.LowerArmLocation.Name, null }
                            };
                        foreach (var location in _processModules.Select(pm => pm.Location))
                        {
                            _persistentLocationInformations.LocationInformations.Add(
                                location.Name,
                                null);
                        }
                    }

                    //Set more time for initialize because it will take longer time than other devices to initialize
                    var totalTimeoutDuration = 0.0;
                    foreach (var device in this.AllDevices())
                    {
                        if (device.DeviceType.AllCommands()
                                .FirstOrDefault(x => x.Name == nameof(IGenericDevice.Initialize)) is
                            {
                            } deviceCommand)
                        {
                            totalTimeoutDuration += deviceCommand.Timeout.Seconds;
                        }
                    }

                    DeviceType.AllCommands().First(x => x.Name == nameof(Initialize)).Timeout =
                        Duration.FromSeconds(totalTimeoutDuration);
                    ConfigManager.CurrentChanged += ConfigManager_CurrentChanged;
                    break;
                case SetupPhase.SetupDone:
                    SetSubstrateInformations(
                        _aligner.Location,
                        _persistentLocationInformations.LocationInformations
                            [_aligner.Location.Name]);
                    _aligner.CheckSubstrateDetectionError();
                    SetSubstrateInformations(
                        _robot.UpperArmLocation,
                        _persistentLocationInformations.LocationInformations[_robot.UpperArmLocation
                            .Name]);
                    _robot.CheckSubstrateDetectionError(RobotArm.Arm1);
                    SetSubstrateInformations(
                        _robot.LowerArmLocation,
                        _persistentLocationInformations.LocationInformations[_robot.LowerArmLocation
                            .Name]);
                    _robot.CheckSubstrateDetectionError(RobotArm.Arm2);
                    foreach (var processModule in _processModules)
                    {
                        SetSubstrateInformations(
                            processModule.Location,
                            _persistentLocationInformations.LocationInformations[processModule
                                .Location.Name]);
                        processModule.CheckSubstrateDetectionError();
                    }

                    RefreshState();
                    RefreshLightCurtain();
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
        public EfemConfiguration Configuration => ConfigManager.Current.Cast<EfemConfiguration>();

        /// <inheritdoc />
        public virtual string RelativeConfigurationDir => $"./Devices/{nameof(Efem)}/Resources";

        /// <inheritdoc />
        public virtual void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        /// <inheritdoc />
        public void SetExecutionMode(ExecutionMode executionMode)
        {
            ExecutionMode = executionMode;
        }

        public EfemConfiguration CreateDefaultConfiguration()
        {
            return new EfemConfiguration();
        }

        #endregion IConfigurableDevice

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            foreach (var device in this.AllOfType<IUnityCommunicatingDevice>())
            {
                if (!device.IsCommunicationStarted)
                {
                    device.StartCommunication();
                }
            }
        }

        protected override void InternalStopCommunication()
        {
            foreach (var device in this.AllOfType<IUnityCommunicatingDevice>())
            {
                if (device.IsCommunicationStarted)
                {
                    device.StopCommunication();
                }
            }
        }

        #endregion ICommunicatingDevice Commands

        #region Event Handlers

        protected virtual void Device_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            switch (sender)
            {
                case LoadPort.LoadPort lp:
                    switch (lp.InstanceId)
                    {
                        case 1:
                            LoadPortStatus1 = GetStatus(lp);
                            IsLoadPort1CarrierPresent =
                                lp.CarrierPresence == CassettePresence.Correctly;
                            break;
                        case 2:
                            LoadPortStatus2 = GetStatus(lp);
                            IsLoadPort2CarrierPresent =
                                lp.CarrierPresence == CassettePresence.Correctly;
                            break;
                        case 3:
                            LoadPortStatus3 = GetStatus(lp);
                            IsLoadPort3CarrierPresent =
                                lp.CarrierPresence == CassettePresence.Correctly;
                            break;
                        default:
                            LoadPortStatus4 = GetStatus(lp);
                            IsLoadPort4CarrierPresent =
                                lp.CarrierPresence == CassettePresence.Correctly;
                            break;
                    }

                    break;
                case Robot.Robot rob:
                    switch (e.Status.Name)
                    {
                        case nameof(IRobot.LowerArmState):
                        case nameof(IRobot.UpperArmState):
                            RefreshArmExtendedInterlocks(rob);
                            break;
                        case nameof(IRobot.UpperArmWaferPresence):
                            if (State != OperatingModes.Initialization)
                            {
                                MoveSubstrate(rob, RobotArm.Arm1);
                            }

                            break;
                        case nameof(IRobot.LowerArmWaferPresence):
                            if (State != OperatingModes.Initialization)
                            {
                                MoveSubstrate(rob, RobotArm.Arm2);
                            }

                            break;
                    }

                    RobotStatus = GetStatus(rob);
                    RobotSpeed = rob.Speed;
                    break;
                case Aligner.Aligner aln:
                    switch (e.Status.Name)
                    {
                        case nameof(IAligner.WaferPresence):
                            if (State != OperatingModes.Initialization)
                            {
                                MoveSubstrate(aln);
                            }

                            break;
                    }

                    AlignerStatus = GetStatus(aln);
                    IsAlignerCarrierPresent = aln.Location.Material != null;
                    break;
            }

            //Load port must not be included in EFEM state
            if ((sender is not LoadPort.LoadPort loadPort || loadPort.IsInService)
                && e.Status.Name == nameof(IGenericDevice.State))
            {
                RefreshState();
            }
        }

        private void AlignerLocation_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((sender as MaterialLocation)?.Container is Aligner.Aligner aligner)
            {
                CheckAlignerWaferLost(aligner);
                UpdatePersistentLocationInformations(
                    aligner.Location.Name,
                    aligner.Location.Substrate);
            }
        }

        private void UpperArmLocation_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((sender as MaterialLocation)?.Container is Robot.Robot robot)
            {
                CheckRobotWaferLost(robot, robot.UpperArmLocation.Material != null, RobotArm.Arm1);
                UpdatePersistentLocationInformations(
                    robot.UpperArmLocation.Name,
                    robot.UpperArmLocation.Substrate);
            }
        }

        private void LowerArmLocation_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((sender as MaterialLocation)?.Container is Robot.Robot robot)
            {
                CheckRobotWaferLost(robot, robot.LowerArmLocation.Material != null, RobotArm.Arm2);
                UpdatePersistentLocationInformations(
                    robot.LowerArmLocation.Name,
                    robot.LowerArmLocation.Substrate);
            }
        }

        private void ProcessModule_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((sender as MaterialLocation)
                ?.Container is ProcessModule.ProcessModule processModule)
            {
                CheckProcessModuleWaferLost(processModule);
                UpdatePersistentLocationInformations(
                    processModule.Location.Name,
                    processModule.Location.Substrate);
            }
        }

        private void ConfigManager_CurrentChanged(object sender, ConfigurationChangedEventArgs e)
        {
            RefreshLightCurtain();
        }

        private void Robot_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            if (sender is not Robot.Robot robot)
            {
                return;
            }

            if (e.Execution.Context.Command.Name == nameof(IRobot.Initialize)
                && e.NewState is ExecutionState.Success or ExecutionState.Failed)
            {
                RefreshArmExtendedInterlocks(robot);
                robot.CheckSubstrateDetectionError(RobotArm.Arm1);
                robot.CheckSubstrateDetectionError(RobotArm.Arm2);
            }
        }

        private void Aligner_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (sender is not Aligner.Aligner aligner)
            {
                return;
            }

            if (e.Execution.Context.Command.Name == nameof(IAligner.Initialize)
                && e.NewState is ExecutionState.Success or ExecutionState.Failed)
            {
                aligner.CheckSubstrateDetectionError();
            }
        }

        #endregion Event Handlers

        #region Private Methods

        private void MoveSubstrate(Aligner.Aligner aligner)
        {
            var robot = this.TryGetDevice<Robot.Robot>();
            if (robot == null)
            {
                return;
            }

            if (aligner.SubstrateDetectionError)
            {
                aligner.CheckSubstrateDetectionError();
            }

            if (aligner.WaferPresence == WaferPresence.Present && aligner.Location.Material == null)
            {
                if (robot.State == OperatingModes.Executing
                    && robot.CurrentCommandContext != null
                    && ((robot.CurrentCommandContext.Command.Name == nameof(IRobot.Place)
                         && robot.CurrentCommandContext.GetArgument("destinationDevice") is
                             IMaterialLocationContainer placeDevice
                         && placeDevice.Name == aligner.Name)
                        || (robot.CurrentCommandContext.Command.Name == nameof(IRobot.Swap)
                            && robot.CurrentCommandContext.GetArgument("sourceDevice") is
                                IMaterialLocationContainer swapDevice
                            && swapDevice.Name == aligner.Name)
                        || (robot.CurrentCommandContext.Command.Name == nameof(IRobot.Transfer)
                            && robot.CurrentCommandContext.GetArgument("destinationDevice") is
                                IMaterialLocationContainer transferDevice
                            && transferDevice.Name == aligner.Name)))
                {
                    return;
                }

                aligner.CheckSubstrateDetectionError();
            }

            if (aligner.WaferPresence == WaferPresence.Absent && aligner.Location.Material != null)
            {
                if (robot.State == OperatingModes.Executing
                    && robot.CurrentCommandContext != null
                    && ((robot.CurrentCommandContext.Command.Name == nameof(IRobot.Pick)
                         && robot.CurrentCommandContext.GetArgument("sourceDevice") is
                             IMaterialLocationContainer sourceDevice
                         && sourceDevice.Name == aligner.Name)
                        || (robot.CurrentCommandContext.Command.Name == nameof(IRobot.Swap)
                            && robot.CurrentCommandContext.GetArgument("sourceDevice") is
                                IMaterialLocationContainer swapDevice
                            && swapDevice.Name == aligner.Name)
                        || (robot.CurrentCommandContext.Command.Name == nameof(IRobot.Transfer)
                            && robot.CurrentCommandContext.GetArgument("sourceDevice") is
                                IMaterialLocationContainer transferDevice
                            && transferDevice.Name == aligner.Name)))
                {
                    return;
                }

                aligner.CheckSubstrateDetectionError();
            }
        }

        private void MoveSubstrate(Robot.Robot robot, RobotArm arm)
        {
            var armLocation = arm == RobotArm.Arm1
                ? robot.UpperArmLocation
                : robot.LowerArmLocation;
            if (armLocation == null)
            {
                return;
            }

            var isWaferDetectedOnArm = arm == RobotArm.Arm1
                ? robot.UpperArmWaferPresence == WaferPresence.Present
                : robot.LowerArmWaferPresence == WaferPresence.Present;
            if (isWaferDetectedOnArm)
            {
                Agileo.EquipmentModeling.Material material = null;

                // Try to get the material from the source
                if (robot.State == OperatingModes.Executing && robot.CurrentCommandContext != null)
                {
                    switch (robot.CurrentCommandContext.Command.Name)
                    {
                        case nameof(IRobot.Pick):
                            {
                                if (robot.CurrentCommandContext.GetArgument("sourceDevice") is
                                        IMaterialLocationContainer device
                                    && robot.CurrentCommandContext.GetArgument("sourceSlot") is byte
                                        slot
                                    && robot.CurrentCommandContext.GetArgument("arm") is RobotArm
                                        currentArm)
                                {
                                    if (currentArm != arm)
                                    {
                                        //If presence changed on another arm no need to update
                                        return;
                                    }

                                    material = GetMaterial(device, slot);
                                }
                            }

                            break;
                        case nameof(IRobot.Swap):
                        case nameof(IRobot.Transfer):
                            {
                                if (robot.CurrentCommandContext.GetArgument("sourceDevice") is
                                        IMaterialLocationContainer device
                                    && robot.CurrentCommandContext.GetArgument("sourceSlot") is byte
                                        slot
                                    && robot.CurrentCommandContext.GetArgument("pickArm") is
                                        RobotArm currentArm)
                                {
                                    if (currentArm != arm)
                                    {
                                        //If presence changed on another arm no need to update
                                        return;
                                    }

                                    material = GetMaterial(device, slot);
                                }
                            }

                            break;
                    }
                }

                if (material == null)
                {
                    robot.CheckSubstrateDetectionError(arm);
                    return;
                }

                material.SetLocation(armLocation);
            }
            else
            {
                MaterialLocation location = null;

                // Try to get the destination location
                if (robot.State == OperatingModes.Executing && robot.CurrentCommandContext != null)
                {
                    switch (robot.CurrentCommandContext.Command.Name)
                    {
                        case nameof(IRobot.Place):
                            {
                                if (robot.CurrentCommandContext.GetArgument("destinationDevice") is
                                        IMaterialLocationContainer device
                                    && robot.CurrentCommandContext.GetArgument("destinationSlot") is
                                        byte slot
                                    && robot.CurrentCommandContext.GetArgument("arm") is RobotArm
                                        currentArm)
                                {
                                    if (currentArm != arm)
                                    {
                                        //If presence changed on another arm no need to update
                                        return;
                                    }

                                    location = GetLocation(device, slot);
                                }
                            }

                            break;
                        case nameof(IRobot.Swap):
                            {
                                if (robot.CurrentCommandContext.GetArgument("sourceDevice") is
                                        IMaterialLocationContainer device
                                    && robot.CurrentCommandContext.GetArgument("sourceSlot") is byte
                                        slot
                                    && robot.CurrentCommandContext.GetArgument("pickArm") is
                                        RobotArm currentArm)
                                {
                                    if (currentArm == arm)
                                    {
                                        //If presence changed on same arm no need to update
                                        return;
                                    }

                                    location = GetLocation(device, slot);
                                }
                            }

                            break;
                        case nameof(IRobot.Transfer):
                            {
                                if (robot.CurrentCommandContext.GetArgument("destinationDevice") is
                                        IMaterialLocationContainer device
                                    && robot.CurrentCommandContext.GetArgument("destinationSlot") is
                                        byte slot
                                    && robot.CurrentCommandContext.GetArgument("placeArm") is
                                        RobotArm currentArm)
                                {
                                    if (currentArm != arm)
                                    {
                                        //If presence changed on another arm no need to update
                                        return;
                                    }

                                    location = GetLocation(device, slot);
                                }
                            }

                            break;
                    }
                }

                if (location == null)
                {
                    robot.CheckSubstrateDetectionError(arm);
                    return;
                }

                armLocation.Material?.SetLocation(location);
            }
        }

        protected virtual Agileo.EquipmentModeling.Material GetMaterial(
            IMaterialLocationContainer device,
            byte slot)
        {
            Agileo.EquipmentModeling.Material material = null;
            switch (device)
            {
                case LoadPort.LoadPort loadPort:
                    if (0 < slot && slot <= loadPort.Carrier?.MaterialLocations.Count)
                    {
                        material = loadPort.Carrier?.MaterialLocations[slot - 1].Material;
                    }

                    break;
                default:
                    if (0 < slot && slot <= device.MaterialLocations.Count)
                    {
                        material = device.MaterialLocations[slot - 1].Material;
                    }

                    break;
            }

            return material;
        }

        protected virtual MaterialLocation GetLocation(IMaterialLocationContainer device, byte slot)
        {
            MaterialLocation location = null;
            switch (device)
            {
                case LoadPort.LoadPort loadPort:
                    if (0 < slot && slot <= loadPort.Carrier?.MaterialLocations.Count)
                    {
                        location = loadPort.Carrier?.MaterialLocations[slot - 1];
                    }

                    break;
                default:
                    if (0 < slot && slot <= device.MaterialLocations.Count)
                    {
                        location = device.MaterialLocations[slot - 1];
                    }

                    break;
            }

            return location;
        }

        protected abstract void RefreshLightCurtain();

        public abstract (DevicePosition, int) GetPosition(Device device);

        public abstract bool CanReleaseRobot();

        public virtual string ReadSubstrateId(
            ReaderSide readerSide,
            string frontRecipeName,
            string backRecipeName)
        {
            var aligner = this.TryGetDevice<Aligner.Aligner>();
            var frontSideReader = aligner.TryGetDevice<SubstrateIdReader.SubstrateIdReader>(1);
            var backSideReader = aligner.TryGetDevice<SubstrateIdReader.SubstrateIdReader>(2);

            if (readerSide is ReaderSide.Front or ReaderSide.Both)
            {
                try
                {
                    frontSideReader.Read(frontRecipeName);
                }
                catch
                {
                    //Error on front reader reading
                    return string.Empty;
                }
            }

            if (readerSide is ReaderSide.Back or ReaderSide.Both)
            {
                try
                {
                    backSideReader.Read(backRecipeName);
                }
                catch
                {
                    //Error on back reader reading
                    return string.Empty;
                }
            }

            switch (readerSide)
            {
                case ReaderSide.Front:
                    return frontSideReader?.SubstrateId;

                case ReaderSide.Back:
                    return backSideReader?.SubstrateId;

                case ReaderSide.Both:
                    if (frontSideReader?.SubstrateId != backSideReader?.SubstrateId)
                    {
                        return string.Empty;
                    }
                    return frontSideReader?.SubstrateId;

                default:
                    return string.Empty;
            }
        }

        private static LoadPortStatus GetStatus(LoadPort.LoadPort loadPort)
        {
            if (!loadPort.IsInService)
            {
                return LoadPortStatus.Disable;
            }

            switch (loadPort.State)
            {
                case OperatingModes.Initialization:
                    return LoadPortStatus.Unknown;
                case OperatingModes.Maintenance:
                    return LoadPortStatus.Error;
                case OperatingModes.Idle when loadPort.PhysicalState == LoadPortState.Open:
                    return LoadPortStatus.Loaded;
                case OperatingModes.Idle when loadPort.PhysicalState == LoadPortState.Closed:
                    return LoadPortStatus.DoorClose;
                case OperatingModes.Idle:
                    return LoadPortStatus.Idle;
                case OperatingModes.Executing:
                    return LoadPortStatus.Moving;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(loadPort),
                        @$"LoadPort has unexpected state {loadPort.State}");
            }
        }

        private static RobotStatus GetStatus(Robot.Robot robot)
        {
            switch (robot.State)
            {
                case OperatingModes.Initialization:
                    return RobotStatus.Unknown;
                case OperatingModes.Maintenance:
                    return RobotStatus.Error;
                case OperatingModes.Idle:
                    return RobotStatus.Idle;
                case OperatingModes.Executing:
                    return RobotStatus.Moving;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(robot),
                        @$"Robot has unexpected state {robot.State}");
            }
        }

        private static AlignerStatus GetStatus(Aligner.Aligner aligner)
        {
            switch (aligner.State)
            {
                case OperatingModes.Initialization:
                    return AlignerStatus.Unknown;
                case OperatingModes.Maintenance:
                    return AlignerStatus.Error;
                case OperatingModes.Idle:
                    return AlignerStatus.Idle;
                case OperatingModes.Executing:
                    return AlignerStatus.Moving;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(aligner),
                        @$"Aligner has unexpected state {aligner.State}");
            }
        }

        private void RegisterRobotLocations()
        {
            var robot = this.AllDevices<Robot.Robot>().FirstOrDefault();
            if (robot == null)
            {
                return;
            }

            // Register load ports
            foreach (var loadPort in this.AllDevices<LoadPort.LoadPort>())
            {
                if (loadPort.InstanceId is >= 1 and <= 9)
                {
                    robot.RegisterLocation(
                        loadPort.Name,
                        TransferLocation.LoadPort1 + loadPort.InstanceId - 1);
                }
            }

            // Register aligner
            var aligner = this.AllDevices<Aligner.Aligner>().FirstOrDefault();
            if (aligner != null)
            {
                robot.RegisterLocation(aligner.Name, TransferLocation.PreAlignerA);
            }

            // Register process modules
            foreach (var processModule in this.GetTopDeviceContainer()
                         .AllDevices<ProcessModule.ProcessModule>())
            {
                if (processModule.InstanceId is >= 1 and <= 4)
                {
                    robot.RegisterLocation(
                        processModule.Name,
                        TransferLocation.ProcessModuleA + processModule.InstanceId - 1);
                }
            }
        }

        private void CheckAlignerWaferLost(Aligner.Aligner aligner)
        {
            // Step 1: Check wafer presence
            if (aligner.Location.Material != null)
            {
                return;
            }

            // Step 2: Check if robot has taken the wafer
            if (_robot == null || _robot.CurrentCommandContext == null)
            {
                // can not test the command context
                return;
            }

            if (_robot.State == OperatingModes.Executing
                && _robot.CurrentCommandContext.Command.Name == nameof(IRobot.Pick)
                && _robot.CurrentCommandContext.GetArgument("sourceDevice") == aligner)
            {
                // normal use case
                return;
            }

            // Step 3: Log error
            var userMessage =
                $"The wafer presence on {aligner.Name} has changed to false in unusual circumstances";
            OnUserErrorRaised(userMessage);
            Logger.Error(userMessage);
        }

        private void CheckRobotWaferLost(
            Robot.Robot robot,
            bool isWaferPresent,
            RobotArm currentArm)
        {
            // Step 1: Check wafer presence
            if (isWaferPresent)
            {
                return;
            }

            // Step 2: Check robot activity (idle / running) + current command == place / swap / transfer
            if (robot.State == OperatingModes.Executing
                && robot.CurrentCommandContext != null
                && robot.CurrentCommandContext.Command.Name == nameof(IRobot.Place))
            {
                var commandName = robot.CurrentCommandContext.Command.Name;
                var arm = RobotArm.Undefined;

                // Verify that robot arm in the command matches to the status arm
                switch (commandName)
                {
                    case nameof(IRobot.Place):
                        arm = (RobotArm)robot.CurrentCommandContext.GetArgument("arm");
                        break;
                }

                if (arm == currentArm)
                {
                    // normal use case
                    return;
                }
            }

            //Step 3: Log error
            var userMessage =
                $"The wafer presence on {currentArm} has changed to false in unusual circumstances";
            OnUserErrorRaised(userMessage);
            Logger.Error(userMessage);
        }

        private void CheckProcessModuleWaferLost(ProcessModule.ProcessModule processModule)
        {
            // Step 1: Check wafer presence
            if (processModule.Location.Material != null)
            {
                return;
            }

            // Step 2: Check if robot has taken the wafer
            if (_robot == null || _robot.CurrentCommandContext == null)
            {
                // can not test the command context
                return;
            }

            if (_robot.State == OperatingModes.Executing
                && _robot.CurrentCommandContext.Command.Name == nameof(IRobot.Pick)
                && _robot.CurrentCommandContext.GetArgument("sourceDevice") == processModule)
            {
                // normal use case
                return;
            }

            // Step 3: Log error
            var userMessage =
                $"The wafer presence on {processModule.Name} has changed to false in unusual circumstances";
            OnUserErrorRaised(userMessage);
            Logger.Error(userMessage);
        }

        public void RefreshArmExtendedInterlocks(Robot.Robot robot)
        {
            if (robot == null)
            {
                return;
            }

            var isExtended = robot.UpperArmState == ArmState.Extended || robot.LowerArmState == ArmState.Extended;
            TransferLocation? position = null;

            if (robot.Position != TransferLocation.Robot)
            {
                position = robot.Position;
            }
           
            SetArmExtendedInterlock(position, isExtended);
        }

        private void UpdatePersistentLocationInformations(string locationName, Substrate substrate)
        {
            if (substrate == null)
            {
                _persistentLocationInformations.LocationInformations[locationName] = null;
            }
            else
            {
                _persistentLocationInformations.LocationInformations[locationName] =
                    new SerializableSubstrate
                    {
                        SourcePort = substrate.SourcePort,
                        SourceSlot = substrate.SourceSlot,
                        MaterialDimension = substrate.MaterialDimension,
                        MaterialType = ((Wafer)substrate).MaterialType
                    };
            }

            var result = PersistentLocationInformations.Serialize(
                _persistentLocationInformations,
                _persistentLocationFilePath);
            if (!string.IsNullOrEmpty(result))
            {
                Logger.Error(result);
            }
        }

        private void SetSubstrateInformations(
            MaterialLocation materialLocation,
            SerializableSubstrate substrateInformations)
        {
            if (substrateInformations != null)
            {
                var sourcePort = substrateInformations.SourcePort;
                var sourceSlot = substrateInformations.SourceSlot;
                var materialDimension = substrateInformations.MaterialDimension;
                var materialType = substrateInformations.MaterialType;
                var substrate = new Wafer($"C{sourcePort:00}:S{sourceSlot:00}")
                {
                    MaterialDimension = materialDimension, MaterialType = materialType
                };
                substrate.SetSource(null, sourcePort, sourceSlot, DateTime.Now);
                substrate.SetLocation(materialLocation);
            }
        }

        private void RefreshState()
        {
            var devices = this.AllOfType<GenericDevice>()
                .Where(
                    d => (d is not ILoadPort loadPort || loadPort.IsInService)
                         && (d is not IProcessModule pm || !pm.IsOutOfService))
                .ToList();
            if (Alarms.Any(al => al.State == AlarmState.Set))
            {
                SetState(OperatingModes.Maintenance);
            }
            else if (devices.Any(d => d.State == OperatingModes.Initialization))
            {
                SetState(OperatingModes.Initialization);
            }
            else if (devices.Any(d => d.State == OperatingModes.Executing))
            {
                SetState(OperatingModes.Executing);
            }
            else if (devices.Any(d => d is not ILoadPort && d.State == OperatingModes.Maintenance)
                     || devices.Where(d => d is ILoadPort)
                         .All(d => d.State == OperatingModes.Maintenance))
            {
                SetState(OperatingModes.Maintenance);
            }
            else
            {
                SetState(OperatingModes.Idle);
            }
        }

        #endregion Private Methods

        #region Other Methods

        protected abstract void SetArmExtendedInterlock(
            TransferLocation? location,
            bool isExtended);

        public void SetDevicesFolderPath(string devicesFolderPath)
        {
            _persistentLocationFilePath = Path.Combine(
                devicesFolderPath,
                $".\\Devices\\{nameof(Efem)}\\Resources\\PersistentLocationInformations.xml");
        }

        #endregion Other Methods

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var device in this.AllDevices())
                {
                    device.StatusValueChanged -= Device_StatusValueChanged;
                }

                if (_aligner != null)
                {
                    _aligner.Location.PropertyChanged -= AlignerLocation_PropertyChanged;
                    _aligner.CommandExecutionStateChanged -= Aligner_CommandExecutionStateChanged;
                    _aligner = null;
                }

                if (_robot != null)
                {
                    _robot.CommandExecutionStateChanged -= Robot_CommandExecutionStateChanged;
                    _robot.UpperArmLocation.PropertyChanged -= UpperArmLocation_PropertyChanged;
                    _robot.LowerArmLocation.PropertyChanged -= LowerArmLocation_PropertyChanged;
                    _robot = null;
                }

                ConfigManager.CurrentChanged -= ConfigManager_CurrentChanged;
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
