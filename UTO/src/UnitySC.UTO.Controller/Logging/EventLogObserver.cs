using System;
using System.Linq;

using AgilController.GUI.Configuration;

using Agileo.AlarmModeling.Configuration;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.EquipmentModeling;
using Agileo.GUI.Configuration;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.SemiDefinitions;

using UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager;
using UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager.Configuration;
using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule;
using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.Configuration;
using UnitySC.DataFlow.ProcessModules.Drivers.WCF;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Configuration;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Configuration;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Configuration;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Configuration;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Efem;
using UnitySC.Equipment.Abstractions.Devices.Ffu;
using UnitySC.Equipment.Abstractions.Devices.Ffu.Configuration;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.Robot.Configuration;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.GUI.Common.Configuration;
using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Configuration;
using UnitySC.UTO.Controller.Configuration;
using UnitySC.UTO.Controller.Remote;
using UnitySC.UTO.Controller.Remote.Constants;

using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;

namespace UnitySC.UTO.Controller.Logging
{
    public class EventLogObserver : IDisposable
    {
        #region Fields

        private bool _disposedValue;
        private readonly ILogger _logger;
        private Agileo.EquipmentModeling.Equipment _equipment;
        private Robot _robot;
        private Aligner _aligner;

        private GemController GemController => App.ControllerInstance.GemController;

        #endregion

        #region Constructor

        public EventLogObserver()
        {
            _logger = Logger.GetLogger(EventLogConstant.EventLog);
        }

        #endregion

        #region Public

        public void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            _equipment = equipment;

            var controller = equipment.AllOfType<Equipment.Devices.Controller.Controller>().First();
            var efem = controller.TryGetDevice<Efem>();
            _robot = efem.TryGetDevice<Robot>();
            _aligner = efem.TryGetDevice<Aligner>();

            _robot.PropertyChanged += Robot_PropertyChanged;
            _robot.CommandExecutionStateChanged += Robot_CommandExecutionStateChanged;
            _aligner.CommandExecutionStateChanged += Aligner_CommandExecutionStateChanged;

            foreach (var loadPort in equipment.AllOfType<LoadPort>())
            {
                loadPort.CommandExecutionStateChanged += LoadPort_CommandExecutionStateChanged;
            }

            GemController.E30Std.EquipmentConstantsServices.EquipmentConstantChanged +=
                EquipmentConstantsServices_EquipmentConstantChanged;
            GemController.E87Std.CarrierIdStateChanged += E87Std_CarrierIdStateChanged;
            GemController.E87Std.CarrierDisposed += E87Std_CarrierDisposed;
            GemController.E40Std.ProcessJobChanged += E40Std_ProcessJobChanged;

            App.ControllerInstance.ConfigurationManager.CurrentChanged +=
                ConfigurationManager_CurrentChanged;

            foreach (var device in _equipment.AllDevices())
            {
                if (device is IExtendedConfigurableDevice { ConfigManager: { } } configurableDevice)
                {
                    configurableDevice.ConfigManager.CurrentChanged +=
                        ConfigurationManager_CurrentChanged;
                }
            }
        }

        #endregion

        #region Event Handler

        private void EquipmentConstantsServices_EquipmentConstantChanged(
            object sender,
            VariableEventArgs e)
        {
            switch (e.Variable.Name)
            {
                case ECs.StopConfig:
                    GenerateConfigurationLog(
                        "Controller",
                        "Process",
                        e.Variable.Name,
                        e.Variable.ValueTo<string>(),
                        "$",
                        e.Variable.ID.ToString());
                    break;

                case ECs.CommunicationType:
                case ECs.IsActive:
                case ECs.DeviceID:
                case ECs.LocalIP:
                case ECs.LocalTCPPort:
                    GenerateConfigurationLog(
                        "Controller",
                        "Connection",
                        e.Variable.Name,
                        e.Variable.ValueTo<string>(),
                        "$",
                        e.Variable.ID.ToString());
                    break;

                case ECs.EstablishCommunicationsTime:
                case ECs.T3:
                case ECs.T5:
                case ECs.T6:
                case ECs.T7:
                case ECs.T8:
                case ECs.LinkTest:
                    GenerateConfigurationLog(
                        "Controller",
                        "Timeouts",
                        e.Variable.Name,
                        e.Variable.ValueTo<string>(),
                        "s",
                        e.Variable.ID.ToString());
                    break;

                case ECs.DefaultCommState:
                case ECs.DefaultControlState:
                case ECs.DefaultOnLineState:
                case ECs.DefaultControlOffLineState:
                case ECs.TimeFormat:
                    GenerateConfigurationLog(
                        "Controller",
                        "Default States",
                        e.Variable.Name,
                        e.Variable.ValueTo<string>(),
                        "$",
                        e.Variable.ID.ToString());
                    break;

                case ECs.LogFileName:
                case ECs.LogMode:
                    GenerateConfigurationLog(
                        "Controller",
                        "SECS-II Logs",
                        e.Variable.Name,
                        e.Variable.ValueTo<string>(),
                        "$",
                        e.Variable.ID.ToString());
                    break;

                case ECs.LogFileSize:
                    GenerateConfigurationLog(
                        "Controller",
                        "SECS-II Logs",
                        e.Variable.Name,
                        e.Variable.ValueTo<string>(),
                        "byte",
                        e.Variable.ID.ToString());
                    break;
            }
        }

        private void Robot_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IRobot.Speed):
                    GenerateConfigurationLog(
                        _robot.Name,
                        "Status",
                        e.PropertyName,
                        _robot.Speed.Percent.ToString("P"),
                        "%");
                    break;
            }
        }

        private void ConfigurationManager_CurrentChanged(
            object sender,
            Agileo.Common.Configuration.ConfigurationChangedEventArgs e)
        {
            switch (e.NewConfiguration)
            {
                case RR75xConfiguration newConfiguration:
                    {
                        var device = _equipment.AllOfType<Robot>()
                            .Single(dev => dev.ConfigManager.Equals(sender));
                        var oldConfiguration = e.OldConfiguration as RR75xConfiguration;
                        CompareRobotConfiguration(device.Name, oldConfiguration, newConfiguration);
                        return;
                    }
                case RA420Configuration newConfiguration:
                    {
                        var device = _equipment.AllOfType<Aligner>()
                            .Single(dev => dev.ConfigManager.Equals(sender));
                        var oldConfiguration = e.OldConfiguration as RA420Configuration;
                        CompareAlignerConfiguration(
                            device.Name,
                            oldConfiguration,
                            newConfiguration);
                        return;
                    }
                case RorzeLoadPortConfiguration newConfiguration:
                    {
                        var device = _equipment.AllOfType<LoadPort>()
                            .Single(dev => dev.ConfigManager.Equals(sender));
                        var oldConfiguration = e.OldConfiguration as RorzeLoadPortConfiguration;
                        CompareLoadPortConfiguration(
                            device.Name,
                            oldConfiguration,
                            newConfiguration);
                        return;
                    }
                case FfuConfiguration newConfiguration:
                    {
                        var device = _equipment.AllOfType<Ffu>()
                            .Single(dev => dev.ConfigManager.Equals(sender));
                        var oldConfiguration = e.OldConfiguration as FfuConfiguration;
                        CompareFfuConfiguration(device.Name, oldConfiguration, newConfiguration);
                        return;
                    }
                case UnityProcessModuleConfiguration newConfiguration:
                    {
                        var device = _equipment.AllOfType<UnityProcessModule>()
                            .Single(dev => dev.ConfigManager.Equals(sender));
                        var oldConfiguration =
                            e.OldConfiguration as UnityProcessModuleConfiguration;
                        ComparePmConfiguration(device.Name, oldConfiguration, newConfiguration);
                        return;
                    }
                case DataFlowManagerConfiguration newConfiguration:
                    {
                        var device = _equipment.AllOfType<DataFlowManager>()
                            .Single(dev => dev.ConfigManager.Equals(sender));
                        var oldConfiguration = e.OldConfiguration as DataFlowManagerConfiguration;
                        CompareDfConfiguration(device.Name, oldConfiguration, newConfiguration);
                        return;
                    }
                case PC1740Configuration newConfiguration:
                    {
                        var device = _equipment.AllOfType<SubstrateIdReader>()
                            .Single(dev => dev.ConfigManager.Equals(sender));
                        var oldConfiguration = e.OldConfiguration as PC1740Configuration;
                        CompareReaderConfiguration(device.Name, oldConfiguration, newConfiguration);
                        return;
                    }
                case GenericRC5xxConfiguration newConfiguration:
                    {
                        var device = _equipment.AllOfType<GenericRC5xx>()
                            .Single(dev => dev.ConfigManager.Equals(sender));
                        var oldConfiguration = e.OldConfiguration as GenericRC5xxConfiguration;
                        CompareIoModuleConfiguration(
                            device.Name,
                            oldConfiguration,
                            newConfiguration);
                        return;
                    }
                case ControllerConfiguration newConfiguration:
                    {
                        var oldConfiguration = e.OldConfiguration as ControllerConfiguration;
                        CompareControllerConfiguration(
                            "Controller",
                            oldConfiguration,
                            newConfiguration);
                        return;
                    }
            }
        }

        private void LoadPort_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            var commandName = e.Execution.Context.Command.Name;
            string eventId;

            if (string.IsNullOrEmpty(commandName))
            {
                return;
            }

            if (e.NewState is not ExecutionState.Success
                and not ExecutionState.Running
                and not ExecutionState.Failed)
            {
                return;
            }

            switch (commandName)
            {
                case nameof(ILoadPort.Clamp):
                    eventId = EventLogConstant.FunctionLog.LPChuck;
                    break;

                case nameof(ILoadPort.Unclamp):
                    eventId = EventLogConstant.FunctionLog.LPDeChuck;
                    break;

                case nameof(ILoadPort.Dock):
                    eventId = EventLogConstant.FunctionLog.LPForward;
                    break;

                case nameof(ILoadPort.Undock):
                    eventId = EventLogConstant.FunctionLog.LPBackward;
                    break;

                case nameof(ILoadPort.ReadCarrierId):
                    eventId = EventLogConstant.FunctionLog.ReadID;
                    break;

                case nameof(ILoadPort.Open):
                    eventId = EventLogConstant.FunctionLog.FoupDoorOpen;
                    break;

                case nameof(ILoadPort.Close):
                    eventId = EventLogConstant.FunctionLog.FoupDoorClose;
                    break;

                case nameof(ILoadPort.Map):
                    eventId = EventLogConstant.FunctionLog.Mapping;
                    break;

                default:
                    return;
            }

            GenerateFunctionLog(
                loadPort.Name,
                eventId,
                e.NewState == ExecutionState.Running
                    ? "Start"
                    : "End",
                "$",
                "$");
        }

        private void Aligner_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            var commandName = e.Execution.Context.Command.Name;
            string eventId;
            string materialId;

            if (string.IsNullOrEmpty(commandName))
            {
                return;
            }

            if (e.NewState is not ExecutionState.Success
                and not ExecutionState.Running
                and not ExecutionState.Failed)
            {
                return;
            }

            switch (commandName)
            {
                case nameof(IAligner.Align):
                    eventId = EventLogConstant.FunctionLog.Align;
                    break;

                case nameof(IAligner.Clamp):
                    eventId = EventLogConstant.FunctionLog.AlignerStageDown;
                    break;

                case nameof(IAligner.Unclamp):
                case nameof(IAligner.PrepareTransfer):
                    eventId = EventLogConstant.FunctionLog.AlignerStageUp;
                    break;

                default:
                    return;
            }

            if (_aligner.Location.Wafer != null && _aligner.Location.Substrate != null)
            {
                materialId =
                    $"{_aligner.Location.Wafer.LotId}:{_aligner.Location.Substrate.SourceSlot}";
            }
            else
            {
                materialId = "$";
            }

            GenerateFunctionLog(
                _aligner.Name,
                eventId,
                e.NewState == ExecutionState.Running
                    ? "Start"
                    : "End",
                materialId);
        }

        private void Robot_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            var commandName = e.Execution.Context.Command.Name;
            var context = e.Execution.Context;

            if (string.IsNullOrEmpty(commandName))
            {
                return;
            }

            if (e.NewState is not ExecutionState.Success
                and not ExecutionState.Running
                and not ExecutionState.Failed)
            {
                return;
            }

            IMaterialLocationContainer fromDevice;
            IMaterialLocationContainer toDevice;
            byte fromSlot;
            byte toSlot;
            string lotId;
            byte waferSlotNumber;

            switch (commandName)
            {
                case nameof(IRobot.Pick):
                    var robotArm = (RobotArm)context.GetArgument("arm");
                    fromDevice = (IMaterialLocationContainer)context.GetArgument("sourceDevice");
                    fromSlot = (byte)context.GetArgument("sourceSlot");

                    if (e.NewState == ExecutionState.Running)
                    {
                        if (fromDevice is LoadPort loadPort)
                        {
                            lotId = ((Wafer)loadPort.Carrier.MaterialLocations[fromSlot - 1]
                                .Material).LotId;
                            waferSlotNumber =
                                ((Wafer)loadPort.Carrier.MaterialLocations[fromSlot - 1].Material)
                                .SourceSlot;
                        }
                        else
                        {
                            lotId = ((Wafer)fromDevice.MaterialLocations[fromSlot - 1].Material)
                                .LotId;
                            waferSlotNumber =
                                ((Wafer)fromDevice.MaterialLocations[fromSlot - 1].Material)
                                .SourceSlot;
                        }
                    }
                    else
                    {
                        lotId = robotArm == RobotArm.Arm1
                            ? _robot.UpperArmLocation.Wafer.LotId
                            : _robot.LowerArmLocation.Wafer.LotId;

                        waferSlotNumber = robotArm == RobotArm.Arm1
                            ? _robot.UpperArmLocation.Wafer.SourceSlot
                            : _robot.LowerArmLocation.Wafer.SourceSlot;
                    }

                    GenerateTransferLog(
                        EventLogConstant.TransferLog.Get,
                        e.NewState == ExecutionState.Running
                            ? "Start"
                            : "End",
                        $"{lotId}:{waferSlotNumber:00}",
                        lotId,
                        fromDevice.Name,
                        fromSlot.ToString(),
                        _robot.Name,
                        robotArm == RobotArm.Arm1
                            ? "1"
                            : "2");
                    break;

                case nameof(IRobot.Place):
                    robotArm = (RobotArm)context.GetArgument("arm");
                    toDevice = (IMaterialLocationContainer)context.GetArgument("destinationDevice");
                    toSlot = (byte)context.GetArgument("destinationSlot");

                    if (e.NewState == ExecutionState.Running)
                    {
                        lotId = robotArm == RobotArm.Arm1
                            ? _robot.UpperArmLocation.Wafer.LotId
                            : _robot.LowerArmLocation.Wafer.LotId;

                        waferSlotNumber = robotArm == RobotArm.Arm1
                            ? _robot.UpperArmLocation.Wafer.SourceSlot
                            : _robot.LowerArmLocation.Wafer.SourceSlot;
                    }
                    else
                    {
                        if (toDevice is LoadPort loadPort)
                        {
                            lotId = ((Wafer)loadPort.Carrier.MaterialLocations[toSlot - 1].Material)
                                .LotId;
                            waferSlotNumber =
                                ((Wafer)loadPort.Carrier.MaterialLocations[toSlot - 1].Material)
                                .SourceSlot;
                        }
                        else
                        {
                            lotId = ((Wafer)toDevice.MaterialLocations[toSlot - 1].Material).LotId;
                            waferSlotNumber =
                                ((Wafer)toDevice.MaterialLocations[toSlot - 1].Material).SourceSlot;
                        }
                    }

                    GenerateTransferLog(
                        EventLogConstant.TransferLog.Put,
                        e.NewState == ExecutionState.Running
                            ? "Start"
                            : "End",
                        $"{lotId}:{waferSlotNumber:00}",
                        lotId,
                        _robot.Name,
                        robotArm == RobotArm.Arm1
                            ? "1"
                            : "2",
                        toDevice.Name,
                        toSlot.ToString());
                    break;

                case nameof(IRobot.GoToSpecifiedLocation):
                    robotArm = (RobotArm)context.GetArgument("arm");
                    toDevice = (IMaterialLocationContainer)context.GetArgument("destinationDevice");
                    toSlot = (byte)context.GetArgument("destinationSlot");

                    string materialId;
                    string sFromDevice;
                    string sFromSLot;

                    if (robotArm == RobotArm.Arm1)
                    {
                        sFromDevice = _robot.UpperArmHistory.Location.ToString();
                        sFromSLot = _robot.UpperArmHistory.Slot.ToString();

                        if (_robot.UpperArmLocation.Wafer != null
                            && _robot.UpperArmLocation.Substrate != null)
                        {
                            materialId =
                                $"{_robot.UpperArmLocation.Wafer.LotId}:{_robot.UpperArmLocation.Substrate.SourceSlot:00}";
                            lotId = _robot.UpperArmLocation.Wafer.LotId;
                        }
                        else
                        {
                            materialId = "$";
                            lotId = "$";
                        }
                    }
                    else
                    {
                        sFromDevice = _robot.LowerArmHistory.Location.ToString();
                        sFromSLot = _robot.LowerArmHistory.Slot.ToString();

                        if (_robot.LowerArmLocation.Wafer != null
                            && _robot.LowerArmLocation.Substrate != null)
                        {
                            materialId =
                                $"{_robot.LowerArmLocation.Wafer.LotId}:{_robot.LowerArmLocation.Substrate.SourceSlot:00}";
                            lotId = _robot.LowerArmLocation.Wafer.LotId;
                        }
                        else
                        {
                            materialId = "$";
                            lotId = "$";
                        }
                    }

                    GenerateTransferLog(
                        EventLogConstant.TransferLog.Move,
                        e.NewState == ExecutionState.Running
                            ? "Start"
                            : "End",
                        materialId,
                        lotId,
                        sFromDevice,
                        sFromSLot,
                        toDevice.Name,
                        toSlot.ToString());
                    break;
            }
        }

        private void E87Std_CarrierIdStateChanged(object sender, CarrierIdStateChangedArgs e)
        {
            if (e.CurrentState != CarrierIdStatus.IDNotRead)
            {
                var lp = _equipment.AllOfType<LoadPort>()
                    .FirstOrDefault(l => l.InstanceId == e.Carrier.PortID);
                if (lp == null)
                {
                    return;
                }

                GenerateLotEventLog(
                    lp.Name,
                    EventLogConstant.LotEventLog.CarrierLoad,
                    "$",
                    "$",
                    "$",
                    lp.Carrier.Id);
            }
        }

        private void E87Std_CarrierDisposed(
            object sender,
            Agileo.Semi.Gem300.Abstractions.E87.CarrierEventArgs e)
        {
            GenerateLotEventLog(
                e.Carrier.LocationId,
                EventLogConstant.LotEventLog.CarrierUnLoad,
                "$",
                "$",
                "$",
                e.Carrier.ObjID);
        }

        private void E40Std_ProcessJobChanged(object sender, ProcessJobStateChangedEventArgs e)
        {
            if (!(e.NewState == JobState.PROCESSCOMPLETE || e.NewState == JobState.STOPPED)
                && !(e.NewState == JobState.PROCESSING && e.PreviousState != JobState.PAUSED))
            {
                return;
            }

            var eventId = e.NewState == JobState.PROCESSING && e.PreviousState != JobState.PAUSED
                ? EventLogConstant.LotEventLog.ProcessJobStart
                : EventLogConstant.LotEventLog.ProcessJobEnd;

            if (e.ProcessJob.MaterialType != MaterialType.Carriers)
            {
                return;
            }

            var carrierId = e.ProcessJob.CarrierIDSlotsAssociation.First().CarrierID;
            var carrier = GemController.E87Std.GetCarrierById(carrierId);
            if (carrier == null)
            {
                return;
            }

            var lp = _equipment.AllOfType<LoadPort>()
                .FirstOrDefault(l => l.InstanceId == carrier.PortID);
            if (lp == null)
            {
                return;
            }

            var substrate = GemController.E90Std.GetCarrierSubstrates(carrierId).First();
            if (substrate == null)
            {
                return;
            }

            GenerateLotEventLog(
                lp.Name,
                eventId,
                substrate.LotID,
                e.ProcessJob.RecipeID,
                $"[4,'{lp.Name}','Aligner','PM1','{lp.Name}']",
                carrierId);
        }

        #endregion

        #region Private

        #region Controller

        private void CompareControllerConfiguration(
            string deviceId,
            ControllerConfiguration oldConfig,
            ControllerConfiguration newConfig)
        {
            if (!newConfig.CarrierPickOrder.Equals(oldConfig.CarrierPickOrder))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(ControllerConfiguration),
                    nameof(newConfig.CarrierPickOrder),
                    newConfig.CarrierPickOrder.ToString());
            }

            if (!newConfig.InitRequiredAtStartup.Equals(oldConfig.InitRequiredAtStartup))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(ControllerConfiguration),
                    nameof(newConfig.InitRequiredAtStartup),
                    newConfig.InitRequiredAtStartup.ToString());
            }

            if (!newConfig.UseWarmInit.Equals(oldConfig.UseWarmInit))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(ControllerConfiguration),
                    nameof(newConfig.UseWarmInit),
                    newConfig.UseWarmInit.ToString());
            }

            if (!newConfig.DataFlowFolderName.Equals(oldConfig.DataFlowFolderName))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(ControllerConfiguration),
                    nameof(newConfig.DataFlowFolderName),
                    newConfig.DataFlowFolderName);
            }

            if (!newConfig.ToolKey.Equals(oldConfig.ToolKey))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(ControllerConfiguration),
                    nameof(newConfig.ToolKey),
                    newConfig.ToolKey.ToString());
            }

            if (!newConfig.UnloadCarrierAfterAbort.Equals(oldConfig.UnloadCarrierAfterAbort))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(ControllerConfiguration),
                    nameof(newConfig.UnloadCarrierAfterAbort),
                    newConfig.UnloadCarrierAfterAbort.ToString());
            }

            if (!newConfig.DisableParallelControlJob.Equals(oldConfig.DisableParallelControlJob))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(ControllerConfiguration),
                    nameof(newConfig.DisableParallelControlJob),
                    newConfig.DisableParallelControlJob.ToString());
            }

            CompareRecipeConfiguration(
                deviceId,
                $"{nameof(ControllerConfiguration)}/{nameof(RecipeConfiguration)}",
                oldConfig.RecipeConfiguration,
                newConfig.RecipeConfiguration);
            CompareUserInterfaceConfiguration(
                deviceId,
                $"{nameof(ControllerConfiguration)}/{nameof(UserInterfaceConfiguration)}",
                oldConfig.UserInterfaceConfiguration,
                newConfig.UserInterfaceConfiguration);
            CompareApplicationPathConfiguration(
                deviceId,
                $"{nameof(ControllerConfiguration)}/{nameof(ApplicationPath)}",
                oldConfig.ApplicationPath,
                newConfig.ApplicationPath);
            CompareEquipmentIdentityConfiguration(
                deviceId,
                $"{nameof(ControllerConfiguration)}/{nameof(EquipmentIdentityConfig)}",
                oldConfig.EquipmentIdentityConfig,
                newConfig.EquipmentIdentityConfig);
            CompareAlarmsConfiguration(
                deviceId,
                $"{nameof(ControllerConfiguration)}/{nameof(AlarmCenterConfiguration)}",
                oldConfig.Alarms,
                newConfig.Alarms);
            CompareEquipmentConfiguration(
                deviceId,
                $"{nameof(ControllerConfiguration)}/{nameof(EquipmentConfiguration)}",
                oldConfig.EquipmentConfig,
                newConfig.EquipmentConfig);
            CompareScenarioConfiguration(
                deviceId,
                $"{nameof(ControllerConfiguration)}/{nameof(ScenarioConfiguration)}",
                oldConfig.ScenarioConfiguration,
                newConfig.ScenarioConfiguration);
            ComparePathConfiguration(
                deviceId,
                $"{nameof(ControllerConfiguration)}/{nameof(Path)}",
                oldConfig.Path,
                newConfig.Path);
            CompareDiagnostics(
                deviceId,
                $"{nameof(ControllerConfiguration)}/{nameof(Diagnostics)}",
                oldConfig.Diagnostics,
                newConfig.Diagnostics);
            CompareGlobalSettings(
                deviceId,
                $"{nameof(ControllerConfiguration)}/{nameof(GlobalSettings)}",
                oldConfig.GlobalSettings,
                newConfig.GlobalSettings);
        }

        private void CompareGlobalSettings(
            string deviceId,
            string category,
            GlobalSettings oldConfig,
            GlobalSettings newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.IsColorizationToolboxVisible.Equals(
                    oldConfig.IsColorizationToolboxVisible))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IsColorizationToolboxVisible),
                    newConfig.IsColorizationToolboxVisible.ToString());
            }

            if (!newConfig.IsDeveloperDebugModeEnabled.Equals(
                    oldConfig.IsDeveloperDebugModeEnabled))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IsDeveloperDebugModeEnabled),
                    newConfig.IsDeveloperDebugModeEnabled.ToString());
            }

            if (!newConfig.IsShutDownConfirmationRequired.Equals(
                    oldConfig.IsShutDownConfirmationRequired))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IsShutDownConfirmationRequired),
                    newConfig.IsShutDownConfirmationRequired.ToString());
            }
        }

        private void CompareDiagnostics(
            string deviceId,
            string category,
            Diagnostics oldConfig,
            Diagnostics newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.DataLogMaxItemCount.Equals(oldConfig.DataLogMaxItemCount))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.DataLogMaxItemCount),
                    newConfig.DataLogMaxItemCount.ToString());
            }

            if (!newConfig.ExportPath.Equals(oldConfig.ExportPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.ExportPath),
                    newConfig.ExportPath);
            }

            CompareDataLogFiltersConfig(
                deviceId,
                $"{category}/{nameof(DataLogFiltersConfig)}",
                oldConfig.DataLogFilters,
                newConfig.DataLogFilters);
            CompareTracingConfig(
                deviceId,
                $"{category}/{nameof(TracingConfig)}",
                oldConfig.TracingConfig,
                newConfig.TracingConfig);
        }

        private void CompareTracingConfig(
            string deviceId,
            string category,
            TracingConfig oldConfig,
            TracingConfig newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            CompareRestrictionsConfig(
                deviceId,
                $"{category}/{nameof(TracingConfig.RestrictionsConfig)}",
                oldConfig.Restrictions,
                newConfig.Restrictions);
            CompareFilePaths(
                deviceId,
                $"{category}/{nameof(TracingConfig.FilePathsConfig)}",
                oldConfig.FilePaths,
                newConfig.FilePaths);
            CompareTraceMonitoring(
                deviceId,
                $"{category}/{nameof(TracingConfig.TraceMonitoringConfig)}",
                oldConfig.TraceMonitoring,
                newConfig.TraceMonitoring);
        }

        private void CompareTraceMonitoring(
            string deviceId,
            string category,
            TracingConfig.TraceMonitoringConfig oldConfig,
            TracingConfig.TraceMonitoringConfig newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.IsDebugLevelOn.Equals(oldConfig.IsDebugLevelOn))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IsDebugLevelOn),
                    newConfig.IsDebugLevelOn.ToString());
            }

            if (!newConfig.IsSpecializedLogOn.Equals(oldConfig.IsSpecializedLogOn))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IsSpecializedLogOn),
                    newConfig.IsSpecializedLogOn.ToString());
            }

            if (!newConfig.IsTraceEventsOn.Equals(oldConfig.IsTraceEventsOn))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IsTraceEventsOn),
                    newConfig.IsTraceEventsOn.ToString());
            }

            if (!newConfig.IsTraceToFileOn.Equals(oldConfig.IsTraceToFileOn))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IsTraceToFileOn),
                    newConfig.IsTraceToFileOn.ToString());
            }
        }

        private void CompareFilePaths(
            string deviceId,
            string category,
            TracingConfig.FilePathsConfig oldConfig,
            TracingConfig.FilePathsConfig newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.ArchivePath.Equals(oldConfig.ArchivePath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.ArchivePath),
                    newConfig.ArchivePath);
            }

            if (!newConfig.TracesPath.Equals(oldConfig.TracesPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.TracesPath),
                    newConfig.TracesPath);
            }
        }

        private void CompareRestrictionsConfig(
            string deviceId,
            string category,
            TracingConfig.RestrictionsConfig oldConfig,
            TracingConfig.RestrictionsConfig newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.IsArchivingOn.Equals(oldConfig.IsArchivingOn))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IsArchivingOn),
                    newConfig.IsArchivingOn.ToString());
            }

            if (!newConfig.ArchiveFileMaxSize.Equals(oldConfig.ArchiveFileMaxSize))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.ArchiveFileMaxSize),
                    newConfig.ArchiveFileMaxSize.ToString(),
                    "KBytes");
            }

            if (!newConfig.NumberOfArchiveFiles.Equals(oldConfig.NumberOfArchiveFiles))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.NumberOfArchiveFiles),
                    newConfig.NumberOfArchiveFiles.ToString());
            }

            if (!newConfig.NumberOfTraceFiles.Equals(oldConfig.NumberOfTraceFiles))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.NumberOfTraceFiles),
                    newConfig.NumberOfTraceFiles.ToString());
            }

            if (!newConfig.TraceFileMaxSize.Equals(oldConfig.TraceFileMaxSize))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.TraceFileMaxSize),
                    newConfig.TraceFileMaxSize.ToString(),
                    "KBytes");
            }

            if (!newConfig.TraceLineMaxLength.Equals(oldConfig.TraceLineMaxLength))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.TraceLineMaxLength),
                    newConfig.TraceLineMaxLength.ToString());
            }
        }

        private void CompareDataLogFiltersConfig(
            string deviceId,
            string category,
            DataLogFiltersConfig oldConfig,
            DataLogFiltersConfig newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.BufferCapacity.Equals(oldConfig.BufferCapacity))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.BufferCapacity),
                    newConfig.BufferCapacity.ToString());
            }
        }

        private void ComparePathConfiguration(
            string deviceId,
            string category,
            Path oldConfig,
            Path newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.AccessRightsConfigurationPath.Equals(
                    oldConfig.AccessRightsConfigurationPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.AccessRightsConfigurationPath),
                    newConfig.AccessRightsConfigurationPath);
            }

            if (!newConfig.AccessRightsSchemaPath.Equals(oldConfig.AccessRightsSchemaPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.AccessRightsSchemaPath),
                    newConfig.AccessRightsSchemaPath);
            }

            if (!newConfig.LocalizationPath.Equals(oldConfig.LocalizationPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.LocalizationPath),
                    newConfig.LocalizationPath);
            }
        }

        private void CompareScenarioConfiguration(
            string deviceId,
            string category,
            ScenarioConfiguration oldConfig,
            ScenarioConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.FileExtension.Equals(oldConfig.FileExtension))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.FileExtension),
                    newConfig.FileExtension);
            }

            if (!newConfig.Path.Equals(oldConfig.Path))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Path),
                    newConfig.Path);
            }

            if (!newConfig.StorageFormat.Equals(oldConfig.StorageFormat))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.StorageFormat),
                    newConfig.StorageFormat.ToString());
            }
        }

        private void CompareEquipmentConfiguration(
            string deviceId,
            string category,
            EquipmentConfiguration oldConfig,
            EquipmentConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.IsSocketLogEnabled.Equals(oldConfig.IsSocketLogEnabled))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IsSocketLogEnabled),
                    newConfig.IsSocketLogEnabled.ToString());
            }

            if (!newConfig.DeviceConfigFolderPath.Equals(oldConfig.DeviceConfigFolderPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.DeviceConfigFolderPath),
                    newConfig.DeviceConfigFolderPath);
            }

            if (!newConfig.EquipmentsFolderPath.Equals(oldConfig.EquipmentsFolderPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.EquipmentsFolderPath),
                    newConfig.EquipmentsFolderPath);
            }

            if (!newConfig.EquipmentFileName.Equals(oldConfig.EquipmentFileName))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.EquipmentFileName),
                    newConfig.EquipmentFileName);
            }
        }

        private void CompareAlarmsConfiguration(
            string deviceId,
            string category,
            AlarmCenterConfiguration oldConfig,
            AlarmCenterConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.DisableAlarmsTextLocalization.Equals(
                    oldConfig.DisableAlarmsTextLocalization))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.DisableAlarmsTextLocalization),
                    newConfig.DisableAlarmsTextLocalization.ToString());
            }

            if (!newConfig.StepProviderClassId.Equals(oldConfig.StepProviderClassId))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.StepProviderClassId),
                    newConfig.StepProviderClassId.ToString());
            }

            if (!newConfig.StepProviderInstance.Equals(oldConfig.StepProviderInstance))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.StepProviderInstance),
                    newConfig.StepProviderInstance.ToString());
            }

            CompareStorageConfiguration(
                deviceId,
                $"{category}/{nameof(AlarmStorageConfiguration)}",
                oldConfig.Storage,
                newConfig.Storage);
        }

        private void CompareStorageConfiguration(
            string deviceId,
            string category,
            AlarmStorageConfiguration oldConfig,
            AlarmStorageConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.DbFullPath.Equals(oldConfig.DbFullPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.DbFullPath),
                    newConfig.DbFullPath);
            }
        }

        private void CompareEquipmentIdentityConfiguration(
            string deviceId,
            string category,
            EquipmentIdentityConfig oldConfig,
            EquipmentIdentityConfig newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.MDLN.Equals(oldConfig.MDLN))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.MDLN),
                    newConfig.MDLN);
            }

            if (!newConfig.SOFTREV.Equals(oldConfig.SOFTREV))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.SOFTREV),
                    newConfig.SOFTREV);
            }
        }

        private void CompareRecipeConfiguration(
            string deviceId,
            string category,
            RecipeConfiguration oldConfig,
            RecipeConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.FileExtension.Equals(oldConfig.FileExtension))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.FileExtension),
                    newConfig.FileExtension);
            }

            if (!newConfig.Path.Equals(oldConfig.Path))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Path),
                    newConfig.Path);
            }

            if (!newConfig.StorageFormat.Equals(oldConfig.StorageFormat))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.StorageFormat),
                    newConfig.StorageFormat.ToString());
            }
        }

        private void CompareUserInterfaceConfiguration(
            string deviceId,
            string category,
            UserInterfaceConfiguration oldConfig,
            UserInterfaceConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.Theme.Equals(oldConfig.Theme))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Theme),
                    newConfig.Theme);
            }

            if (!newConfig.ThemeFolder.Equals(oldConfig.ThemeFolder))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.ThemeFolder),
                    newConfig.ThemeFolder);
            }

            if (!newConfig.FontScale.Equals(oldConfig.FontScale))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.FontScale),
                    newConfig.FontScale.ToString("P"));
            }

            if (!newConfig.GlobalScale.Equals(oldConfig.GlobalScale))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.GlobalScale),
                    newConfig.GlobalScale.ToString("F"));
            }
        }

        private void CompareApplicationPathConfiguration(
            string deviceId,
            string category,
            ApplicationPath oldConfig,
            ApplicationPath newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.UserManualPath.Equals(oldConfig.UserManualPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.UserManualPath),
                    newConfig.UserManualPath);
            }

            if (!newConfig.AlarmAnalysisCaptureStoragePath.Equals(
                    oldConfig.AlarmAnalysisCaptureStoragePath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.AlarmAnalysisCaptureStoragePath),
                    newConfig.AlarmAnalysisCaptureStoragePath);
            }

            if (!newConfig.DataMonitoringPath.Equals(oldConfig.DataMonitoringPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.DataMonitoringPath),
                    newConfig.DataMonitoringPath);
            }

            if (!newConfig.AutomationLogPath.Equals(oldConfig.AutomationLogPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.AutomationLogPath),
                    newConfig.AutomationLogPath);
            }

            if (!newConfig.AutomationConfigPath.Equals(oldConfig.AutomationConfigPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.AutomationConfigPath),
                    newConfig.AutomationConfigPath);
            }

            if (!newConfig.AutomationVariablesPath.Equals(oldConfig.AutomationVariablesPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.AutomationVariablesPath),
                    newConfig.AutomationVariablesPath);
            }

            if (!newConfig.DcpStoragePath.Equals(oldConfig.DcpStoragePath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.DcpStoragePath),
                    newConfig.DcpStoragePath);
            }

            if (!newConfig.DfClientConfigurationFolderPath.Equals(
                    oldConfig.DfClientConfigurationFolderPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.DfClientConfigurationFolderPath),
                    newConfig.DfClientConfigurationFolderPath);
            }
        }

        #endregion

        #region Compare Io module

        private void CompareIoModuleConfiguration(
            string deviceId,
            GenericRC5xxConfiguration oldConfig,
            GenericRC5xxConfiguration newConfig)
        {
            if (!newConfig.InitializationTimeout.Equals(oldConfig.InitializationTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(GenericRC5xxConfiguration),
                    nameof(newConfig.InitializationTimeout),
                    newConfig.InitializationTimeout.ToString(),
                    "s");
            }

            CompareCommunicationConfiguration(
                deviceId,
                $"{nameof(GenericRC5xxConfiguration)}/{nameof(CommunicationConfiguration)}",
                oldConfig.CommunicationConfig,
                newConfig.CommunicationConfig);
        }

        #endregion

        #region Compare Substrate Id Reader

        private void CompareReaderConfiguration(
            string deviceId,
            PC1740Configuration oldConfig,
            PC1740Configuration newConfig)
        {
            if (!newConfig.InitializationTimeout.Equals(oldConfig.InitializationTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(PC1740Configuration),
                    nameof(newConfig.InitializationTimeout),
                    newConfig.InitializationTimeout.ToString(),
                    "s");
            }

            if (!newConfig.ImagePath.Equals(oldConfig.ImagePath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(PC1740Configuration),
                    nameof(newConfig.ImagePath),
                    newConfig.ImagePath);
            }

            if (!newConfig.RecipeFolderPath.Equals(oldConfig.RecipeFolderPath))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(PC1740Configuration),
                    nameof(newConfig.RecipeFolderPath),
                    newConfig.RecipeFolderPath);
            }

            if (!newConfig.UseOnlyOneT7.Equals(oldConfig.UseOnlyOneT7))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(PC1740Configuration),
                    nameof(newConfig.UseOnlyOneT7),
                    newConfig.UseOnlyOneT7.ToString());
            }

            CompareCommunicationConfiguration(
                deviceId,
                $"{nameof(PC1740Configuration)}/{nameof(CommunicationConfiguration)}",
                oldConfig.CommunicationConfig,
                newConfig.CommunicationConfig);
            CompareT7Configuration(
                deviceId,
                $"{nameof(PC1740Configuration)}/{nameof(newConfig.T7Recipe)}",
                oldConfig.T7Recipe,
                newConfig.T7Recipe);
        }

        #endregion

        #region Compare DataFlowManager

        private void CompareDfConfiguration(
            string deviceId,
            DataFlowManagerConfiguration oldConfig,
            DataFlowManagerConfiguration newConfig)
        {
            if (!newConfig.InitializationTimeout.Equals(oldConfig.InitializationTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(DataFlowManagerConfiguration),
                    nameof(newConfig.InitializationTimeout),
                    newConfig.InitializationTimeout.ToString(),
                    "s");
            }

            CompareWcfConfiguration(
                deviceId,
                $"{nameof(DataFlowManagerConfiguration)}/{nameof(WcfConfiguration)}",
                oldConfig.WcfConfiguration,
                newConfig.WcfConfiguration);
        }

        #endregion

        #region Compare Process module

        private void ComparePmConfiguration(
            string deviceId,
            UnityProcessModuleConfiguration oldConfig,
            UnityProcessModuleConfiguration newConfig)
        {
            if (!newConfig.InitializationTimeout.Equals(oldConfig.InitializationTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(UnityProcessModuleConfiguration),
                    nameof(newConfig.InitializationTimeout),
                    newConfig.InitializationTimeout.ToString(),
                    "s");
            }

            CompareWcfConfiguration(
                deviceId,
                $"{nameof(UnityProcessModuleConfiguration)}/{nameof(WcfConfiguration)}",
                oldConfig.WcfConfiguration,
                newConfig.WcfConfiguration);
        }

        #endregion

        #region Compare Ffu

        private void CompareFfuConfiguration(
            string deviceId,
            FfuConfiguration oldConfig,
            FfuConfiguration newConfig)
        {
            if (!newConfig.InitializationTimeout.Equals(oldConfig.InitializationTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(FfuConfiguration),
                    nameof(newConfig.InitializationTimeout),
                    newConfig.InitializationTimeout.ToString(),
                    "s");
            }

            if (!newConfig.LowSpeedThreshold.Equals(oldConfig.LowSpeedThreshold))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(FfuConfiguration),
                    nameof(newConfig.LowSpeedThreshold),
                    newConfig.LowSpeedThreshold.ToString("F"),
                    "RPM");
            }

            if (!newConfig.LowPressureThreshold.Equals(oldConfig.LowPressureThreshold))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(FfuConfiguration),
                    nameof(newConfig.LowPressureThreshold),
                    newConfig.LowPressureThreshold.ToString("F"),
                    "Pa");
            }
        }

        #endregion

        #region Compare LoadPort

        private void CompareLoadPortConfiguration(
            string deviceId,
            RorzeLoadPortConfiguration oldConfig,
            RorzeLoadPortConfiguration newConfig)
        {
            if (!newConfig.InitializationTimeout.Equals(oldConfig.InitializationTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.InitializationTimeout),
                    newConfig.InitializationTimeout.ToString(),
                    "s");
            }

            if (!newConfig.CassetteType.Equals(oldConfig.CassetteType))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.CassetteType),
                    newConfig.CassetteType.ToString());
            }

            if (!newConfig.IsE84Enabled.Equals(oldConfig.IsE84Enabled))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.IsE84Enabled),
                    newConfig.IsE84Enabled.ToString());
            }

            if (!newConfig.HandOffType.Equals(oldConfig.HandOffType))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.HandOffType),
                    newConfig.HandOffType.ToString());
            }

            if (!newConfig.AutoHandOffTimeout.Equals(oldConfig.AutoHandOffTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.AutoHandOffTimeout),
                    newConfig.AutoHandOffTimeout.ToString(),
                    "s");
            }

            if (!newConfig.IsMappingSupported.Equals(oldConfig.IsMappingSupported))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.IsMappingSupported),
                    newConfig.IsMappingSupported.ToString());
            }

            if (!newConfig.IsInService.Equals(oldConfig.IsInService))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.IsInService),
                    newConfig.IsInService.ToString());
            }

            if (!newConfig.IsCarrierIdSupported.Equals(oldConfig.IsCarrierIdSupported))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.IsCarrierIdSupported),
                    newConfig.IsCarrierIdSupported.ToString());
            }

            if (!newConfig.IsManualCarrierType.Equals(oldConfig.IsManualCarrierType))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.IsManualCarrierType),
                    newConfig.IsManualCarrierType.ToString());
            }

            if (!newConfig.UseDefaultPageIntervalForReading.Equals(
                    oldConfig.UseDefaultPageIntervalForReading))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.UseDefaultPageIntervalForReading),
                    newConfig.UseDefaultPageIntervalForReading.ToString());
            }

            if (!newConfig.CarrierIdStartPage.Equals(oldConfig.CarrierIdStartPage))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.CarrierIdStartPage),
                    newConfig.CarrierIdStartPage.ToString());
            }

            if (!newConfig.CarrierIdStopPage.Equals(oldConfig.CarrierIdStopPage))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.CarrierIdStopPage),
                    newConfig.CarrierIdStopPage.ToString());
            }

            if (!newConfig.CarrierIdentificationConfig.CarrierIdStartIndex.Equals(
                    oldConfig.CarrierIdentificationConfig.CarrierIdStartIndex))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.CarrierIdentificationConfig.CarrierIdStartIndex),
                    newConfig.CarrierIdentificationConfig.CarrierIdStartIndex.ToString());
            }

            if (!newConfig.CarrierIdentificationConfig.CarrierIdStopIndex.Equals(
                    oldConfig.CarrierIdentificationConfig.CarrierIdStopIndex))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RorzeLoadPortConfiguration),
                    nameof(newConfig.CarrierIdentificationConfig.CarrierIdStopIndex),
                    newConfig.CarrierIdentificationConfig.CarrierIdStopIndex.ToString());
            }

            CompareCommunicationConfiguration(
                deviceId,
                $"{nameof(RorzeLoadPortConfiguration)}/{nameof(CommunicationConfiguration)}",
                oldConfig.CommunicationConfig,
                newConfig.CommunicationConfig);
            CompareCarrierIdentificationConfiguration(
                deviceId,
                $"{nameof(RorzeLoadPortConfiguration)}/{nameof(CarrierIdentificationConfiguration)}",
                oldConfig.CarrierIdentificationConfig,
                newConfig.CarrierIdentificationConfig);
            CompareE84Configuration(
                deviceId,
                $"{nameof(RorzeLoadPortConfiguration)}/{nameof(E84Configuration)}",
                oldConfig.E84Configuration,
                newConfig.E84Configuration);
        }

        #endregion

        #region Compare Aligner

        private void CompareAlignerConfiguration(
            string deviceId,
            RA420Configuration oldConfig,
            RA420Configuration newConfig)
        {
            if (!newConfig.InitializationTimeout.Equals(oldConfig.InitializationTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RA420Configuration),
                    nameof(newConfig.InitializationTimeout),
                    newConfig.InitializationTimeout.ToString(),
                    "s");
            }

            if (!newConfig.AlignOffset.Equals(oldConfig.AlignOffset))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RA420Configuration),
                    nameof(newConfig.AlignOffset),
                    newConfig.AlignOffset.ToString("F"),
                    "°");
            }

            CompareCommunicationConfiguration(
                deviceId,
                $"{nameof(RA420Configuration)}/{nameof(CommunicationConfiguration)}",
                oldConfig.CommunicationConfig,
                newConfig.CommunicationConfig);
        }

        #endregion

        #region Compare Robot

        private void CompareRobotConfiguration(
            string deviceId,
            RR75xConfiguration oldConfig,
            RR75xConfiguration newConfig)
        {
            CompareCommunicationConfiguration(
                deviceId,
                $"{nameof(RR75xConfiguration)}/{nameof(CommunicationConfiguration)}",
                oldConfig.CommunicationConfig,
                newConfig.CommunicationConfig);

            CompareArmConfiguration(
                deviceId,
                $"{nameof(RR75xConfiguration)}/{nameof(newConfig.UpperArm)}",
                oldConfig.UpperArm,
                newConfig.UpperArm);

            CompareArmConfiguration(
                deviceId,
                $"{nameof(RR75xConfiguration)}/{nameof(newConfig.LowerArm)}",
                oldConfig.LowerArm,
                newConfig.LowerArm);

            if (!newConfig.InitializationTimeout.Equals(oldConfig.InitializationTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    nameof(RR75xConfiguration),
                    nameof(newConfig.InitializationTimeout),
                    newConfig.InitializationTimeout.ToString(),
                    "s");
            }
        }

        #endregion

        #region Compare generic

        private void CompareE84Configuration(
            string deviceId,
            string category,
            E84Configuration oldConfig,
            E84Configuration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.Tp1.Equals(oldConfig.Tp1))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Tp1),
                    newConfig.Tp1.ToString(),
                    "s");
            }

            if (!newConfig.Tp2.Equals(oldConfig.Tp2))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Tp2),
                    newConfig.Tp2.ToString(),
                    "s");
            }

            if (!newConfig.Tp3.Equals(oldConfig.Tp3))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Tp3),
                    newConfig.Tp3.ToString(),
                    "s");
            }

            if (!newConfig.Tp4.Equals(oldConfig.Tp4))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Tp4),
                    newConfig.Tp4.ToString(),
                    "s");
            }

            if (!newConfig.Tp5.Equals(oldConfig.Tp5))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Tp5),
                    newConfig.Tp5.ToString(),
                    "s");
            }
        }

        private void CompareCommunicationConfiguration(
            string deviceId,
            string category,
            CommunicationConfiguration oldConfig,
            CommunicationConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.ConnectionMode.Equals(oldConfig.ConnectionMode))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.ConnectionMode),
                    newConfig.ConnectionMode.ToString());
            }

            if (!newConfig.IpAddress.Equals(oldConfig.IpAddress))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IpAddress),
                    newConfig.IpAddressAsString);
            }

            if (!newConfig.TcpPort.Equals(oldConfig.TcpPort))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.TcpPort),
                    newConfig.TcpPort.ToString());
            }

            if (!newConfig.AnswerTimeout.Equals(oldConfig.AnswerTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.AnswerTimeout),
                    newConfig.AnswerTimeout.ToString(),
                    "ms");
            }

            if (!newConfig.ConfirmationTimeout.Equals(oldConfig.ConfirmationTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.ConfirmationTimeout),
                    newConfig.ConfirmationTimeout.ToString(),
                    "ms");
            }

            if (!newConfig.InitTimeout.Equals(oldConfig.InitTimeout))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.InitTimeout),
                    newConfig.InitTimeout.ToString(),
                    "ms");
            }

            if (!newConfig.CommunicatorId.Equals(oldConfig.CommunicatorId))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.CommunicatorId),
                    newConfig.CommunicatorId.ToString());
            }

            if (!newConfig.MaxNbRetry.Equals(oldConfig.MaxNbRetry))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.MaxNbRetry),
                    newConfig.MaxNbRetry.ToString());
            }

            if (!newConfig.ConnectionRetryDelay.Equals(oldConfig.ConnectionRetryDelay))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.ConnectionRetryDelay),
                    newConfig.ConnectionRetryDelay.ToString(),
                    "ms");
            }
        }

        private void CompareWcfConfiguration(
            string deviceId,
            string category,
            WcfConfiguration oldConfig,
            WcfConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.WcfHostIpAddress.Equals(oldConfig.WcfHostIpAddress))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.WcfHostIpAddress),
                    newConfig.WcfHostIpAddressAsString);
            }

            if (!newConfig.WcfHostPort.Equals(oldConfig.WcfHostPort))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.WcfHostPort),
                    newConfig.WcfHostPort.ToString());
            }

            if (!newConfig.WcfServiceUriSegment.Equals(oldConfig.WcfServiceUriSegment))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.WcfServiceUriSegment),
                    newConfig.WcfServiceUriSegment);
            }
        }

        private void CompareT7Configuration(
            string deviceId,
            string category,
            T7RecipeConfiguration oldConfig,
            T7RecipeConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.Angle.Equals(oldConfig.Angle))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Angle),
                    newConfig.Angle.ToString("F"),
                    "°");
            }

            if (!newConfig.Angle8.Equals(oldConfig.Angle8))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Angle8),
                    newConfig.Angle8.ToString("F"),
                    "°");
            }

            if (!newConfig.Date.Equals(oldConfig.Date))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Date),
                    newConfig.Date.ToLongDateString());
            }

            if (!newConfig.Name.Equals(oldConfig.Name))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.Name),
                    newConfig.Name);
            }
        }

        private void CompareArmConfiguration(
            string deviceId,
            string category,
            ArmConfiguration oldConfig,
            ArmConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.IsEnabled.Equals(oldConfig.IsEnabled))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.IsEnabled),
                    newConfig.IsEnabled.ToString());
            }

            if (!newConfig.EffectorType.Equals(oldConfig.EffectorType))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.EffectorType),
                    newConfig.EffectorType.ToString());
            }
        }

        private void CompareCarrierIdentificationConfiguration(
            string deviceId,
            string category,
            CarrierIdentificationConfiguration oldConfig,
            CarrierIdentificationConfiguration newConfig)
        {
            if (oldConfig == null || newConfig == null)
            {
                return;
            }

            if (!newConfig.ByPassReadId.Equals(oldConfig.ByPassReadId))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.ByPassReadId),
                    newConfig.ByPassReadId.ToString());
            }

            if (!newConfig.CarrierIdAcquisition.Equals(oldConfig.CarrierIdAcquisition))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.CarrierIdAcquisition),
                    newConfig.CarrierIdAcquisition.ToString());
            }

            if (!newConfig.CarrierTagLocation.Equals(oldConfig.CarrierTagLocation))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.CarrierTagLocation),
                    newConfig.CarrierTagLocation.ToString());
            }

            if (!newConfig.DefaultCarrierId.Equals(oldConfig.DefaultCarrierId))
            {
                GenerateConfigurationLog(
                    deviceId,
                    category,
                    nameof(newConfig.DefaultCarrierId),
                    newConfig.DefaultCarrierId);
            }
        }

        #endregion

        #region Generate log

        private void GenerateTransferLog(
            string eventId,
            string status,
            string materialId,
            string lotId,
            string fromDevice,
            string fromSlot,
            string toDevice,
            string toSlot)
        {
            _logger.Info(
                $"{DateTime.Now:yyyy-MM-dd}\t{DateTime.Now:hh:mm:ss.fff}\t{_robot.Name}\t{EventLogConstant.TransferLog.LogType}\t{eventId}\t{status}\t{materialId}\tWafer\t{lotId}\t{fromDevice}\t{fromSlot}\t{toDevice}\t{toSlot}\t$");
        }

        private void GenerateLotEventLog(
            string deviceId,
            string eventId,
            string lotId,
            string flowRecipeId,
            string flowInfo,
            string carrierId)
        {
            _logger.Info(
                $"{DateTime.Now:yyyy-MM-dd}\t{DateTime.Now:hh:mm:ss.fff}\t{deviceId}\t{EventLogConstant.LotEventLog.LogType}\t{eventId}\t{lotId}\t{flowRecipeId}\t{flowInfo}\t{carrierId}\t$");
        }

        private void GenerateFunctionLog(
            string deviceId,
            string eventId,
            string status,
            string materialId,
            string materialType = "Wafer")
        {
            _logger.Info(
                $"{DateTime.Now:yyyy-MM-dd}\t{DateTime.Now:hh:mm:ss.fff}\t{deviceId}\t{EventLogConstant.FunctionLog.LogType}\t{eventId}\t{status}\t{materialId}\t{materialType}\t$");
        }

        private void GenerateConfigurationLog(
            string deviceId,
            string category,
            string cfgId,
            string value,
            string unit = "$",
            string ecId = "$")
        {
            _logger.Info(
                $"{DateTime.Now:yyyy-MM-dd}\t{DateTime.Now:hh:mm:ss.fff}\t{deviceId}\t{EventLogConstant.ConfigurationLog.LogType}\t{category}\t{cfgId}\t{value}\t{unit}\t{ecId}\t$");
        }

        #endregion

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _robot.PropertyChanged -= Robot_PropertyChanged;
                    _robot.CommandExecutionStateChanged -= Robot_CommandExecutionStateChanged;
                    _aligner.CommandExecutionStateChanged -= Aligner_CommandExecutionStateChanged;

                    foreach (var loadPort in _equipment.AllOfType<LoadPort>())
                    {
                        loadPort.CommandExecutionStateChanged -=
                            LoadPort_CommandExecutionStateChanged;
                    }

                    GemController.E30Std.EquipmentConstantsServices.EquipmentConstantChanged -=
                        EquipmentConstantsServices_EquipmentConstantChanged;
                    GemController.E87Std.CarrierIdStateChanged -= E87Std_CarrierIdStateChanged;
                    GemController.E87Std.CarrierDisposed -= E87Std_CarrierDisposed;
                    GemController.E40Std.ProcessJobChanged -= E40Std_ProcessJobChanged;

                    App.ControllerInstance.ConfigurationManager.CurrentChanged -=
                        ConfigurationManager_CurrentChanged;

                    foreach (var device in _equipment.AllDevices())
                    {
                        if (device is IExtendedConfigurableDevice
                            {
                                ConfigManager: { }
                            } configurableDevice)
                        {
                            configurableDevice.ConfigManager.CurrentChanged -=
                                ConfigurationManager_CurrentChanged;
                        }
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
