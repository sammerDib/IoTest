using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using BAI.Maint.HwComp.Dio;
using BAI.Systems.Common;
using BAI.Systems.Modules.EFEM;

using UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem.Configuration;
using UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem.Resources;
using UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

namespace UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem
{
    public partial class BrooksEfem :IConfigurableDevice<BrooksEfemConfiguration>, IProcessModuleIos
    {
        #region Fields

        private EfemProxy _efemProxy;
        private DioCtrlProxy _dioCtrlProxy;

        #endregion

        #region IConfigurableDevice

        public new BrooksEfemConfiguration Configuration
            => ConfigurationExtension.Cast<BrooksEfemConfiguration>(base.Configuration);

        public new BrooksEfemConfiguration CreateDefaultConfiguration()
        {
            return new BrooksEfemConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Efem)}/{nameof(BrooksEfem)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration<BrooksEfemConfiguration>(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }
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
                        _efemProxy = new EfemProxy(Configuration.BrooksEfemName, Configuration.BrooksClientName);
                        _dioCtrlProxy = new DioCtrlProxy(Configuration.BrooksDioName, Configuration.BrooksClientName);
                        _dioCtrlProxy.SignalChanged += DioCtrlProxy_SignalChanged;
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            try
            {
                if (!_efemProxy.Connected)
                {
                    _efemProxy.Connect();
                }

                if (!_efemProxy.HaveControl())
                {
                    _efemProxy.TakeControl();
                }

                IsCommunicationStarted = true;
                IsCommunicating = true;
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
                if (_efemProxy.Connected)
                {
                    _efemProxy.Disconnect();
                }

                IsCommunicationStarted = false;
                IsCommunicating = false;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
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

            _efemProxy.Ems();
        }

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                //Base init
                base.InternalInitialize(mustForceInit);

                //Device init
                _efemProxy.ClearAlarm();
                _efemProxy.Initialize();

                //Status update
                RefreshLightCurtain();

                //Check device ready
                if (!_efemProxy.IsOperable())
                {
                    throw new InvalidOperationException(Messages.EfemNotOperable);
                }

                if (_efemProxy.IsInMaintenance())
                {
                    throw new InvalidOperationException(Messages.EfemInMaintenance);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion

        #region IEfem

        protected override void RefreshLightCurtain()
        {
            var lightCurtains = new List<HighLowState>();
            foreach (var loadPort in this
                         .AllDevices<BrooksLoadPort>())
            {
                var lightCurtain = _dioCtrlProxy.GetDigitalSignalValue(loadPort.Configuration.LightCurtainNodeSignal);
                lightCurtains.Add(lightCurtain);
            }
            
            LightCurtainBeam = lightCurtains.Any(s => s == HighLowState.High);
        }

        public override (DevicePosition, int) GetPosition(Device device)
        {
            switch (device)
            {
                case Equipment.Abstractions.Devices.LoadPort.LoadPort:
                    return (DevicePosition.Bottom, device.InstanceId);
                case Equipment.Abstractions.Devices.Aligner.Aligner:
                    return (DevicePosition.Right, device.InstanceId);
                case ProcessModule:
                    return (DevicePosition.Left, device.InstanceId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(device), device, null);
            }
        }

        protected override void SetArmExtendedInterlock(TransferLocation? location, bool isExtended)
        {
            //Auto managed by brooks
        }

        protected override void Device_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            base.Device_StatusValueChanged(sender, e);

            if (sender is Equipment.Abstractions.Devices.Robot.Robot
                && e.Status.Name == nameof(IRobot.Position))
            {
                RefreshProcessModuleIos();
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
                    case SampleDimension.S200mm:
                    case SampleDimension.S300mm:
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

        #endregion

        #region Event handler

        private void DioCtrlProxy_SignalChanged(
            string nodeSignal,
            byte port,
            byte bit,
            HighLowState newValue)
        {

            if (this.AllDevices<BrooksLoadPort>().Any(lp=>lp.Configuration.LightCurtainNodeSignal == nodeSignal))
            {
                RefreshLightCurtain();
            }

            if (nodeSignal == Configuration.AirNodeSignal)
            {
                AirSensor = newValue == HighLowState.High;
            }
            else if (nodeSignal == Configuration.DoorSensor1NodeSignal
                     || nodeSignal == Configuration.DoorSensor2NodeSignal)
            {
                var sensor1 = _dioCtrlProxy.GetDigitalSignalValue(Configuration.DoorSensor1NodeSignal) == HighLowState.High;
                var sensor2 = _dioCtrlProxy.GetDigitalSignalValue(Configuration.DoorSensor2NodeSignal) == HighLowState.High;
                SafetyDoorSensor = sensor1 && sensor2;
            }
            else if (nodeSignal == Configuration.InterlockSensor1NodeSignal
                     || nodeSignal == Configuration.InterlockSensor2NodeSignal)
            {
                var interlock1 = _dioCtrlProxy.GetDigitalSignalValue(Configuration.InterlockSensor1NodeSignal) == HighLowState.High;
                var interlock2 = _dioCtrlProxy.GetDigitalSignalValue(Configuration.InterlockSensor2NodeSignal) == HighLowState.High;
                Interlock = interlock1 && interlock2;
            }
            else if (nodeSignal == Configuration.PressureNodeSignal)
            {
                VacuumSensor = newValue == HighLowState.High;
            }

            IonizerAirState = false;
            IonizerAlarm = false;
        }

        #endregion

        #region IProcessModuleIos

        private void RefreshProcessModuleIos()
        {
           var processModules = this.GetTopDeviceContainer()
                .AllDevices<ProcessModule>().ToList();

           foreach (var processModule in processModules)
           {
               switch (processModule.InstanceId)
               {
                    case 1: //TODO remove try/catch after test on machine
                        try
                        {
                            I_PM1_DoorOpened = I_PM1_ReadyToLoadUnload = _efemProxy.PortIsReadyForEndEffecter("U");
                        }
                        catch (Exception)
                        {
                            I_PM1_DoorOpened = I_PM1_ReadyToLoadUnload = true;
                        }
                        break;
                    case 2:
                        try
                        {
                            I_PM2_DoorOpened = I_PM2_ReadyToLoadUnload = _efemProxy.PortIsReadyForEndEffecter("V");
                        }
                        catch (Exception)
                        {
                            I_PM2_DoorOpened = I_PM2_ReadyToLoadUnload = true;
                        }
                        break;
                    case 3:
                        try
                        {
                            I_PM3_DoorOpened = I_PM3_ReadyToLoadUnload = _efemProxy.PortIsReadyForEndEffecter("W");
                        }
                        catch (Exception)
                        {
                            I_PM3_DoorOpened = I_PM3_ReadyToLoadUnload = true;
                        }
                        break;
               }
           }
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
                if (_efemProxy != null)
                {
                    _efemProxy.Dispose();
                }

                if (_dioCtrlProxy != null)
                {
                    _dioCtrlProxy.SignalChanged -= DioCtrlProxy_SignalChanged;
                    _dioCtrlProxy.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
