using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.CommandConstants;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Converters;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Enums;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;

namespace UnitySC.EFEM.Rorze.Devices.Efem.UniversalEfem
{
    public partial class UniversalEfem : IRorzeStoppingPositionConverterCallBack
    {
        #region Fields

        private const string _safetyDoorOpenAlarmKey = "Efem_011";
        private const string _vacuumPressureSensorAlarmKey = "Efem_012";
        private const string _airPressureSensorAlarmKey = "Efem_013";

        #endregion

        private Dio1 Dio1 { get; set; }
        private Dio2 Dio2 { get; set; }

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
                    var robot = this.TryGetDevice<RR75x>();
                    if (robot != null && DeviceContainerExtensions.AllOfType<RE201>(this).Any())
                    {
                        //In case we are using SMIF load ports, robot must load SMIF stopping position config
                        robot.ConfigurationFileName = "ConfigurationSmif.xml";
                    }

                    break;
                case SetupPhase.SettingUp:
                    Dio1 = IExtendedObjectExtensions.GetTopDeviceContainer(this)
                        .AllDevices<Dio1>()
                        .FirstOrDefault();
                    if (Dio1 == null)
                    {
                        throw new InvalidOperationException(
                            $"Mandatory device of type {nameof(IoModule.RC530.Dio1.Dio1)} is not found in equipment model tree.");
                    }

                    Dio2 = IExtendedObjectExtensions.GetTopDeviceContainer(this)
                        .AllDevices<Dio2>()
                        .FirstOrDefault();
                    if (Dio2 == null)
                    {
                        throw new InvalidOperationException(
                            $"Mandatory device of type {nameof(IoModule.RC530.Dio2.Dio2)} is not found in equipment model tree.");
                    }

                    Dio1.StatusValueChanged += Dio1_StatusValueChanged;
                    Dio1.CommandExecutionStateChanged += Dio1_CommandExecutionStateChanged;

                    // Even if EFEM is not communicating (with its DIO card), we want to initialize child devices
                    DeviceType.RemovePrecondition(
                        nameof(IGenericDevice.Initialize),
                        typeof(IsCommunicating));

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

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
                    AirSensor = Dio1.I_PressureSensor_AIR;
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
                    SafetyDoorSensor = Dio1.I_DoorStatus;
                    if (!SafetyDoorSensor)
                    {
                        SetAlarm(_safetyDoorOpenAlarmKey);
                    }

                    break;
                case nameof(IDio1.I_RV201Interlock):
                    Interlock = !Dio1.I_RV201Interlock; // 0 for Normal, 1 for Failed: The reverse of efemIoCard
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
            if (e.Execution.Context.Command.Name == nameof(IGenericDevice.Initialize))
            {
                if (e.Execution.CurrentState is not (ExecutionState.Success or ExecutionState.Failed))
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
        }

        #endregion Event Handlers

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
                    return GetLoadPortInnerLocationType(
                        transferLocation - TransferLocation.LoadPort1 + 1);

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

        #region Other Methods

        protected override Material GetMaterial(IMaterialLocationContainer device, byte slot)
        {
            Material material = null;
            switch (device)
            {
                case RE201 smifLoadPort:
                    if (0 < smifLoadPort.CurrentSlot
                        && smifLoadPort.CurrentSlot
                        <= smifLoadPort.Carrier?.MaterialLocations.Count)
                    {
                        material = smifLoadPort.Carrier
                            ?.MaterialLocations[smifLoadPort.CurrentSlot - 1].Material;
                    }
                    break;

                default:
                    material = base.GetMaterial(device, slot);
                    break;
            }

            return material;
        }

        protected override MaterialLocation GetLocation(
            IMaterialLocationContainer device,
            byte slot)
        {
            MaterialLocation location = null;
            switch (device)
            {
                case RE201 smifLoadPort:
                    if (0 < smifLoadPort.CurrentSlot
                        && smifLoadPort.CurrentSlot
                        <= smifLoadPort.Carrier?.MaterialLocations.Count)
                    {
                        location =
                            smifLoadPort.Carrier?.MaterialLocations[smifLoadPort.CurrentSlot - 1];
                    }

                    break;
                default:
                    location = base.GetLocation(device, slot);

                    break;
            }

            return location;
        }

        protected override void RefreshLightCurtain()
        {
            LightCurtainBeam = Configuration.LightCurtainWiring == LightCurtainWiring.PNP
                ? Dio1.I_LightCurtain
                : !Dio1.I_LightCurtain;
        }

        public override (DevicePosition,int) GetPosition(Device device)
        {
            switch (device)
            {
                case Equipment.Abstractions.Devices.LoadPort.LoadPort:
                    return (DevicePosition.Bottom, device.InstanceId);
                case Equipment.Abstractions.Devices.Aligner.Aligner:
                    return (DevicePosition.Right, device.InstanceId);
                case ProcessModule:
                    return (DevicePosition.Top, device.InstanceId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(device), device, null);
            }
        }

        protected override void SetArmExtendedInterlock(TransferLocation? location, bool isExtended)
        {
            var dio2Signal = new Dio2SignalData();
            if (location != null)
            {
                // Not extended in PM when not extended OR not facing PM
                dio2Signal.RobotArmNotExtended_PM1 =
                    !isExtended || location != TransferLocation.ProcessModuleA;
                dio2Signal.RobotArmNotExtended_PM2 =
                    !isExtended || location != TransferLocation.ProcessModuleB;
                dio2Signal.RobotArmNotExtended_PM3 =
                    !isExtended || location != TransferLocation.ProcessModuleC;
            }
            else
            {
                // Unknown position
                dio2Signal.RobotArmNotExtended_PM1 = !isExtended;
                dio2Signal.RobotArmNotExtended_PM2 = !isExtended;
                dio2Signal.RobotArmNotExtended_PM3 = !isExtended;
            }

            if (Dio2.IsCommunicating)
            {
                Dio2.SetOutputSignal(dio2Signal);
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

        /// <summary>
        /// Convert a <see cref="TransferLocation"/> into a <see cref="SampleDimension"/> according to the current EFEM state.
        /// </summary>
        /// <remarks>If no dimension is found, return <see cref="RorzeConstants.SubstrateDimension"/> which is the default value for the current EFEM.</remarks>
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

        private string GetLoadPortInnerLocationType(int loadPortId)
        {
            var loadPorts = this.AllDevices<Equipment.Abstractions.Devices.LoadPort.LoadPort>();

            if (loadPorts.FirstOrDefault(lp=>lp.InstanceId == loadPortId) is not {} loadPort)
                throw new InvalidOperationException(
                    $"{nameof(RobotStoppingPositionConverter)} - Current EFEM does not contain any load port {loadPortId}.");

            // If no carrier is present or if the load port is not a rorze load port, give FUP1 type by default
            if (loadPort.Carrier == null)
            {
                return CarrierTypeConstants.ToString(CarrierType.FOUP, 1);
            }

            switch (loadPort)
            {
                case IRV201 rv201LoadPort:
                    return CarrierTypeConstants.ToString(rv201LoadPort.CarrierType, rv201LoadPort.CarrierTypeIndex);

                case IRV101 rv101LoadPort:
                    return rv101LoadPort.CarrierTypeName;

                case IRE201 re201LoadPort:
                    return LoadPort.RE201.Driver.CommandConstants.CarrierTypeConstants.ToString(re201LoadPort.CarrierType, re201LoadPort.CarrierTypeIndex);

                default:
                    return CarrierTypeConstants.ToString(CarrierType.FOUP, 1);
            }
        }

        public override bool CanReleaseRobot()
        {
            var aligner = this.TryGetDevice<Equipment.Abstractions.Devices.Aligner.Aligner>();
            return aligner.WaferDimension is SampleDimension.S300mm or SampleDimension.S200mm;
        }

        public override string ReadSubstrateId(ReaderSide readerSide, string frontRecipeName, string backRecipeName)
        {
            try
            {
                var dimension = this.TryGetDevice<Equipment.Abstractions.Devices.Aligner.Aligner>().WaferDimension;

                switch (dimension)
                {
                    case SampleDimension.S150mm:
                        return ReadSubstrateIdOnRobotArm(
                            readerSide,
                            frontRecipeName,
                            backRecipeName);

                    case SampleDimension.S200mm:
                    case SampleDimension.S300mm:
                        return base.ReadSubstrateId(
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

        #region Configuration

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Efem)}/{nameof(UniversalEfem)}/Resources";

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing && Dio1 != null)
            {
                Dio1.StatusValueChanged -= Dio1_StatusValueChanged;
                Dio1.CommandExecutionStateChanged -= Dio1_CommandExecutionStateChanged;
                Dio1 = null;
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
