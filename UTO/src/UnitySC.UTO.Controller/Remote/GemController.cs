using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

using Agileo.AlarmModeling;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.Semi.Abstractions;
using Agileo.Semi.Communication.Abstractions;
using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Communication.Abstractions.E5.MessageDescriptions;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300;
using Agileo.Semi.Gem300.Abstractions;
using Agileo.Semi.Gem300.Abstractions.E116;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E90;
using Agileo.Semi.Gem300.Abstractions.E94;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Devices.LightTower.Enums;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.JobDefinition;
using UnitySC.Equipment.Devices.Controller;
using UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum;
using UnitySC.Equipment.Devices.Controller.JobDefinition;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.Shared.Data.Enum;
using UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager;
using UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.Recipes;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.E5.MessageHandlers;
using UnitySC.UTO.Controller.Remote.Enums;
using UnitySC.UTO.Controller.Remote.Helpers;
using UnitySC.UTO.Controller.Remote.Observers;
using UnitySC.UTO.Controller.Remote.Services;

using static Agileo.Semi.Gem300.Abstractions.E87.Status;

using Alarm = Agileo.AlarmModeling.Alarm;
using E87Status = Agileo.Semi.Gem300.Abstractions.E87.Status;
using E40Status = Agileo.Semi.Gem300.Abstractions.E40.Status;
using E94Status = Agileo.Semi.Gem300.Abstractions.E94.Status;
using Error = Agileo.Semi.Gem300.Abstractions.E87.Error;
using ErrorCode = Agileo.Semi.Communication.Abstractions.E5.ErrorCode;
using LoadPort = Agileo.Semi.Gem300.Abstractions.E87.LoadPort;
using MaterialLocation = Agileo.Semi.Gem300.Abstractions.E90.MaterialLocation;
using MaterialType = Agileo.Semi.Gem300.Abstractions.E40.MaterialType;
using Status = Agileo.Semi.Gem300.Abstractions.E90.Status;

using UnitySC.Shared.Tools.Collection;
using UnitySC.UTO.Controller.Counters;

using SlotState = Agileo.Semi.Gem300.Abstractions.E87.SlotState;

namespace UnitySC.UTO.Controller.Remote
{
    public sealed class GemController
        : Notifier,
            IE30Callback,
            IDisposable,
            IAlarmProvider,
            IE87Callback,
            IE40Callback,
            IE94Callback,
            IE90Callback,
            IE116Callback,
            IUserInformationProvider
    {
        #region Fields

        private ILogger _logger;
        private IGem300Environment _gem300Environment;
        private bool _isControlledByHost;
        private Equipment.Devices.Controller.Controller _controller;
        private AbstractDataFlowManager _dataFlowManager;

        #endregion Fields

        #region Properties

        internal TerminalServices TerminalServices { get; private set; }

        internal ControlServices ControlServices { get; private set; }

        internal EquipmentObserver EquipmentServices { get; private set; }

        internal AlarmObserver AlarmObserver { get; private set; }

        internal MaterialMovementObserver MaterialMovementObserver { get; private set; }

        internal ProcessingSmObserver ProcessingSmObserver { get; private set; }

        internal StatusVariableUpdater StatusVariableUpdater { get; private set; }

        internal E40Observer E40Observer { get; private set; }

        internal E84Observer E84Observer { get; private set; }

        internal E87Observer E87Observer { get; private set; }

        internal E90Observer E90Observer { get; private set; }

        internal E94Observer E94Observer { get; private set; }

        internal E116Observer E116Observer { get; private set; }

        public IE30Standard E30Std => _gem300Environment.E30Standard;
        public IE87Standard E87Std => _gem300Environment.E87Standard;
        public IE40Standard E40Std => _gem300Environment.E40Standard;
        public IE90Standard E90Std => _gem300Environment.E90Standard;
        public IE94Standard E94Std => _gem300Environment.E94Standard;
        public IE116Standard E116Std => _gem300Environment.E116Standard;

        public bool IsControlledByHost
        {
            get
            {
                if (!IsSetupDone || _isDisposed)
                {
                    return false;
                }

                return _isControlledByHost;
            }
        }

        public bool IsSetupDone { get; private set; }

        #endregion Properties

        #region Public Methods

        public void OnSetup(
            ApplicationConfiguration configuration,
            Agileo.EquipmentModeling.Equipment equipment)
        {
            IsSetupDone = true;

            _logger = Logger.GetLogger("Automation");

            _controller =
                App.ControllerInstance.ControllerEquipmentManager.Controller as
                    UnitySC.Equipment.Devices.Controller.Controller;

            _dataFlowManager = _controller.TryGetDevice<AbstractDataFlowManager>();

            // Create and configure alarms
            var rcmdCommandFailedAlarm =
                GUI.Common.App.Instance.AlarmCenter.ModelBuilder.CreateAlarm(
                    AlNames.RemoteCommandFailed,
                    "An exception was thrown during remote command execution. See data logs fore more details. Initialize machine to clear alarm.",
                    140000,
                    AlarmSeverity.Indeterminate);
            Alarms.Add(rcmdCommandFailedAlarm);

            var tcCommunicationTimeout =
                GUI.Common.App.Instance.AlarmCenter.ModelBuilder.CreateAlarm(
                    AlNames.TcCommunicationTimeout,
                    "interruption detected during Tool Control communication between the equipment and the communicating server ",
                    140001,
                    AlarmSeverity.Indeterminate);
            Alarms.Add(tcCommunicationTimeout);

            var doubleSlotDetected = GUI.Common.App.Instance.AlarmCenter.ModelBuilder.CreateAlarm(
                AlNames.DoubleSlotDetected,
                "Alarm when the wafer is a double slot during slot mapping. ",
                140002,
                AlarmSeverity.Indeterminate);
            Alarms.Add(doubleSlotDetected);

            var carrierIdReadFail = GUI.Common.App.Instance.AlarmCenter.ModelBuilder.CreateAlarm(
                AlNames.CarrierIDReadFail,
                "The Carrier ID reading has failed.",
                140003,
                AlarmSeverity.Indeterminate);
            Alarms.Add(carrierIdReadFail);

            GUI.Common.App.Instance.AlarmCenter.ModelBuilder.AddAlarms(this);

            MessagesConfigurationManager currentmessageConfiguration = null;
            if (File.Exists(
                    Path.Combine(
                        configuration.ApplicationPath.AutomationConfigPath,
                        FileNames.MessagesConfiguration)))
            {
                //Load current MessagesConfiguration file
                currentmessageConfiguration = MessagesConfigurationManager.Load(
                    Path.Combine(
                        configuration.ApplicationPath.AutomationConfigPath,
                        FileNames.MessagesConfiguration));
            }

            //Generate new MessagesConfiguration file
            var newMessageConfiguration = MessagesConfigurationManager.Load(
                Path.Combine(
                    configuration.ApplicationPath.AutomationConfigPath,
                    FileNames.CommonMessagesConfiguration));

            //Append MessagesConfiguration for each process modules available on the tool
            foreach (var driveableProcessModule in App.ControllerInstance.ControllerEquipmentManager
                         .ProcessModules.Values)
            {
                newMessageConfiguration.AppendConfiguration(
                    driveableProcessModule.GetMessagesConfigurationPath(
                        App.ControllerInstance.Config.EquipmentConfig.DeviceConfigFolderPath),
                    driveableProcessModule.InstanceId,
                    driveableProcessModule.Name);
            }

            if (currentmessageConfiguration == null)
            {
                //Save the configuration to the final MessagesConfiguration file
                newMessageConfiguration.SaveSettings(
                    Path.Combine(
                        configuration.ApplicationPath.AutomationConfigPath,
                        FileNames.MessagesConfiguration));
            }
            else
            {
                var errors = newMessageConfiguration.CompareTo(currentmessageConfiguration);
                if (errors == string.Empty)
                {
                    errors = currentmessageConfiguration.CompareTo(newMessageConfiguration);
                }

                if (errors != string.Empty)
                {
                    //Display popup
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            var blockingWindow = new Window
                            {
                                Background = System.Windows.Media.Brushes.Transparent,
                                WindowStyle = WindowStyle.None,
                                ShowInTaskbar = true,
                                AllowsTransparency = true,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                Owner = App.UtoInstance.Windows.Cast<Window>().FirstOrDefault(),
                                Height = 400,
                                Width = 700
                            };

                            var message = new StringBuilder(errors);
                            message.AppendLine(
                                "When you use the new configuration, a backup of the old one is saved.");

                            var popup =
                                new Popup(
                                    Global.MESSAGESCONFIGURATIONS_INCOHERENCES,
                                    message.ToString())
                                {
                                    Commands =
                                    {
                                        new PopupCommand(
                                            Global.KEEP_ORIGINAL_FILE,
                                            new DelegateCommand(() => blockingWindow.Close())),
                                        new PopupCommand(
                                            Global.USE_NEW_CONFIGURATION,
                                            new DelegateCommand(
                                                () =>
                                                {
                                                    //Save the configuration to the final MessagesConfiguration file
                                                    var path = Path.Combine(
                                                        configuration.ApplicationPath
                                                            .AutomationConfigPath,
                                                        FileNames.MessagesConfiguration);

                                                    var pathBackup = Path.Combine(
                                                        configuration.ApplicationPath
                                                            .AutomationConfigPath,
                                                        FileNames.MessagesConfigurationBackup);

                                                    File.Copy(path, pathBackup, true);
                                                    File.Delete(path);
                                                    newMessageConfiguration.SaveSettings(path);
                                                    blockingWindow.Close();
                                                }))
                                    }
                                };

                            popup.OnShow();

                            blockingWindow.DataContext = popup;
                            blockingWindow.Content = popup;

                            blockingWindow.ShowDialog();

                            popup.Dispose();
                        });
                }
            }

            // Create and configure the Gem300Environment instance
            var gem300EnvironmentBuilder =
                new Gem300EnvironmentBuilder(TraceManager.Instance()).UseE30Standard(
                    E30Compliance.New(E30Revision.AgilGEM3007_2_x),
                    this,
                    configuration.ApplicationPath.AutomationConfigPath,
                    configuration.ApplicationPath.AutomationLogPath,
                    configuration.ApplicationPath.AutomationVariablesPath,
                    e30Standard =>
                    {
                        e30Standard.Configuration.DefaultOnlineSubstateSelection =
                            DefaultOnlineSubstateSelection.BySwitch;
                        e30Standard.Configuration.E30StartupConf.AutoRegisterRemoteLocalRcmd =
                            false;
                        e30Standard.Configuration.E30StartupConf.AutoRegisterE30DefaultRcmd = false;
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.LOCAL),
                            "Switch the Equipment to LOCAL mode.");

                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.PROCEEDWITHCARRIER),
                            "The ProceedWithCarrier RCMD is sent by the host to indicate that the carrier operations may continue.");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.CANCELCARRIER),
                            "The CancelCarrier command is used to stop a carrier. If the carrier is at a load port, then it shall be returned to load/unload location of the load port and make ready for unload");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.SETUPJOB),
                            "The Host creates new recipes based on an existing recipe by sending this command. AutosStart is true by default. Ocr will not be used if OCRProfileName is not specified.");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.START),
                            "This command starts processing of the specified job on the equipment assuming all proper initialization has taken place and material is present");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.STOP),
                            "This command will stop the processing of material at the equipment. StopConfig parameter is used to configure the stop strategy (see EC stopConfig for more details) ");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.PAUSE),
                            "This command will cause the equipment to suspend processing of material at the equipment. Processing can be restarted by issuing the Resume");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.RESUME),
                            "This command will continue processing (can only be used after pause command)");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.ABORT),
                            "This command aborts processing and immediately stops all movement from the equipment");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.PROCEEDWITHSUBSTRATE),
                            "Request to the equipment to accept the substrate");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.CANCELSUBSTRATE),
                            "Request to the equipment to skip the substrate and move into it's source location.");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.InappropriateTimeRcmd),
                            "Remote command to test inappropriate time behavior");
                        e30Standard.RemoteControlServices.RegisterRemoteCommand(
                            nameof(RemoteCommands.InvalidParameterRcmd),
                            "Remote command to test invalid parameter behavior");
                        e30Standard.Configuration.SendS6F11OnEquipmentConstantChange =
                            Integration.Integrated;

                        if (App.ControllerInstance.ControllerEquipmentManager.Equipment
                            .AllDevices<ToolControlManager>()
                            .Any())
                        {
                            var toolControlManager = App.ControllerInstance
                                .ControllerEquipmentManager.Equipment
                                .AllDevices<ToolControlManager>()
                                .First();

                            e30Standard.MessageServices.RegisterMessageHandler(
                                new S13F14MessageHandler(toolControlManager));
                            e30Standard.MessageServices.RegisterMessageHandler(
                                new S13F15MessageHandler(toolControlManager));
                            e30Standard.MessageServices.RegisterMessageHandler(
                                new S2F43MessageHandler());
                            e30Standard.MessageServices.RegisterMessageHandler(
                                new S6F23MessageHandler());
                        }

                        e30Standard.Connection.SetSecsMessageLogVerbosity(
                            SecsMessageLogVerbosity.Detailed);
                        e30Standard.Connection.SetSecsMessageLogFormat(SecsMessageLogFormat.SMN);
                    });

            short substrateLocationId = 1;
            if (App.ControllerInstance.IsGem300Supported)
            {
                gem300EnvironmentBuilder
                    .UseE40Standard(E40Compliance.New(E40Revision.AgilGEM3007_2_x), this, 99)
                    .UseE90Standard(
                        E90Compliance.New(E90Revision.AgilGEM3007_2_x),
                        this,
                        e90Standard =>
                        {
                            e90Standard.Configuration.HasSubstrateIdReader = true;

                            e90Standard.AddSubstrateLocation(
                                App.ControllerInstance.ControllerEquipmentManager.Robot
                                    .UpperArmLocation.Name,
                                substrateLocationId);
                            substrateLocationId++;

                            e90Standard.AddSubstrateLocation(
                                App.ControllerInstance.ControllerEquipmentManager.Robot
                                    .LowerArmLocation.Name,
                                substrateLocationId);
                            substrateLocationId++;

                            e90Standard.AddSubstrateLocation(
                                App.ControllerInstance.ControllerEquipmentManager.Aligner.Location
                                    .Name,
                                substrateLocationId);
                            substrateLocationId++;

                            foreach (var pm in App.ControllerInstance.ControllerEquipmentManager
                                         .ProcessModules.Values)
                            {
                                e90Standard.AddSubstrateLocation(
                                    pm.Location.Name,
                                    substrateLocationId);
                                substrateLocationId++;
                            }
                        })
                    .UseE94Standard(E94Compliance.New(E94Revision.AgilGEM3007_2_x), this, 99)
                    .UseE87Standard(
                        E87Compliance.New(E87Revision.AgilGEM3007_2_x),
                        this,
                        null,
                        e87Standard =>
                        {
                            substrateLocationId = 7;
                            foreach (var loadPortConfiguration in equipment
                                         .AllOfType<
                                             Equipment.Abstractions.Devices.LoadPort.LoadPort>()
                                         .Select(lp => lp.Configuration))
                            {
                                var lp = e87Standard.AddLoadPort(
                                    baseSubstrateLocationIndex: substrateLocationId,
                                    maxSubstrateLocationCount: 25);
                                substrateLocationId = (short)(substrateLocationId + 25);
                                try
                                {
                                    if (loadPortConfiguration.IsCarrierIdSupported)
                                    {
                                        e87Standard.AdditionalServices.NotifyIDReaderIsAvailable(
                                            lp.PortID);
                                    }
                                    else
                                    {
                                        e87Standard.AdditionalServices.NotifyIDReaderIsUnavailable(
                                            lp.PortID);
                                    }

                                    e87Standard.SetBypassReadID(
                                        lp.PortID,
                                        loadPortConfiguration.CarrierIdentificationConfig
                                            .ByPassReadId);
                                }
                                catch (Exception e)
                                {
                                    _logger.Error(e, "Error occurred in E87 services.");
                                }
                            }

                            e87Standard.Configuration.CancelCarrierBehavior =
                                CancelCarrierBehavior.UseCarrierHoldAndUnclampControl;
                        })
                    .UseE116Standard(
                        E116Compliance.New(E116Revision.AgilGEM3007_2_x)
                            .UseTrackerIDWellKnownName(),
                        this,
                        nameof(App.ControllerInstance.ControllerEquipmentManager.Controller),
                        standard =>
                        {
                            standard.AddEFEMModule(
                                nameof(App.ControllerInstance.ControllerEquipmentManager.Robot),
                                1);
                            standard.AddEFEMModule(
                                nameof(App.ControllerInstance.ControllerEquipmentManager.Aligner),
                                2);

                            var indexTracker = Locations.StartIndexSubstrateIdReader;
                            if (App.ControllerInstance.ControllerEquipmentManager
                                    .SubstrateIdReaderFront is { } idFrontReader)
                            {
                                standard.AddEFEMModule(idFrontReader.Name, indexTracker);
                            }

                            indexTracker++;

                            if (App.ControllerInstance.ControllerEquipmentManager
                                    .SubstrateIdReaderBack is { } idBackReader)
                            {
                                standard.AddEFEMModule(idBackReader.Name, indexTracker);
                            }

                            indexTracker = Locations.StartIndexLoadPort;
                            foreach (var lp in App.ControllerInstance.ControllerEquipmentManager
                                         .LoadPorts.Values)
                            {
                                standard.AddEFEMModule(lp.Name, indexTracker);
                                indexTracker++;
                            }

                            indexTracker = Locations.StartIndexProcessModule;
                            foreach (var pm in App.ControllerInstance.ControllerEquipmentManager
                                         .ProcessModules.Values)
                            {
                                standard.AddProductionModule(pm.Name, indexTracker);
                                indexTracker++;
                            }
                        });
            }

            _gem300Environment = gem300EnvironmentBuilder.Build();

            DataItemDictionary.Instance.OverwriteFormatsFromFile(
                Path.Combine(
                    configuration.ApplicationPath.AutomationConfigPath,
                    FileNames.DataItemFormats));
            MessageDescriptionDictionary.Instance.AppendFromFile(
                Path.Combine(
                    configuration.ApplicationPath.AutomationConfigPath,
                    FileNames.MessageDescriptions));

            // Create and configure all observers and other classes required for E30 support
            AlarmObserver = new AlarmObserver(E30Std, _logger);
            EquipmentServices = new EquipmentObserver(E30Std, _logger);
            MaterialMovementObserver = new MaterialMovementObserver(E30Std, _logger);
            ProcessingSmObserver = new ProcessingSmObserver(E30Std, _logger);
            StatusVariableUpdater = new StatusVariableUpdater(E30Std, _logger);
            TerminalServices = new TerminalServices(E30Std, _logger);
            ControlServices = new ControlServices(E30Std, _logger);
            E87Observer = new E87Observer(E87Std, E30Std, _logger);
            E40Observer = new E40Observer(E40Std, E30Std, _logger);
            E90Observer = new E90Observer(E90Std, E87Std, E30Std, _logger);
            E84Observer = new E84Observer(E87Std, E30Std, _logger);
            E94Observer = new E94Observer(E87Std, E94Std, E30Std, _logger);
            E116Observer = new E116Observer(E116Std, E30Std, _logger);

            var myComponents = new List<E30StandardSupport>
            {
                AlarmObserver,
                EquipmentServices,
                MaterialMovementObserver,
                ProcessingSmObserver,
                StatusVariableUpdater,
                TerminalServices,
                ControlServices,
                E87Observer,
                E40Observer,
                E90Observer,
                E84Observer,
                E94Observer,
                E116Observer
            };

            myComponents.ForEach(
                c =>
                {
                    c.OnCreate();
                    c.OnSetup(equipment);
                });

            ControlServices.OnCommunicationStateChanged += (_, _) => ControlServices_StateChanged();
            ControlServices.OnControlStateChanged += (_, _) => ControlServices_StateChanged();

            // Load Process queue
            var appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Assembly.GetExecutingAssembly().GetName().Name,
                "ProcessQueue");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            foreach (var e30Alarm in E30Std.AlarmServices.GetAlarms().Where(al => al.ID > 870000))
            {
                var alarm = GUI.Common.App.Instance.AlarmCenter.ModelBuilder.CreateAlarm(
                    e30Alarm.Name,
                    e30Alarm.Text,
                    e30Alarm.ID,
                    AlarmSeverity.Indeterminate);
                Alarms.Add(alarm);
            }

            GUI.Common.App.Instance.AlarmCenter.ModelBuilder.AddAlarms(this);
        }

        #endregion

        #region IE30Callback Support

        public int ConnectionID => 1;

        public string MDLN => GUI.Common.App.Instance.Config.EquipmentIdentityConfig.MDLN;

        public string SOFTREV => GUI.Common.App.Instance.Config.EquipmentIdentityConfig.SOFTREV;

        public RemoteCommandStatus AcceptRCMD(IRemoteCommand rcmd)
        {
            if (rcmd == null)
            {
                return new RemoteCommandStatus(HCACK.NoSuchObjectExists);
            }

            RemoteCommandStatus ret;
            _logger.Info(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Remote Command Name: {0}",
                    rcmd.Rcmd.ValueTo<string>()));

            foreach (var key in rcmd.Parameters)
            {
                _logger.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Command Parameter [{0}] : {1} ",
                        key.CPName.ValueTo<string>(),
                        key.CPVal.ToMinimalSMN()));
            }

            var remoteCmd = RemoteCommands.None;

            if (!Enum.GetNames(typeof(RemoteCommands)).Contains(rcmd.Rcmd.ValueTo<string>()))
            {
                ret = new RemoteCommandStatus(HCACK.CommandDoesNotExist);
            }
            else
            {
                if (!Enum.TryParse(rcmd.Rcmd.ValueTo<string>(), out remoteCmd))
                {
                    _logger.Info(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enable to retrieve expected command {0}",
                            rcmd.Rcmd.ValueTo<string>()));
                    return new RemoteCommandStatus(HCACK.CommandDoesNotExist);
                }

                var invalidParams = rcmd.Parameters
                    .Where(parameter => parameter.CPName.ValueTo<string>() == "INVALIDPARAM")
                    .Select(
                        param => new RemoteCommandParameterAck(
                            param.CPName,
                            CPACK.ParameterNameDoesNotExist));
                var remoteCommandParameterAcks = invalidParams.ToList();
                ret = remoteCommandParameterAcks.Any()
                    ? new RemoteCommandStatus(
                        HCACK.AtLeastOneParameterIsInvalid,
                        remoteCommandParameterAcks)
                    : new RemoteCommandStatus(HCACK.AcknowledgeCommandHasBeenPerformed);
            }

            if (!ret.Acknowledgement.Equals(HCACK.AcknowledgeCommandHasBeenPerformed))
            {
                _logger.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Remote command rejected by Application with code {0}",
                        ret.Acknowledgement.ValueTo<string>()));
            }

            // ReSharper disable once RedundantIfElseBlock
            else
            {
                switch (remoteCmd)
                {
                    case RemoteCommands.None:
                        break;
                    case RemoteCommands.LOCAL:
                        var result = TreatLocalRcmd();
                        return new RemoteCommandStatus(result);
                    case RemoteCommands.InappropriateTimeRcmd:
                        return new RemoteCommandStatus(HCACK.CannotPerformedNow);
                    case RemoteCommands.InvalidParameterRcmd:
                        return new RemoteCommandStatus(HCACK.AtLeastOneParameterIsInvalid);
                    default:
                        return new RemoteCommandStatus(HCACK.CommandDoesNotExist);
                }
            }

            return ret;
        }

        public EnhancedRemoteCommandStatus AcceptRCMD(IEnhancedRemoteCommand rcmd)
        {
            if (rcmd == null)
            {
                return new EnhancedRemoteCommandStatus(HCACK.NoSuchObjectExists);
            }

            EnhancedRemoteCommandStatus ret;
            _logger.Info(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Remote Command Name: {0}",
                    rcmd.Rcmd.ValueTo<string>()));

            foreach (var key in rcmd.Parameters)
            {
                _logger.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Command Parameter [{0}] : {1} ",
                        key.CPName.ValueTo<string>(),
                        key.CEPVal.ToMinimalSMN()));
            }

            var remoteCmd = RemoteCommands.None;

            if (!Enum.GetNames(typeof(RemoteCommands)).Contains(rcmd.Rcmd.ValueTo<string>()))
            {
                ret = new EnhancedRemoteCommandStatus(HCACK.CommandDoesNotExist);
            }
            else
            {
                if (!Enum.TryParse(rcmd.Rcmd.ValueTo<string>(), out remoteCmd))
                {
                    _logger.Info(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enable to retrieve expected command {0}",
                            rcmd.Rcmd.ValueTo<string>()));
                    return new EnhancedRemoteCommandStatus(HCACK.CommandDoesNotExist);
                }

                var invalidParams = rcmd.Parameters
                    .Where(parameter => parameter.CPName.ValueTo<string>() == "INVALIDPARAM")
                    .Select(
                        param => new EnhancedRemoteCommandParameterAck(
                            param.CPName,
                            CEPACK.ParameterNameDoesNotExist));
                var remoteCommandParameterAcks = invalidParams.ToList();
                ret = remoteCommandParameterAcks.Any()
                    ? new EnhancedRemoteCommandStatus(
                        HCACK.AtLeastOneParameterIsInvalid,
                        remoteCommandParameterAcks)
                    : new EnhancedRemoteCommandStatus(HCACK.AcknowledgeCommandHasBeenPerformed);
            }

            if (E30Std.ControlServices.ControlState != ControlState.Remote)
            {
                ret = new EnhancedRemoteCommandStatus(HCACK.CannotPerformedNow);
            }

            if (!ret.Acknowledgement.Equals(HCACK.AcknowledgeCommandHasBeenPerformed))
            {
                _logger.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Remote command rejected by Application with code {0}",
                        ret.Acknowledgement.ValueTo<string>()));
                return ret;
            }

            HCACK result;
            switch (remoteCmd)
            {
                case RemoteCommands.None:
                    result = HCACK.AcknowledgeCommandHasBeenPerformed;
                    break;
                case RemoteCommands.LOCAL:
                    result = TreatLocalRcmd();
                    break;
                case RemoteCommands.InappropriateTimeRcmd:
                    result = HCACK.CannotPerformedNow;
                    break;
                case RemoteCommands.InvalidParameterRcmd:
                    result = HCACK.AtLeastOneParameterIsInvalid;
                    break;
                case RemoteCommands.PROCEEDWITHCARRIER:
                    result = TreatProceedWithCarrierRcmd(rcmd);
                    break;
                case RemoteCommands.CANCELCARRIER:
                    result = TreatCancelCarrierRcmd(rcmd);
                    break;
                case RemoteCommands.SETUPJOB:
                    result = TreatSetupJobRcmd(rcmd);
                    break;
                case RemoteCommands.START:
                    result = TreatStartRcmd(rcmd);
                    break;
                case RemoteCommands.STOP:
                    result = TreatStopRcmd(rcmd);
                    break;
                case RemoteCommands.PAUSE:
                    result = TreatPauseRcmd(rcmd);
                    break;
                case RemoteCommands.RESUME:
                    result = TreatResumeRcmd(rcmd);
                    break;
                case RemoteCommands.ABORT:
                    result = TreatAbortRcmd(rcmd);
                    break;
                case RemoteCommands.PROCEEDWITHSUBSTRATE:
                    result = TreatProceedWithSubstrateRcmd(rcmd);
                    break;
                case RemoteCommands.CANCELSUBSTRATE:
                    result = TreatCancelSubstrateRcmd(rcmd);
                    break;
                default:
                    result = HCACK.CommandDoesNotExist;
                    break;
            }

            return new EnhancedRemoteCommandStatus(result);
        }

        public ACKC10 AcceptTerminalMessage(int terminalNumber, string[] messages)
        {
            return terminalNumber != Constants.Services.TerminalId
                ? ACKC10.TerminalNotAvailable
                : ACKC10.AcceptedForDisplay;
        }

        public ACKC7 DeletePP(IEnumerable<string> ppIDs)
        {
            if (_dataFlowManager is not ToolControlManager toolControlManager || !ppIDs.Any())
            {
                return ACKC7.ModeUnsupported;
            }

            _dataFlowManager.GetAvailableRecipes();

            var success = true;
            foreach (var ppId in ppIDs)
            {
                var recipe = _dataFlowManager.GetRecipeInfo(ppId);

                if (recipe == null)
                {
                    return ACKC7.PPIDNotFound;
                }

                var result = toolControlManager.DeleteRecipe(new List<string>() { _dataFlowManager.GetRecipeName(recipe) });

                if (!result.Success)
                {
                    success = false;
                    break;
                }
            }

            return success
                ? ACKC7.Accepted
                : ACKC7.PermissionNotGranted;
        }

        public IProcessProgram GetPP(string ppID, bool isFormatedPP)
        {
            if (_dataFlowManager is not ToolControlManager toolControlManager
                || isFormatedPP
                || string.IsNullOrEmpty(ppID))
            {
                return null;
            }

            var result = toolControlManager.DownloadRecipe(ppID);

            if (result is not { Success: true })
            {
                return null;
            }

            var processProgram = new UnformattedProcessProgram(result.RecipeName);
            processProgram.Body = Convert.ToBase64String(result.Recipe.ToArray());

            return processProgram;
        }

        public IEnumerable<string> GetPPList()
        {
            _dataFlowManager.GetAvailableRecipes();
            return _dataFlowManager.GetRecipeNames();
        }

        public void PPAvailableRequest(
            string recipeName,
            out UNFLEN unformattedPPLength,
            out FRMLEN formattedPPLength)
        {
            _dataFlowManager.GetAvailableRecipes();
            var recipe = _dataFlowManager.GetRecipeInfo(recipeName);
            unformattedPPLength = 0;
            formattedPPLength = 0;
            if (recipe != null)
            {
                unformattedPPLength = 1;
            }
        }

        public PPGNT PPLoadInquire(string recipeName, int lengthInBytes)
        {
            //TODO Handle errors
            return PPGNT.OK;
        }

        public ACKC7 StorePP(IProcessProgram processProgram)
        {
            if (_dataFlowManager is not ToolControlManager toolControlManager
                || processProgram is not UnformattedProcessProgram unformattedProcessProgram)
            {
                return ACKC7.ModeUnsupported;
            }

            RecipeUploadResult result;
            using (var stream = new MemoryStream())
            {
                var bytes = Convert.FromBase64String(unformattedProcessProgram.Body);
                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                result = toolControlManager.UploadRecipe(unformattedProcessProgram.Name, stream);
            }

            if (result == null)
            {
                return ACKC7.ModeUnsupported;
            }

            return result.Success
                ? ACKC7.Accepted
                : ACKC7.ModeUnsupported;
        }

        public EAC VerifyEquipmentConstantValue(int variableId, string variableName, DataItem value)
        {
            switch (variableName)
            {
                case ECs.SetupMatchingConfigurationValue:
                case ECs.PmMatchingConfigurationValue:
                    return EAC.DeniedAtLeastOneConstantOutOfRange;
                default:
                    return EAC.Acknowledge;
            }
        }

        #endregion IE30Callback Support

        #region IDisposable Support

        private bool _isDisposed; // To detect redundant calls

        // Override a finalizer only if Dispose(bool isDisposing) has code to free unmanaged resources.
        ~GemController()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing).
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing).
            Dispose(true);

            // Uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    IsSetupDone = false;

                    StatusVariableUpdater?.Dispose();
                    TerminalServices?.Dispose();
                    ControlServices?.Dispose();
                    AlarmObserver?.Dispose();
                    EquipmentServices?.Dispose();
                    MaterialMovementObserver?.Dispose();
                    ProcessingSmObserver?.Dispose();
                    E40Observer?.Dispose();
                    E84Observer?.Dispose();
                    E87Observer?.Dispose();
                    E90Observer?.Dispose();
                    E94Observer?.Dispose();
                }

                // To detect redundant calls
                _isDisposed = true;
            }
        }

        #endregion IDisposable Support

        #region IAlarmProvider

        public string Class { get; } = "GEMControllerAlarms";

        public int InstanceId { get; } = 0;

        public Collection<Alarm> Alarms { get; } = new();

        #endregion

        #region IE87Callback

        public CarrierPosition GetCarrierPosition(Carrier carrier)
        {
            return CarrierPosition.ReadWrite;
        }

        public CarrierIDReadResult ReadTag(Carrier carrier, string dataSeg, uint dataSize)
        {
            var deviceLoadPort =
                App.ControllerInstance.ControllerEquipmentManager.LoadPorts.Values.FirstOrDefault(
                    lp => lp.Carrier != null && lp.Carrier.Id == carrier.ObjID);

            if (deviceLoadPort == null)
            {
                return new CarrierIDReadResult(string.Empty, InvalidDataOrArgument());
            }

            try
            {
                deviceLoadPort.ReadCarrierId();
                return new CarrierIDReadResult(deviceLoadPort.Carrier.Id, Performed());
            }
            catch
            {
                return new CarrierIDReadResult(string.Empty, CannotPerformNow());
            }
        }

        public E87Status WriteTag(Carrier carrier, string dataSeg, uint dataSize, string data)
        {
            throw new InvalidOperationException($"{nameof(WriteTag)} is not supported");
        }

        public void ReadCarrierId(LoadPort loadPort)
        {
            try
            {
                var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                    .First(lp => lp.Value.InstanceId == loadPort.PortID)
                    .Value;

                deviceLoadPort.ReadCarrierId();
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error occurred in {nameof(ReadCarrierId)} callback");
            }
        }

        public void WriteCarrierId(LoadPort loadPort, string carrierId)
        {
            throw new InvalidOperationException($"{nameof(WriteCarrierId)} is not supported");
        }

        public void MoveCarrierToUnloadPosition(LoadPort loadPort, bool keepCarrierClamped = false)
        {
            ClearAlarms();

            var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                .First(lp => lp.Value.InstanceId == loadPort.PortID)
                .Value;

            var jobList = App.ControllerInstance.JobQueueManager.JobQueue.Where(
                    j => j.MaterialNameList.Any(
                        m => m.CarrierID == loadPort.AssociatedCarrier?.ObjID))
                .ToList();

            if (_controller.State is not OperatingModes.Maintenance and not OperatingModes.Engineering
                && jobList.Any(j => j.CurrentExecution < j.NumberOfExecutions || j.LoopMode)
                && (!App.ControllerInstance.ControllerConfig.UnloadCarrierBetweenJobs
                    || IsControlledByHost))

            {
                //Do not unload the carrier if we know that another job will be run
                return;
            }

            //Do not unload carrier in case of job aborted
            if (_controller.State is not OperatingModes.Maintenance and not OperatingModes.Engineering
                || App.ControllerInstance.ControllerConfig.UnloadCarrierAfterAbort)
            {
                if (keepCarrierClamped)
                {
                    deviceLoadPort.Undock();
                }
                else
                {
                    if (deviceLoadPort.IsClamped)
                    {
                        deviceLoadPort.ReleaseCarrier();
                    }
                }

                try
                {
                    E87Std.IntegrationServices.NotifyCarrierMoveToUnloadPositionFinished(
                        loadPort.PortID);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error occurred in E87 services.");
                }
            }
        }

        public void MoveCarrierToWriteTimePosition(LoadPort loadPort)
        {
            var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                .First(lp => lp.Value.InstanceId == loadPort.PortID)
                .Value;

            var jobList = App.ControllerInstance.JobQueueManager.JobQueue.Where(
                    j => j.MaterialNameList.Any(
                        m => m.CarrierID == loadPort.AssociatedCarrier?.ObjID))
                .ToList();

            if (_controller.State is not OperatingModes.Maintenance and not OperatingModes.Engineering
                && jobList.Any(j => j.CurrentExecution < j.NumberOfExecutions || j.LoopMode)
                && (!App.ControllerInstance.ControllerConfig.UnloadCarrierBetweenJobs
                    || IsControlledByHost))

            {
                //Do not unload the carrier if we know that another job will be run
                return;
            }

            //Do not unload carrier in case of job aborted
            if (_controller.State is not OperatingModes.Maintenance and not OperatingModes.Engineering
                || App.ControllerInstance.ControllerConfig.UnloadCarrierAfterAbort)
            {
                deviceLoadPort.Undock();
                try
                {
                    E87Std.IntegrationServices.NotifyCarrierMoveToWritePositionFinished(
                        loadPort.PortID);
                    if (E87Std.Configuration.CancelCarrierBehavior is CancelCarrierBehavior
                            .UseCarrierHoldAndUnclampControl
                        && E87Std.CarrierHold is CarrierHold.HostRelease
                        && !IsControlledByHost)
                    {
                        E87Std.StandardServices.CarrierRelease(
                            loadPort.PortID,
                            loadPort.AssociatedCarrier.ObjID);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error occurred in E87 services.");
                }
            }
        }

        public void ReadCarrierSlotMap(LoadPort loadPort)
        {
            try
            {
                var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                    .First(lp => lp.Value.InstanceId == loadPort.PortID)
                    .Value;

                deviceLoadPort.Open(true);
                deviceLoadPort.PostTransfer();
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error occurred in {nameof(ReadCarrierSlotMap)} callback");
            }
        }

        public E87Status AcceptCarrierAccessModeChangement(AccessMode newMode, LoadPort loadPort)
        {
            try
            {
                var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                    .First(lp => lp.Value.InstanceId == loadPort.PortID)
                    .Value;

                if (deviceLoadPort == null)
                {
                    return InvalidDataOrArgument();
                }

                if (!deviceLoadPort.Configuration.IsE84Enabled && newMode == AccessMode.Auto)
                {
                    return InvalidState();
                }

                if ((newMode == AccessMode.Auto && deviceLoadPort.AccessMode == LoadingType.Auto)
                    || (newMode == AccessMode.Manual
                        && deviceLoadPort.AccessMode == LoadingType.Manual))
                {
                    //Device load port already in good access mode
                    return Performed();
                }

                if (!deviceLoadPort.CanExecute(
                        nameof(ILoadPort.SetAccessMode),
                        true,
                        out var context,
                        newMode == AccessMode.Auto
                            ? LoadingType.Auto
                            : LoadingType.Manual))
                {
                    return CannotPerformNow()
                        .WithError(
                            ErrorCode.ActionCanNotBePerformedNow,
                            context.Errors.FirstOrDefault() ?? string.Empty);
                }

                return Performed();
            }
            catch
            {
                return CannotPerformNow();
            }
        }

        public E87Status AcceptCarrierRecreate(string carrierID, LoadPort loadPort)
        {
            return Performed();
        }

        public E87Status AcceptProceedWithCarrier(
            string carrierId,
            LoadPort loadPort,
            E87PropertiesList propertiesList)
        {
            var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                .First(lp => lp.Value.InstanceId == loadPort.PortID)
                .Value;

            if (deviceLoadPort == null)
            {
                return InvalidDataOrArgument();
            }

            if (deviceLoadPort.Carrier is null)
            {
                return CannotPerformNow();
            }

            if (propertiesList != null
                && !string.IsNullOrEmpty(propertiesList.Usage)
                && !DVs.Usages.Contains(propertiesList.Usage))
            {
                return InvalidDataOrArgument()
                    .WithError(new Error(ErrorCode.InvalidAttributeValue, "Unknown Usage"));
            }

            ClearAlarms();

            return Performed();
        }

        public bool IsDuplicateCarrierProcessingHasBegun(string carrierId)
        {
            var carrier = E87Std.GetCarrierById(carrierId);
            return carrier.SlotMapStatus == SlotMapStatus.VerificationOk;
        }

        public bool IsLoadPortReadyForTransfer(LoadPort loadPort)
        {
            var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                .First(lp => lp.Value.InstanceId == loadPort.PortID)
                .Value;

            return !deviceLoadPort.IsClamped;
        }

        public void FinishCarrierReCreateLoading(LoadPort loadPort)
        {
            var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                .First(lp => lp.Value.InstanceId == loadPort.PortID)
                .Value;

            if (!deviceLoadPort.IsClamped)
            {
                deviceLoadPort.Clamp();
            }
        }

        #endregion

        #region E40

        public E40Status AcceptCreation(IProcessJob job)
        {
            _dataFlowManager.GetAvailableRecipes();

            //Check if recipe exists
            if (_dataFlowManager.GetRecipeInfo(job.RecipeID) is null)
            {
                var userMessage =
                    $"Unable to create job {job.ObjID}: recipe {job.RecipeID} does not exist";
                OnUserErrorRaised(userMessage);

                return E40Status.Unsuccessful(ErrorCode.InvalidParameter, userMessage);
            }

            //Check if ocr profile exists
            if (job.RecipeMethod == RecipeMethod.RecipeWithVariableTuning)
            {
                foreach (var recipeVariable in job.RecipeVariables)
                {
                    string userMessage;
                    switch (recipeVariable.Name)
                    {
                        case RecipeParameter.OcrProfileName:
                            var ocrProfileName = recipeVariable.ValueAsDataItem.ValueTo<string>();

                            if (!App.ControllerInstance.ControllerConfig.OcrProfiles.Exists(
                                    profile => profile.Name.Equals(ocrProfileName)))
                            {
                                userMessage =
                                    $"Unable to create job {job.ObjID}: OCR profile {ocrProfileName} does not exist";
                                OnUserErrorRaised(userMessage);
                                return E40Status.Unsuccessful(
                                    ErrorCode.InvalidParameter,
                                    userMessage);
                            }

                            break;
                        default:
                            userMessage =
                                $"Unable to create job {job.ObjID}: Recipe parameter {recipeVariable.Name} does not exist";
                            OnUserErrorRaised(userMessage);
                            return E40Status.Unsuccessful(ErrorCode.InvalidParameter, userMessage);
                    }
                }
            }

            foreach (var element in job.CarrierIDSlotsAssociation)
            {
                foreach (var slotId in element.SlotIds)
                {
                    var substrates = E90Std.Substrates.Where(
                        s => s.SubstSource == $"{element.CarrierID}.{slotId:00}"
                             && s.SubstProcState > SubstProcState.InProcess);
                    if (substrates.Any())
                    {
                        var userMessage =
                            $"Unable to create job {job.ObjID}: Carrier {element.CarrierID} / Slot {slotId} already processed";
                        OnUserErrorRaised(userMessage);
                        return E40Status.Unsuccessful(
                            ErrorCode.MaterialPartiallyProcessed,
                            userMessage);
                    }
                }

                var deviceLoadPort = App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                    .FirstOrDefault(lp => lp.Value.Carrier?.Id == element.CarrierID)
                    .Value;

                if (deviceLoadPort is { State: OperatingModes.Maintenance })
                {
                    var userMessage =
                        $"Unable to create job {job.ObjID}: Load port for Carrier {element.CarrierID} is in maintenance";
                    OnUserErrorRaised(userMessage);
                    return E40Status.Unsuccessful(
                        ErrorCode.ActionCanNotBePerformedNow,
                        userMessage);
                }
            }

            return E40Status.Successful();
        }

        public E40Status AcceptJobSetup(IProcessJob job)
        {
            return E40Status.Successful();
        }

        public void SetupJob(IProcessJob job)
        {
            // Setup is done when all material is arrived
            var isMaterialPresent = true;
            try
            {
                if (job.MaterialType == MaterialType.Carriers)
                {
                    if (job.CarrierIDSlotsAssociation
                        .Select(element => E87Std.GetCarrierById(element.CarrierID))
                        .Any(carrier => carrier == null))
                    {
                        isMaterialPresent = false;
                    }

                    if (isMaterialPresent && !Helpers.Helpers.IsAllJobWafersInstantiated(job))
                    {
                        return;
                    }
                }
                else
                {
                    // Handle only Carriers MaterialType
                    isMaterialPresent = false;
                }
            }
            catch (Exception)
            {
                isMaterialPresent = false;
            }

            if (!isMaterialPresent)
            {
                var userMessage = $"Setup job {job.ObjID} failed: All material not received.";
                E40Std.Logger.Error(userMessage);
                OnUserErrorRaised(userMessage);
                return;
            }

            try
            {
                var controlJob = E94Std.GetControlJobFromProcessJob(job.ObjID);
                var equipmentJob = CreateControllerJob(controlJob, job);
                _controller.CreateJob(equipmentJob);
                App.ControllerInstance.CountersManager.IncrementCounter(CounterDefinition.JobCounter);

                var dataflowRecipeInfos = _dataFlowManager.GetRecipeInfo(job.RecipeID);
                var utoJobProgram = _dataFlowManager.GetJobProgramFromRecipe(dataflowRecipeInfos);
                var processModules = _controller.AllDevices<DriveableProcessModule>().ToList();

                foreach (var actorType in utoJobProgram.PMItems.Select(s => s.PMType))
                {
                    var modules = processModules.Where(pm => pm.ActorType == actorType).ToList();
                    if (modules.All(
                            s => s.ProcessModuleState == ProcessModuleState.Offline
                                 || s.IsOutOfService))
                    {
                        var userMessage =
                            $"Unable to setup job {job.ObjID}: {actorType} is not available";
                        throw new InvalidOperationException(userMessage);
                    }

                    foreach (var processModule in modules)
                    {
                        if (equipmentJob.Wafers.All(
                                w => processModule.SupportedSampleDimensions.Contains(
                                    w.MaterialDimension)))
                        {
                            continue;
                        }

                        var userMessage =
                            $"Unable to setup job {job.ObjID}: Mismatch supported wafer size detected for process module {processModule.Name}";
                        throw new InvalidOperationException(userMessage);
                    }
                }

                E40Std.IntegrationServices.NotifySetupIsDone(job.ObjID);
            }
            catch (InvalidOperationException ex)
            {
                E40Std.Logger.Error(ex.Message);
                OnUserErrorRaised(ex.Message);
            }
            catch (Exception e)
            {
                E40Std.Logger.Error($"Setup job {job.ObjID} failed: {e.Message}");
                OnUserErrorRaised(
                    $"Unable to setup job {job.ObjID}. Please check logs and flow manager.");
                E40Std.StandardServices.Command(
                    job.ObjID,
                    CommandName.STOP,
                    new List<CommandParameter>());
            }
        }

        public E40Status AcceptJobExecution(IProcessJob job)
        {
            var equipmentJob = _controller.Jobs.FirstOrDefault(j => j.Name == job.ObjID);
            var context = _controller.NewCommandContext(nameof(IController.StartJobExecution));
            context.AddArgument("job", equipmentJob);
            if (!_controller.CanExecute(context))
            {
                var userMessage =
                    $"Unable to start job {job.ObjID}: Equipment is not ready to execute a job";
                OnUserErrorRaised(userMessage);
                return E40Status.Unsuccessful(ErrorCode.ActionCanNotBePerformedNow, userMessage);
            }

            return E40Status.Successful();
        }

        public void ExecuteJob(IProcessJob job)
        {
            Job equipmentJob;
            try
            {
                equipmentJob = _controller.Jobs.FirstOrDefault(j => j.Name == job.ObjID);

                if (equipmentJob != null)
                {
                    var recipe = _dataFlowManager.GetRecipeInfo(job.RecipeID);
                    _dataFlowManager.StartRecipe(
                        new MaterialRecipe(recipe, equipmentJob.Wafers),
                        job.ObjID);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Not able to start the job execution: Job is null");
                }
            }
            catch (Exception e)
            {
                E40Std.Logger.Error($"Execute job {job.ObjID} failed: {e.Message}");
                OnUserErrorRaised(
                    $"Unable to start recipe {job.RecipeID}. Please check logs and flow manager.");
                E40Std.StandardServices.Command(
                    job.ObjID,
                    CommandName.ABORT,
                    new List<CommandParameter>());
                return;
            }

            try
            {
                _controller.StartJobExecution(equipmentJob);
            }
            catch (Exception e)
            {
                E40Std.Logger.Error($"Execute job {job.ObjID} failed: {e.Message}");
                OnUserErrorRaised($"Unable to start job {job.ObjID}. Please check logs.");
                E40Std.StandardServices.Command(
                    job.ObjID,
                    CommandName.STOP,
                    new List<CommandParameter>());
            }
        }

        public E40Status AcceptJobAbort(IProcessJob job)
        {
            return E40Status.Successful();
        }

        public void AbortJob(IProcessJob job)
        {
            //The job state machine is running so we can abort it
            _controller.Interrupt(InterruptionKind.Abort);
            App.ControllerInstance.CountersManager.IncrementCounter(CounterDefinition.AbortCounter);
            try
            {
                E40Std.IntegrationServices.NotifyJobHasBeenAborted(job.ObjID);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error occurred in {nameof(AbortJob)}");
            }
        }

        public E40Status AcceptJobStop(IProcessJob job)
        {
            if (job.JobState is JobState.QUEUED_POOLED
                or JobState.WAITINGFORSTART
                or JobState.SETTINGUP)
            {
                return E40Status.Successful();
            }

            var stopConfig = E30Std.EquipmentConstantsServices
                .GetValueByWellKnownName(ECs.StopConfig)
                .ValueTo<StopConfig>();
            return _controller.CanExecute(
                nameof(IController.Stop),
                true,
                out _,
                job.ObjID,
                stopConfig)
                ? E40Status.Successful()
                : E40Status.Unsuccessful();
        }

        public void StopJob(IProcessJob job)
        {
            //Verify if any substrate are in wait of proceed with substrate on aligner
            var substrateList =
                E90Std.Substrates.Where(s => s.SubstIDStatus.Equals(SubstIDStatus.WaitingForHost));

            foreach (var substrate in substrateList)
            {
                E90Std.StandardServices.CancelSubstrate(substrate.ObjID, substrate.SubstLocID);
            }

            var dataFlowManager = _controller.GetDevice<AbstractDataFlowManager>();
            var stopConfig = dataFlowManager is { IsStopCancelAllJobsRequested: true }
                ? StopConfig.CancelProcess
                : E30Std.EquipmentConstantsServices.GetValueByWellKnownName(ECs.StopConfig)
                    .ValueTo<StopConfig>();

            _controller.Stop(job.ObjID, stopConfig);
        }

        public E40Status AcceptJobPause(IProcessJob job)
        {
            return _controller.CanExecute(nameof(IController.Pause), true, out _, job.ObjID)
                ? E40Status.Successful()
                : E40Status.Unsuccessful();
        }

        public void PauseJob(IProcessJob job)
        {
            _controller.Pause(job.ObjID);
        }

        public E40Status AcceptJobResume(IProcessJob job)
        {
            return _controller.CanExecute(nameof(IController.Resume), true, out _, job.ObjID)
                ? E40Status.Successful()
                : E40Status.Unsuccessful();
        }

        public void ResumeJob(IProcessJob job)
        {
            _controller.Resume(job.ObjID);
        }

        public E40Status AcceptJobCancel(IProcessJob job)
        {
            if (job.JobState is not JobState.QUEUED_POOLED
                || _controller.Jobs.Any(j => j.Name == job.ObjID))
            {
                return E40Status.Unsuccessful();
            }

            return E40Status.Successful();
        }

        public E40Status AcceptRecipeVariable(
            IProcessJob job,
            IEnumerable<RecipeVariable> recipeVariables)
        {
            return E40Status.Unsuccessful(
                ErrorCode.InvalidParameter,
                "This method is not supported");
        }

        public Ack AcceptMtrlOrder(MaterialOrder materialOrder)
        {
            return Ack.Unsuccessful;
        }

        public E40Status AcceptSuperseding(IProcessJob job)
        {
            return E40Status.Unsuccessful(
                ErrorCode.InvalidParameter,
                "This method is not supported");
        }

        #endregion

        #region E94

        public E94Status AcceptCreation(IControlJob job)
        {
            return E94Status.Success();
        }

        public E94Status AcceptStart(string controlJobID)
        {
            return E94Status.Success();
        }

        public E94Status AcceptPause(string controlJobID)
        {
            return E94Status.Success();
        }

        public E94Status AcceptResume(string controlJobID)
        {
            return E94Status.Success();
        }

        public E94Status AcceptCancel(string controlJobID)
        {
            return E94Status.Success();
        }

        public E94Status AcceptStop(string controlJobID)
        {
            var cj = E94Std.GetControlJob(controlJobID);
            var stopConfig = E30Std.EquipmentConstantsServices
                .GetValueByWellKnownName(ECs.StopConfig)
                .ValueTo<StopConfig>();

            if (cj.State is State.QUEUED or State.SELECTED or State.WAITINGFORSTART)
            {
                return E94Status.Success();
            }

            foreach (var job in cj.CurrentProcessJobs)
            {
                if (_controller.CanExecute(nameof(IController.Stop), true, out _, job, stopConfig))
                {
                    return E94Status.Success();
                }
            }

            return E94Status.Failure(
                ErrorCode.ActionCanNotBePerformedNow,
                "Equipment is not ready to stop a job");
        }

        public E94Status AcceptAbort(string controlJobID)
        {
            return E94Status.Success();
        }

        public E94Status AcceptHoq(string controlJobID)
        {
            return E94Status.Success();
        }

        public E94Status AcceptDeselect(string controlJobID)
        {
            return E94Status.Success();
        }

        public IProcessJob SelectNextProcessJobToSetup(
            IControlJob cj,
            IEnumerable<IProcessJob> possibleJobsToSetup)
        {
            return possibleJobsToSetup.First();
        }

        private readonly object _selectJobLock = new();

        public E94Status AcceptSelect(string controlJobID)
        {
            lock (_selectJobLock)
            {
                if (_controller.State is OperatingModes.Maintenance or OperatingModes.Engineering)
                {
                    return E94Status.Failure(
                        ErrorCode.ActionCanNotBePerformedNow,
                        "Controller is in maintenance");
                }

                if (_controller.TryGetDevice<AbstractDataFlowManager>() is
                    {
                        DataflowState: TC_DataflowStatus.Maintenance
                    })
                {
                    return E94Status.Failure(
                        ErrorCode.ActionCanNotBePerformedNow,
                        "DataFlowManager is in maintenance");
                }

                if (E94Std.ControlJobs.Any(cj => cj.State == State.SELECTED))
                {
                    return E94Status.Failure(
                        ErrorCode.ActionCanNotBePerformedNow,
                        "One CJ is already selected");
                }

                //Use copy for avoid exception on collection changed
                var jobs = _controller.Jobs.ToList();

                if (!jobs.Any(j => j.Status is JobStatus.Executing)
                    || jobs.All(
                        j => j.Status is JobStatus.Completed
                            or JobStatus.Failed
                            or JobStatus.Stopped)
                    || (jobs.All(
                            j => j.Status == JobStatus.Executing && j.RemainingWafers.Count == 0)
                        && !App.ControllerInstance.ControllerConfig.DisableParallelControlJob))
                {
                    return E94Status.Success();
                }

                return E94Status.Failure(
                    ErrorCode.ActionCanNotBePerformedNow,
                    "Equipment is not ready to select a new job");
            }
        }

        public E94Status AcceptMtrlOutSpec(
            string controlJobID,
            Collection<MaterialOutSpecification> mtrlOutSpec)
        {
            return E94Status.Failure(ErrorCode.InvalidCommand, "This method is not supported");
        }

        #endregion

        #region E90

        public Status IsSubstratePresent(string substrateId, MaterialLocation location)
        {
            if (App.ControllerInstance.ControllerEquipmentManager.Robot.LowerArmLocation.Wafer
                    .SubstrateId.Equals(substrateId)
                || App.ControllerInstance.ControllerEquipmentManager.Robot.UpperArmLocation.Wafer
                    .SubstrateId.Equals(substrateId)
                || App.ControllerInstance.ControllerEquipmentManager.Aligner.Location.Wafer
                    .SubstrateId.Equals(substrateId)
                || App.ControllerInstance.ControllerEquipmentManager.ProcessModules.Values.Any(
                    p => p.Location.Wafer.SubstrateId.Equals(substrateId)))
            {
                return Status.PerformedSuccessful();
            }

            return Status.AtLeastOneParameterIsInvalid();
        }

        public Status AcceptProceedWithSubstrate(Substrate substrate)
        {
            if (substrate is null)
            {
                return Status.AtLeastOneParameterIsInvalid(
                    ErrorCode.InvalidParameter,
                    "Substrate is null");
            }

            //Keep safety test light on for the moment
            //We can add test for job status or aligner activity later

            return Status.PerformedSuccessful();
        }

        public void ProceedSubstrate(Substrate substrate)
        {
            _controller.ProceedWithSubstrate();
        }

        public Status AcceptCancelSubstrate(Substrate substrate)
        {
            if (substrate is null)
            {
                return Status.AtLeastOneParameterIsInvalid(
                    ErrorCode.InvalidParameter,
                    "Substrate is null");
            }

            if (substrate.SubstIDStatus != SubstIDStatus.WaitingForHost)
            {
                return Status.CannotPerformNow();
            }

            //Keep safety test light on for the moment
            //We can add test for job status or aligner activity later

            return Status.PerformedSuccessful();
        }

        public void MoveSubstrateToDestination(Substrate substrate)
        {
            _controller.CancelSubstrate();
        }

        #endregion

        #region E116

        public BlockedResult IsEquipmentBlocked()
        {
            return _controller.State == OperatingModes.Maintenance
                ? BlockedResult.Blocked(
                    BlockedReason.AbortingAborted,
                    "Equipment is in Maintenance mode")
                : BlockedResult.NotBlocked();
        }

        #endregion

        #region Private Methods

        #region RCMD

        private HCACK TreatLocalRcmd()
        {
            if (!E30Std.RemoteControlServices.ProcessState.Equals(
                    nameof(OperatingModes.Idle)))
            {
                return HCACK.CannotPerformedNow;
            }

            if (ControlServices.ControlState != ControlState.Local)
            {
                ControlServices.SetLocalMode();
            }

            return HCACK.AcknowledgeCommandHasBeenPerformed;
        }

        private EnhancedRemoteCommandParameter GetParameter(
            IEnumerable<EnhancedRemoteCommandParameter> parameters,
            string paramName)
        {
            return parameters.FirstOrDefault(
                p => p.CPName == paramName);
        }

        private List<SlotState> GetSlotMap(IEnumerable<EnhancedRemoteCommandParameter> parameters)
        {
            return GetParameter(parameters, nameof(RemoteCommandsParamters.SlotMap))?.CEPVal.AdaptTo<DataList>()
                ?.DataItems.Select(slot => (SlotState)slot.ValueTo<byte>())
                .ToList();
        }

        private List<ContentMapItem> GetContentMap(IEnumerable<EnhancedRemoteCommandParameter> parameters)
        {
            var contentMap = new List<ContentMapItem>();
            var dataList = GetParameter(parameters, nameof(RemoteCommandsParamters.ContentMap))?.CEPVal.AdaptTo<DataList>();

            if (dataList != null)
            {
                foreach (var item in dataList.DataItems)
                {
                    if (item is DataList subItemList)
                    {
                        contentMap.Add(
                            new ContentMapItem(
                                subItemList.DataItems[0].ValueTo<string>(),
                                subItemList.DataItems[1].ValueTo<string>()));
                    }
                }
            }

            return contentMap;
        }

        private List<MaterialNameListElement> GetMaterialNameListElements(IEnumerable<EnhancedRemoteCommandParameter> parameters)
        {
            var materialNameListElement = new List<MaterialNameListElement>();
            var dataList = GetParameter(parameters, nameof(RemoteCommandsParamters.Carriers))?.CEPVal.AdaptTo<DataList>();

            if(dataList != null)
            {
                foreach (var item in dataList.DataItems)
                {
                    if (item is DataList subItemList)
                    {
                        var carrierId = subItemList.DataItems[0].ValueTo<string>();
                        var slotIds = new List<byte>();
                        if (subItemList.DataItems[1].AdaptTo<DataList>() is { } slotDataList)
                        {
                            slotIds = slotDataList.DataItems.Select(slot => slot.ValueTo<byte>()).ToList();
                        }

                        materialNameListElement.Add(new MaterialNameListElement(carrierId, slotIds));
                    }
                }
            }

            return materialNameListElement;
        }

        private HCACK TreatProceedWithCarrierRcmd(IEnhancedRemoteCommand rcmd)
        {
            var carrierId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.CarrierId))?.CEPVal.ValueTo<string>();
            var portId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.PortId))?.CEPVal.ValueTo<byte>();
            var slotMap = GetSlotMap(rcmd.Parameters);
            var contentMap = GetContentMap(rcmd.Parameters);

            if (string.IsNullOrEmpty(carrierId)
                || portId == null
                || slotMap.IsNullOrEmpty()
                || contentMap.IsNullOrEmpty())
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            var pwcStatus = E87Std.StandardServices.ProceedWithCarrier(new PortId((byte)portId), carrierId, new E87PropertiesList(slotMap,contentMap));
            return Helpers.Helpers.ConvertAcknowledgeToHcack(pwcStatus.Acknowledge);
        }

        private HCACK TreatCancelCarrierRcmd(IEnhancedRemoteCommand rcmd)
        {
            var carrierId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.CarrierId))?.CEPVal.ValueTo<string>();
            var portId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.PortId))?.CEPVal.ValueTo<byte>();

            if (string.IsNullOrEmpty(carrierId) || portId == null)
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            var cancelCarrierStatus = E87Std.StandardServices.CancelCarrier(carrierId, new PortId((byte)portId));
            return Helpers.Helpers.ConvertAcknowledgeToHcack(cancelCarrierStatus.Acknowledge);
        }

        private HCACK TreatSetupJobRcmd(IEnhancedRemoteCommand rcmd)
        {
            var jobId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.JobId))?.CEPVal.ValueTo<string>();
            var recipeId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.RecipeId))?.CEPVal.ValueTo<string>();
            var materialElements = GetMaterialNameListElements(rcmd.Parameters);
            var autoStart = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.AutoStart))?.CEPVal.ValueTo<bool>();
            var ocrProfileName = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.OcrProfileName))?.CEPVal.ValueTo<string>();

            if (string.IsNullOrEmpty(jobId)
                || string.IsNullOrEmpty(recipeId)
                || materialElements.IsNullOrEmpty())
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            var pjResult = E40Std.StandardServices.CreateEnh(
                jobId,
                materialElements,
                new Recipe()
                {
                    ID = recipeId,
                    Method = string.IsNullOrEmpty(ocrProfileName)
                        ? RecipeMethod.RecipeOnly
                        : RecipeMethod.RecipeWithVariableTuning,
                    Variables = string.IsNullOrEmpty(ocrProfileName)
                        ? new Collection<RecipeVariable>()
                        : new Collection<RecipeVariable>()
                        {
                            new(
                                RecipeParameter.OcrProfileName,
                                DataItem.FromObject(
                                    ocrProfileName,
                                    DataItemFormat.ASC)),
                        }
                },
                autoStart.HasValue && autoStart.Value
                    ? ProcessStart.AutomaticStart
                    : ProcessStart.ManualStart,
                new List<string>());


            if (pjResult.Status.IsFailure)
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            var cjId = $"CJ_{DateTime.Now:yyyyMMddhhmmss}";
            E94Std.AddControlJob(
                cjId,
                new Collection<string>(materialElements.Select(e => e.CarrierID).ToList()),
                new Collection<MaterialOutSpecification>(),
                new Collection<ProcessingControlSpecification>()
                {
                    new() { PRJobID = jobId }
                },
                ProcessOrderManagement.LIST,
                StartMethod.Auto);

            return HCACK.AcknowledgeCommandHasBeenPerformed;
        }

        private HCACK TreatStartRcmd(IEnhancedRemoteCommand rcmd)
        {
            var jobId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.JobId))?.CEPVal.ValueTo<string>();
            if (string.IsNullOrEmpty(jobId))
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            var startStatus = E40Std.StandardServices.Command(jobId, CommandName.STARTPROCESS, new List<CommandParameter>());
            return Helpers.Helpers.ConvertBoolToHcack(startStatus.IsSuccess);
        }

        private HCACK TreatStopRcmd(IEnhancedRemoteCommand rcmd)
        {
            var jobId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.JobId))?.CEPVal.ValueTo<string>();
            var stopConfig = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.StopConfig))?.CEPVal.ValueTo<byte>();
            if (string.IsNullOrEmpty(jobId))
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            if (stopConfig != null)
            {
                E30Std.EquipmentConstantsServices.SetValueByWellKnownName(ECs.StopConfig, stopConfig);
            }

            var stopStatus = E40Std.StandardServices.Command(jobId, CommandName.STOP, new List<CommandParameter>());
            return Helpers.Helpers.ConvertBoolToHcack(stopStatus.IsSuccess);
        }

        private HCACK TreatPauseRcmd(IEnhancedRemoteCommand rcmd)
        {
            var jobId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.JobId))?.CEPVal.ValueTo<string>();
            if (string.IsNullOrEmpty(jobId))
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            var pauseStatus = E40Std.StandardServices.Command(jobId, CommandName.PAUSE, new List<CommandParameter>());
            return Helpers.Helpers.ConvertBoolToHcack(pauseStatus.IsSuccess);
        }

        private HCACK TreatResumeRcmd(IEnhancedRemoteCommand rcmd)
        {
            var jobId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.JobId))?.CEPVal.ValueTo<string>();
            if (string.IsNullOrEmpty(jobId))
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            var pauseStatus = E40Std.StandardServices.Command(jobId, CommandName.RESUME, new List<CommandParameter>());
            return Helpers.Helpers.ConvertBoolToHcack(pauseStatus.IsSuccess);
        }

        private HCACK TreatAbortRcmd(IEnhancedRemoteCommand rcmd)
        {
            var jobId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.JobId))?.CEPVal.ValueTo<string>();
            if (string.IsNullOrEmpty(jobId))
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            var pauseStatus = E40Std.StandardServices.Command(jobId, CommandName.ABORT, new List<CommandParameter>());
            return Helpers.Helpers.ConvertBoolToHcack(pauseStatus.IsSuccess);
        }

        private HCACK TreatProceedWithSubstrateRcmd(IEnhancedRemoteCommand rcmd)
        {
            var substId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.SubstId))?.CEPVal.ValueTo<string>();
            var substLocId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.SubstLocId))?.CEPVal.ValueTo<string>();

            if (string.IsNullOrEmpty(substId) || string.IsNullOrEmpty(substLocId))
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            var pwsStatus = E90Std.StandardServices.ProceedWithSubstrate(substId, substLocId);
            return Helpers.Helpers.ConvertAcknowledgeToHcack(pwsStatus.Acknowledge);
        }

        private HCACK TreatCancelSubstrateRcmd(IEnhancedRemoteCommand rcmd)
        {
            var substId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.SubstId))?.CEPVal.ValueTo<string>();
            var substLocId = GetParameter(rcmd.Parameters, nameof(RemoteCommandsParamters.SubstLocId))?.CEPVal.ValueTo<string>();

            if (string.IsNullOrEmpty(substId) || string.IsNullOrEmpty(substLocId))
            {
                return HCACK.AtLeastOneParameterIsInvalid;
            }

            var pwsStatus = E90Std.StandardServices.CancelSubstrate(substId, substLocId);
            return Helpers.Helpers.ConvertAcknowledgeToHcack(pwsStatus.Acknowledge);
        }
        #endregion

        private Job CreateControllerJob(IControlJob controlJob, IProcessJob processJob)
        {
            switch (processJob.MaterialType)
            {
                case MaterialType.Carriers:
                    var wafers = Helpers.Helpers.BuildWaferList(
                        processJob,
                        _controller.GetSubstrates());

                    //Search OcrProfile
                    //Implement here the default behavior if no recipe parameter is present
                    OcrProfile ocrProfile = null;
                    if (processJob.RecipeMethod == RecipeMethod.RecipeWithVariableTuning
                        && processJob.RecipeVariables.FirstOrDefault(
                                p => p.Name.Equals(RecipeParameter.OcrProfileName)) is
                            {
                            } recipeVariable)
                    {
                        ocrProfile = App.ControllerInstance.ControllerConfig.OcrProfiles.Find(
                            p => p.Name.Equals(recipeVariable.ValueAsDataItem.ValueTo<string>()));
                    }

                    return new Job(
                        processJob.ObjID,
                        controlJob.ObjID,
                        processJob.RecipeID,
                        wafers,
                        ocrProfile);

                default:
                    return null;
            }
        }

        private void ControlServices_StateChanged()
        {
            // update host control flag
            _isControlledByHost =
                ControlServices.CommunicationState == CommunicationState.Communicating
                && ControlServices.ControlState == ControlState.Remote;

            if (ControlServices.CommunicationState == CommunicationState.WaitDelay)
            {
                GUI.Common.App.Instance.AlarmCenter.Services.SetAlarm(
                    this,
                    AlNames.TcCommunicationTimeout);
            }

            foreach (var loadPort in App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                         .Values)
            {
                if (_isControlledByHost)
                {
                    loadPort.SetIsAutoHandOffEnabled();
                }
                else
                {
                    loadPort.ResetIsAutoHandOffEnabled();
                }
            }

            DefineLightsState(
                _isControlledByHost
                    ? LightTowerState.RemoteControl
                    : LightTowerState.LocalControl);
        }

        private void DefineLightsState(LightTowerState state)
        {
            var lightTower = App.ControllerInstance.ControllerEquipmentManager.LightTower;
            if (lightTower is { IsCommunicating: true })
            {
                lightTower.DefineStateAsync(state);
            }
        }

        private void ClearAlarms()
        {
            var carrierIdReadFailAlarm = GUI.Common.App.Instance.AlarmCenter.Repository
                .GetAlarmOccurrences()
                .FirstOrDefault(x => x.Alarm.Name == AlNames.CarrierIDReadFail);

            if (carrierIdReadFailAlarm is { State: AlarmState.Set })
            {
                GUI.Common.App.Instance.AlarmCenter.Services.ClearAlarm(
                    App.ControllerInstance.GemController,
                    AlNames.CarrierIDReadFail);
            }

            var doubleSlotDetectedAlarm = GUI.Common.App.Instance.AlarmCenter.Repository
                .GetAlarmOccurrences()
                .FirstOrDefault(x => x.Alarm.Name == AlNames.DoubleSlotDetected);

            if (doubleSlotDetectedAlarm is { State: AlarmState.Set })
            {
                GUI.Common.App.Instance.AlarmCenter.Services.ClearAlarm(
                    App.ControllerInstance.GemController,
                    AlNames.DoubleSlotDetected);
            }

            E30Std.AlarmServices.ClearAlarm(E87WellknownNames.Alarms.CarrierVerificationFailure);
            E30Std.AlarmServices.ClearAlarm(E87WellknownNames.Alarms.SlotMapReadFailed);
            E30Std.AlarmServices.ClearAlarm(E87WellknownNames.Alarms.SlotMapVerificationFailed);
        }

        #endregion

        #region IUserInformationProvider

        public event EventHandler<UserInformationEventArgs> UserInformationRaised;

        private void OnUserInformationRaised(string message)
        {
            UserInformationRaised?.Invoke(this, new UserInformationEventArgs(message));
        }

        public event EventHandler<UserInformationEventArgs> UserWarningRaised;

        private void OnUserWarningRaised(string message)
        {
            UserWarningRaised?.Invoke(this, new UserInformationEventArgs(message));
        }

        public event EventHandler<UserInformationEventArgs> UserErrorRaised;

        private void OnUserErrorRaised(string message)
        {
            UserErrorRaised?.Invoke(this, new UserInformationEventArgs(message));
        }

        #endregion
    }
}
