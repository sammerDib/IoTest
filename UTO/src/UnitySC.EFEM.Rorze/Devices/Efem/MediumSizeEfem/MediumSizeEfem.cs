using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0;
using UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort;
using UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Converters;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Enums;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;

namespace UnitySC.EFEM.Rorze.Devices.Efem.MediumSizeEfem
{
    public partial class MediumSizeEfem : IRorzeStoppingPositionConverterCallBack
    {
        #region Fields

        private const string _safetyDoorOpenAlarmKey = "Efem_011";
        private const string _vacuumPressureSensorAlarmKey = "Efem_012";
        private const string _airPressureSensorAlarmKey = "Efem_013";
        private Dio0 Dio0 { get; set; }
        private Dio1MediumSizeEfem Dio1 { get; set; }
        private MapperRR75x RobotMapper { get; set; }

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            var robot = this.TryGetDevice<MapperRR75x>();
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    Dio0 = this.GetTopDeviceContainer().AllDevices<Dio0>().FirstOrDefault();
                    if (Dio0 == null)
                    {
                        throw new InvalidOperationException(
                            $"Mandatory device of type {nameof(IoModule.RC550.Dio0.Dio0)} is not found in equipment model tree.");
                    }

                    Dio1 = this.GetTopDeviceContainer().AllDevices<Dio1MediumSizeEfem>().FirstOrDefault();
                    if (Dio1 == null)
                    {
                        throw new InvalidOperationException(
                            $"Mandatory device of type {nameof(Dio1MediumSizeEfem)} is not found in equipment model tree.");
                    }

                    Dio1.StatusValueChanged += Dio1_StatusValueChanged;
                    Dio1.CommandExecutionStateChanged += Dio1_CommandExecutionStateChanged;

                    // Even if EFEM is not communicating (with its DIO card), we want to initialize child devices
                    DeviceType.RemovePrecondition(
                        nameof(IGenericDevice.Initialize),
                        typeof(IsCommunicating));

                    if (robot != null)
                    {
                        RobotMapper = robot;
                        RobotMapper.CommandExecutionStateChanged += RobotMapper_CommandExecutionStateChanged;
                    }

                    var layingPlanLoadPorts = this.GetEquipment().AllDevices<LayingPlanLoadPort>().ToList();
                    foreach (var layingPlanLoadPort in layingPlanLoadPorts)
                    {
                        layingPlanLoadPort.StatusValueChanged += LayingPlanLoadPort_StatusValueChanged;
                    }

                    //Sensor not present 
                    AirSensor = true;
                    SafetyDoorSensor = true;
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region IRorzeStoppingPositionConverterCallBack

        public SampleDimension GetSampleDimension(TransferLocation transferLocation, RobotArm arm)
        {
            // If wafer is on arm, take dimension of the wafer on arm.
            var waferOnArmDimension = GetSampleDimensionOnArm(arm);
            if (waferOnArmDimension != null)
            {
                return (SampleDimension)waferOnArmDimension;
            }

            // No wafer on arm, try to get wafer size of transferLocation if any,
            // Elsewhere give by default the 300mm dimension because it is the standard use case of the current EFEM.
            return ToSampleDimension(transferLocation);
        }

        public string GetInnerLocation(TransferLocation transferLocation)
        {
            switch (transferLocation)
            {
                case TransferLocation.LoadPort1:
                case TransferLocation.LoadPort2:
                case TransferLocation.LoadPort3:
                case TransferLocation.LoadPort4:
                case TransferLocation.LoadPort5:
                case TransferLocation.LoadPort6:
                case TransferLocation.LoadPort7:
                case TransferLocation.LoadPort8:
                case TransferLocation.LoadPort9:
                    return "FUP1";

                // There are no inner location type for aligner and process modules
                case TransferLocation.PreAlignerA:
                case TransferLocation.PreAlignerD:
                case TransferLocation.ProcessModuleA:
                case TransferLocation.ProcessModuleB:
                case TransferLocation.ProcessModuleC:
                case TransferLocation.ProcessModuleD:
                    return "";

                // Not managed transfer locations
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(transferLocation),
                        transferLocation,
                        null);
            }
        }

        #endregion

        #region Event Handlers

        private void Dio1_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status.Name)
            {
                case nameof(IDio1.I_MaintenanceSwitch):
                    OperationMode = Dio1.I_MaintenanceSwitch
                        ? OperationMode.Maintenance
                        : OperationMode.Remote;
                    break;
                case nameof(IDio1.I_PressureSensor_VAC):
                    VacuumSensor = Dio1.I_PressureSensor_VAC;
                    if (!VacuumSensor)
                    {
                        SetAlarm(_vacuumPressureSensorAlarmKey);
                    }
                    break;
                case nameof(IDio1.I_PressureSensor_AIR):
                    AirSensor = true;
                    if (!AirSensor)
                    {
                        SetAlarm(_airPressureSensorAlarmKey);
                    }
                    break;
                case nameof(IDio1.I_PressureSensor_ION_AIR):
                    IonizerAirState = Dio1.I_PressureSensor_ION_AIR;
                    break;
                case nameof(IDio1.I_Ionizer1Alarm):
                    IonizerAlarm = Dio1.I_Ionizer1Alarm;
                    break;
                case nameof(IDio1.I_DoorStatus):
                    SafetyDoorSensor = true;
                    if (!SafetyDoorSensor)
                    {
                        SetAlarm(_safetyDoorOpenAlarmKey);
                    }
                    break;
                case nameof(IDio1.I_LightCurtain):
                    RefreshLightCurtain();
                    break;
                case nameof(IDio1.IsCommunicating):
                    IsCommunicating = Dio1.IsCommunicating;
                    break;
                case nameof(IDio1.IsCommunicationStarted):
                    IsCommunicationStarted = Dio1.IsCommunicationStarted;
                    break;
            }

            if (State == OperatingModes.Maintenance || State == OperatingModes.Idle)
            {
                SetState(
                    !IsCommunicating
                        ? OperatingModes.Maintenance
                        : OperatingModes.Idle);
            }
        }

        private void Dio1_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            if (e.Execution.Context.Command.Name != nameof(IGenericDevice.Initialize))
            {
                return;
            }

            if (e.Execution.CurrentState != ExecutionState.Success
                && e.Execution.CurrentState != ExecutionState.Failed)
            {
                return;
            }

            if (!VacuumSensor)
            {
                SetAlarm(_vacuumPressureSensorAlarmKey);
            }

            if (!AirSensor)
            {
                SetAlarm(_airPressureSensorAlarmKey);
            }

            if (!SafetyDoorSensor)
            {
                SetAlarm(_safetyDoorOpenAlarmKey);
            }
        }

        private void LayingPlanLoadPort_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not LayingPlanLoadPort layingPlanLoadPort)
            {
                return;
            }
            
            if (e.Status.Name == nameof(ILayingPlanLoadPort.MappingRequested)
                && layingPlanLoadPort.MappingRequested)
            {
                if (RobotMapper.State == OperatingModes.Idle)
                {
                    RobotMapper.MapLocationAsync(layingPlanLoadPort);
                }
                else
                {
                    RobotMapper.EnqueueMapping(layingPlanLoadPort);
                }
            }
        }

        private void RobotMapper_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            if(e.NewState is ExecutionState.Success or ExecutionState.Failed
                && e.Execution.Context.Command.Name is nameof(IRobot.Place)
                                                    or nameof(IRobot.Pick)
                                                    or nameof(IRobot.Transfer)
                                                    or nameof(IRobot.Swap))
            {
                var layingPlanLoadPorts = this.GetEquipment().AllDevices<LayingPlanLoadPort>().ToList();

                switch (RobotMapper.Position)
                {
                    case TransferLocation.LoadPort1:
                    case TransferLocation.LoadPort2:
                    case TransferLocation.LoadPort3:
                    case TransferLocation.LoadPort4:
                        layingPlanLoadPorts.FirstOrDefault(lp => lp.InstanceId == (int)RobotMapper.Position)?.CheckWaferProtrudeError();
                        break;
                }
            }
        }
        #endregion Event Handlers

        #region Other Methods

        public override (DevicePosition, int) GetPosition(Device device)
        {
            switch (device)
            {
                case Equipment.Abstractions.Devices.LoadPort.LoadPort:
                    var index = device.InstanceId;
                    if (index >= 3)
                    {
                        index++;
                    }
                    return (DevicePosition.Bottom,index);
                case Equipment.Abstractions.Devices.Aligner.Aligner:
                    return (DevicePosition.Bottom, 3);
                case ProcessModule:
                    return (DevicePosition.Top, device.InstanceId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(device), device, null);
            }
        }

        protected override void RefreshLightCurtain()
        {
            LightCurtainBeam = Configuration.LightCurtainWiring == LightCurtainWiring.PNP
                ? Dio1.I_LightCurtain
                : !Dio1.I_LightCurtain;
        }

        protected override void SetArmExtendedInterlock(TransferLocation? location, bool isExtended)
        {
            var dio1Signal = new Dio1MediumSizeEfemSignalData();
            if (location != null)
            {
                // Not extended in PM when not extended OR not facing PM
                dio1Signal.RobotArmNotExtended_PM1 =
                    !isExtended || location != TransferLocation.ProcessModuleA;
                dio1Signal.RobotArmNotExtended_PM2 =
                    !isExtended || location != TransferLocation.ProcessModuleB;
                dio1Signal.RobotArmNotExtended_PM3 =
                    !isExtended || location != TransferLocation.ProcessModuleC;
            }
            else
            {
                // The robot is not facing a PM (or other known location) so it's most likely not extended in any PM
                dio1Signal.RobotArmNotExtended_PM1 = true;
                dio1Signal.RobotArmNotExtended_PM2 = true;
                dio1Signal.RobotArmNotExtended_PM3 = true;
            }
            if (Dio1.IsCommunicating)
            {
                Dio1.SetOutputSignal(dio1Signal);
            }
        }

        private SampleDimension? GetSampleDimensionOnArm(RobotArm arm)
        {
            var robot = this.TryGetDevice<Equipment.Abstractions.Devices.Robot.Robot>();
            switch (arm)
            {
                case RobotArm.Arm1:
                    return robot.UpperArmLocation.Substrate?.MaterialDimension;

                case RobotArm.Arm2:
                    return robot.LowerArmLocation.Substrate?.MaterialDimension;

                default:
                    throw new ArgumentOutOfRangeException(nameof(arm), arm, null);
            }
        }

        private SampleDimension ToSampleDimension(TransferLocation transferLocation)
        {
            switch (transferLocation)
            {
                case TransferLocation.LoadPort1:
                case TransferLocation.LoadPort2:
                case TransferLocation.LoadPort3:
                case TransferLocation.LoadPort4:
                case TransferLocation.LoadPort5:
                case TransferLocation.LoadPort6:
                case TransferLocation.LoadPort7:
                case TransferLocation.LoadPort8:
                case TransferLocation.LoadPort9:
                    var loadPorts = this.AllDevices<Equipment.Abstractions.Devices.LoadPort.LoadPort>();
                    var loadPortId = transferLocation - TransferLocation.LoadPort1 + 1;

                    if (loadPorts.FirstOrDefault(lp => lp.InstanceId == loadPortId) is not { } loadPort)
                        throw new InvalidOperationException(
                            $"{nameof(RobotStoppingPositionConverter)} - Current EFEM does not contain any {transferLocation}.");

                    return loadPort.GetMaterialDimension();

                case TransferLocation.PreAlignerA:
                    var aligner = this.TryGetDevice<Equipment.Abstractions.Devices.Aligner.Aligner>();
                    return aligner.GetMaterialDimension();

                case TransferLocation.ProcessModuleA:
                case TransferLocation.ProcessModuleB:
                case TransferLocation.ProcessModuleC:
                case TransferLocation.ProcessModuleD:
                    var processModules = this.GetEquipment().AllDevices<ProcessModule>();
                    var processModuleId = transferLocation - TransferLocation.ProcessModuleA + 1;

                    if (processModules.FirstOrDefault(pm => pm.InstanceId == processModuleId) is not { } processModule)
                        throw new InvalidOperationException(
                            $"{nameof(RobotStoppingPositionConverter)} - Current EFEM does not contain any {transferLocation}.");

                    return processModule.GetMaterialDimension();

                // Not managed transfer locations
                default:
                    throw new ArgumentOutOfRangeException(nameof(transferLocation), transferLocation, null);
            }
        }

        public override bool CanReleaseRobot()
        {
            return false;
        }

        public override string ReadSubstrateId(ReaderSide readerSide, string frontRecipeName, string backRecipeName)
        {
            try
            {
                var dimension = this.TryGetDevice<Equipment.Abstractions.Devices.Aligner.Aligner>().WaferDimension;

                switch (dimension)
                {
                    case SampleDimension.S100mm:
                    case SampleDimension.S150mm:
                    case SampleDimension.S200mm:
                        return ReadSubstrateIdOnRobotArm(
                            readerSide,
                            frontRecipeName,
                            backRecipeName);

                    default:
                        throw new InvalidOperationException($"Substrate id reading for sample dimension {dimension} is not supported");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return string.Empty;
            }
        }

        private string ReadSubstrateIdOnRobotArm(ReaderSide readerSide, string frontRecipeName, string backRecipeName)
        {
            var robot = this.TryGetDevice<Equipment.Abstractions.Devices.Robot.Robot>();
            var aligner = this.TryGetDevice<Equipment.Abstractions.Devices.Aligner.Aligner>();


            //First select robot arm to use
            var robotArm = RobotArm.Arm1;
            if (robot.UpperArmLocation.Wafer != null)
            {
                robotArm = RobotArm.Arm2;
            }

            //Then prepare aligner for pick
            aligner.PrepareTransfer(
                robotArm == RobotArm.Arm1
                    ? robot.Configuration.UpperArm.EffectorType
                    : robot.Configuration.LowerArm.EffectorType,
                aligner.GetMaterialDimension(),
                aligner.GetMaterialType());

            //Then pick the wafer in the aligner with robot selected arm
            robot.Pick(robotArm, aligner, 1);

            //Then move down the aligner lift pins
            aligner.MoveZAxis(true);

            //Then go to the substrate id reader position
            robot.ExtendArm(robotArm, TransferLocation.PreAlignerD, 1);

            //read wafer id
            var acquiredId = base.ReadSubstrateId(
                readerSide,
                frontRecipeName,
                backRecipeName);

            //Then prepare aligner for place
            robot.GoToSpecifiedLocation(
                aligner,
                1,
                robotArm,
                false);

            aligner.PrepareTransfer(
                robotArm == RobotArm.Arm1
                    ? robot.Configuration.UpperArm.EffectorType
                    : robot.Configuration.LowerArm.EffectorType,
                robotArm == RobotArm.Arm1
                    ? robot.UpperArmLocation.Wafer!.MaterialDimension
                    : robot.LowerArmLocation.Wafer.MaterialDimension,
                robotArm == RobotArm.Arm1
                    ? robot.UpperArmLocation.Wafer!.MaterialType
                    : robot.LowerArmLocation.Wafer.MaterialType);

            //Then place the wafer in the aligner with robot upper arm
            robot.Place(robotArm, aligner, 1);

            return acquiredId;
        }

        #endregion Other Methods

        #region Configurations

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Efem)}/{nameof(MediumSizeEfem)}/Resources";

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing && Dio1 != null)
            {
                Dio1.StatusValueChanged -= Dio1_StatusValueChanged;
                Dio1.CommandExecutionStateChanged -= Dio1_CommandExecutionStateChanged;
                Dio1 = null;

                RobotMapper.CommandExecutionStateChanged -= RobotMapper_CommandExecutionStateChanged;
                RobotMapper = null;

                var layingPlanLoadPorts = this.GetEquipment().AllDevices<LayingPlanLoadPort>().ToList();
                foreach (var layingPlanLoadPort in layingPlanLoadPorts)
                {
                    layingPlanLoadPort.StatusValueChanged -= LayingPlanLoadPort_StatusValueChanged;
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
